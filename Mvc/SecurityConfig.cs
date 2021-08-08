using Sepidar.Framework;
using Sepidar.Framework.Extensions;

namespace Sepidar.Mvc
{
    public class SecurityConfig : Config
    {
        public static AuthenticationType AuthenticationType
        {
            get
            {
                if (HasSetting("AuthenticationType"))
                {
                    return GetSetting("AuthenticationType").ToEnum<AuthenticationType>();
                }
                return AuthenticationType.EmailPassword;
            }
        }

        public static string AuthenticationCookieName
        {
            get
            {
                if (HasSetting("Security:AuthenticationCookieName"))
                {
                    return GetSetting("Security:AuthenticationCookieName");
                }
                // fallback, convention over configuration
                return "AuthenticationTicket";
            }
        }

        public static string AuthenticationCookieDomain
        {
            get
            {
                return GetSetting("Security:AuthenticationCookieDomain");
            }
        }

        public static string ReturnUrlKey
        {
            get
            {
                return "returnUrl";
            }
        }

        public static string LoginPageUrl
        {
            get
            {
                if (HasSetting("LoginPageUrl"))
                {
                    return GetSetting("LoginPageUrl");
                }
                // fallback, convention over configuration
                return "/login";
            }
        }

        public static string LogoutPageUrl
        {
            get
            {
                if (HasSetting("LogoutPageUrl"))
                {
                    return GetSetting("LogoutPageUrl");
                }
                return "/logout";
            }
        }

        public static bool IsSso
        {
            get
            {
                if (HasSetting("IsSso") && GetSetting("IsSso").ToBoolean())
                {
                    return true;
                }
                return false;
            }
        }

        public static bool UseSimplestSecurity
        {
            get
            {
                var key = "Security:UseSimplestSecurity";
                if (HasSetting(key))
                {
                    return GetSetting(key).ToBoolean();
                }
                return false;
            }
        }
    }
}
