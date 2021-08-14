using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Text.RegularExpressions;

namespace Sepidar.Validation
{
    public static class UserNameValidator
    {
        public static bool IsUserName(string text)
        {
            if (text.IsNothing())
            {
                return false;
            }
            var regex = new Regex(RegularExpressions.UserName);
            if (!regex.Match(text).Success)
            {
                return false;
            }
            return true;
        }
    }
}
