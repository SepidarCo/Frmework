using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Text.RegularExpressions;

namespace Sepidar.Validation
{
    public static class MobileNumberValidator
    {
        public static bool IsMobileNumber(string number)
        {
            if (number.IsNothing())
            {
                return false;
            }
            var regex = new Regex(RegularExpressions.Mobile);
            if (!regex.Match(number).Success)
            {
                return false;
            }
            return true;
        }

        public static bool IsMciMobileNumber(string number)
        {
            if (!IsMobileNumber(number))
            {
                return false;
            }
            var regex = new Regex(RegularExpressions.MciMobile);
            if (!regex.Match(number).Success)
            {
                return false;
            }
            return true;
        }

        public static void ValidateMobileNumber(string number)
        {
            number.Ensure().AsString().IsSomething("شماره تلفن نباید خالی باشد").And().IsMobileNumber("شماره تلفن معتبر نیست");
        }
    }
}
