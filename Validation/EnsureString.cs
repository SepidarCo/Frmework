using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Validation
{
    public class EnsureString
    {
        string @string;

        public EnsureString(string @string)
        {
            this.@string = @string;
        }

        public EnsureString IsNationalCode()
        {
            if (!NationalCodeValidator.IsNationalCode(@string))
            {
                throw new BusinessException("کد ملی اشتباه است");
            }
            return this;
        }

        public EnsureString HasLengthGreaterThan(int length)
        {
            if (@string.Length <= length)
            {
                throw new FrameworkException("Ensure validation: '{0}' length is {1} characters, but it should be more than {2} characters.".Fill(@string, @string.Length, length));
            }
            return this;
        }

        public EnsureString HasLengthGreaterThanOrEqualTo(int length)
        {
            if (@string.Length < length)
            {
                throw new FrameworkException("Ensure validation: '{0}' length is {1} characters, but it should be more than or equal to {2} characters.".Fill(@string, @string.Length, length));
            }
            return this;
        }

        public EnsureString And()
        {
            return this;
        }

        public EnsureString IsPersian()
        {
            throw new NotImplementedException();
        }

        public EnsureString IsSomething(string message = null)
        {
            if (@string.IsNothing())
            {
                throw new FrameworkException(message ?? "Ensure validation: string is either null, or whitespace.");
            }
            return this;
        }

        public EnsureString IsEmail(string message = null)
        {
            if (EmailValidator.IsEmail(@string))
            {
                return this;
            }
            throw new FrameworkException(message ?? "Ensure validation: '{0}' is not a valid email address.".Fill(@string));
        }

        public EnsureString IsEqualTo(string target)
        {
            if (@string.ToLower() != target.ToLower())
            {
                throw new FrameworkException("Ensure validation: '{0}' is not equal to '{1}'. Comparison is made without considerint case sensitivity.".Fill(@string, target));
            }
            return this;
        }

        public EnsureString IsUrl(string message = null)
        {
            new Ensure(@string).AsString().IsSomething();
            if (!Uri.TryCreate(@string, UriKind.Absolute, out Uri uri))
            {
                throw new FrameworkException(message ?? "Ensure validation: '{0}' is not a valid URL".Fill(@string));
            }
            return this;
        }

        public EnsureString IsMobileNumber(string message = null)
        {
            if (MobileNumberValidator.IsMobileNumber(@string))
            {
                return this;
            }
            throw new FrameworkException(message ?? "{0} is not a valid cell phone number".Fill(@string));
        }

        public EnsureString IsMciMobileNumber(string message = null)
        {
            if (MobileNumberValidator.IsMciMobileNumber(@string))
            {
                return this;
            }
            throw new FrameworkException(message ?? "{0} is not a valid MCI cell phone number".Fill(@string));
        }

        public EnsureString IsIp(string message = null)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(@string, out ip))
            {
                throw new FrameworkException(message ?? "{0} is not a valid IP address".Fill(@string));
            }
            return this;
        }

        public EnsureString IsIban(string message = null)
        {
            string iban = @string;
            string error = message ?? "شماره شبا نامعتبر است";
            if (iban.IsNothing())
            {
                throw new FrameworkException(error);
            }
            iban = iban.ToUpper().Trim().Replace(" ", String.Empty);
            if (!Regex.IsMatch(iban, "^[A-Z]{2}[0-9]{24}$"))
            {
                throw new FrameworkException(error);
            }
            string bank = iban.Substring(4, iban.Length - 4) + iban.Substring(0, 4);
            int asciiShift = 55;
            StringBuilder sb = new StringBuilder();
            foreach (char c in bank)
            {
                int v;
                if (Char.IsLetter(c)) v = c - asciiShift;
                else v = int.Parse(c.ToString());
                sb.Append(v);
            }
            string checkSumString = sb.ToString();
            int checksum = int.Parse(checkSumString.Substring(0, 1));
            for (int i = 1; i < checkSumString.Length; i++)
            {
                int v = int.Parse(checkSumString.Substring(i, 1));
                checksum *= 10;
                checksum += v;
                checksum %= 97;
            }
            if (checksum != 1)
            {
                throw new FrameworkException(error);
            }
            return this;
        }

        public EnsureString IsPersianDate()
        {
            if (!PersianDateValidator.IsPersianDate(@string))
            {
                throw new BusinessException("تاریخ شمسی صحیح نیست");
            }
            return this;
        }
    }
}
