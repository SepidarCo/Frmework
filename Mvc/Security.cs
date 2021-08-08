using System;
using Sepidar.Framework.Extensions;
using Sepidar.Validation;
using Microsoft.AspNetCore.Http;
using Sepidar.Framework;
using Sepidar.Security;

namespace Sepidar.Mvc
{
    public class Security
    {
        public static void SetAuthenticationCookie(long userId, object userData)
        {
            var clientToken = GenerateClientToken();
            var value = "{0}-{1}-{2}".Fill(userId, userData.JsonSerialize(), clientToken);
            value = value.AesEncrypt();

            var cookieOptions = CreateCookieOptions(DateTime.Now.AddYears(1));

            var key = SecurityConfig.AuthenticationCookieName;
            AppHttpContext.Current.Response.Cookies.Append(key, value, cookieOptions);
        }

        public static CookieOptions CreateCookieOptions(DateTime expireDate)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expireDate,
                Domain = SecurityConfig.AuthenticationCookieDomain
            };
            return cookieOptions;
        }

        public static void DeleteAuthenticationCookie()
        {
            if (AuthenticationCookie.IsSomething())
            {
                var authenticationCookieName = SecurityConfig.AuthenticationCookieName;

                var cookieOptions = CreateCookieOptions(DateTime.Now.AddDays(-1));
                AppHttpContext.Current.Response.Cookies.Append(authenticationCookieName, "", cookieOptions);
            }
        }

        private static string GenerateClientToken()
        {
            var ua = AppHttpContext.Current.Request.Headers["User-Agent"].ToString();
            var clientToken = ua.Hash().Replace("-", "");
            return clientToken;
        }


        public static bool IsAuthenticated
        {
            get
            {
                return HasAuthenticationCookie && AuthenticationCookieIsValid;
            }
        }

        protected static string AuthenticationCookie
        {
            get
            {
                string authenticationCookie = AppHttpContext.Current.Request.Cookies[SecurityConfig.AuthenticationCookieName];
                if (authenticationCookie.IsNothing())
                {
                }
                return authenticationCookie;
            }
        }

        public static bool HasAuthenticationCookie
        {
            get
            {
                return AuthenticationCookie.IsNotNull();
            }
        }


        public static bool AuthenticationCookieIsValid
        {
            get
            {
                try
                {
                    var value = AuthenticationCookie.AesDecrypt();
                    if (SecurityConfig.UseSimplestSecurity)
                    {
                        return true;
                    }
                    if (SecurityConfig.AuthenticationType == AuthenticationType.EmailPassword)
                    {
                        var email = value.Split('-')[0];
                        email.Ensure().AsString().IsEmail();
                    }
                    var clientToken = value.Split('-')[2];
                    Logger.LogInfo("Authentication: cookie client token = {0} & request client token = {1}".Fill(clientToken, GenerateClientToken()));
                    clientToken.Ensure().AsString().IsEqualTo(GenerateClientToken());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static string UserData
        {
            get
            {
                if (AuthenticationCookieIsValid)
                {
                    var value = AuthenticationCookie.AesDecrypt();
                    var userData = value.Split('-')[1];
                    return userData;
                }
                return null;
            }
        }

    }
}
