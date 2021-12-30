using System;

using Sepidar.Framework.Extensions;

namespace Sepidar.BaseApi
{
    public class CommonSecurity : Sepidar.BaseApi.Security
    {
        public static new bool IsAdmin
        {
            get
            {
                if (ParsedUserData.IsNull())
                {
                    return false;
                }
                return ParsedUserData.IsAdmin;
            }
        }

        public static new bool IsEndUser
        {
            get
            {
                if (ParsedUserData.IsNull())
                {
                    return false;
                }
                return ParsedUserData.IsEndUser;
            }
        }

        public static UserData ParsedUserData
        {
            get
            {
                try
                {
                    return UserData.JsonDeserialize<UserData>();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
