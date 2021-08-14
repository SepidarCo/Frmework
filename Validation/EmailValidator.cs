using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Text.RegularExpressions;

namespace Sepidar.Validation
{
    public static class EmailValidator
    {
        public static bool IsEmail(string text)
        {
            if (text.IsNothing())
            {
                return false;
            }
            var regex = new Regex(RegularExpressions.Email);
            if (!regex.Match(text).Success)
            {
                return false;
            }
            return true;
        }

        public static void ValidateEmail(string email)
        {
            email.Ensure().AsString().IsSomething("ایمیل نباید خالی باشد").And().IsEmail("ایمیل معتبر نیست");
        }
    }
}
