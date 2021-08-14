using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;

namespace Sepidar.Validation
{
    public class Ensure
    {
        object @object;

        public Ensure(object @object)
        {
            this.@object = @object;
        }

        public Ensure And()
        {
            return this;
        }

        public Ensure IsNotNull(string message = null)
        {
            if (@object == null)
            {
                throw new FrameworkException(message ?? "Ensure validation: object is null.");
            }
            return this;
        }

        public Ensure IsInteger()
        {
            int integer;
            if (!int.TryParse(@object.ToString(), out integer))
            {
                throw new FrameworkException("Ensure validation: object is not an integer.");
            }
            return this;
        }

        public EnsureString AsString()
        {
            if (@object == null)
            {
                return new EnsureString(null);
            }
            return new EnsureString(@object.ToString());
        }

        public EnsureNumber IsNumeric(string message = null)
        {
            decimal number;
            string error = message ?? "Ensure validation: object '{0}' is not a valid number.".Fill(@object);
            if (@object.IsNull())
            {
                throw new FrameworkException(error);
            }
            if (!decimal.TryParse(@object.ToString(), out number))
            {
                throw new FrameworkException(error);
            }
            return new EnsureNumber(number);
        }

        public EnsureDate IsDate(string message = null)
        {
            DateTime date;
            if (!DateTime.TryParse(@object.ToString(), out date))
            {
                throw new FrameworkException(message ?? "Ensure validation: '{0}' is not a date".Fill(@object));
            }
            return new EnsureDate(date);
        }

        public Ensure EqualsTo(object target, string message = null)
        {
            if (@object.ToString() == target.ToString())
            {
                return this;
            }
            throw new BusinessException(message ?? "مقادیر برابر نیستند");
        }
    }
}

/*
 * private static object EnsureNationalCode(this object code)
        {
            throw new NotImplementedException();
            return code;
        }

        public static object EnsureEnglish(this object text)
        {
            if (Regex.Match(text.ToString(), "^[a-zA-Z]*$").Success) { }
            else
            {
                throw new FrameworkException("{0} is not English and has some non-English characters.".Fill(text));
            }
            return text;
        }

        public static object EnsureNumber(this object value)
        {
            if (Regex.Match(value.ToString(), "^[0-9]*$").Success) { }
            else
            {
                throw new FrameworkException("Validation of {0} failed. It's not a number.".Fill(value));
            }
            return value;
        }

        public static object EnsureRegex(this object value, string pattern)
        {
            if (Regex.Match(value.ToString().ToString(), pattern).Success) { }
            else
            {
                throw new FrameworkException("Validation of {0} failed. It doesn't follow the {1} Regex pattern.".Fill(value, pattern));
            }
            return value;
        }

        public static void EnsureEquals(this string value, string target)
        {
            if (value != target)
            {
                throw new FrameworkException("{0} is not equal to {1}".Fill(value, target));
            }
        }

        public static object EnsureRequired(this object value)
        {
            if (value.IsNull() || value.ToString().IsNothing())
            {
                throw new FrameworkException("فیلد اجباری پر نشده است");
            }
            return value;
        }

        public static object EnsureIsSomething(this object value)
        {
            return EnsureRequired(value);
        }

        public static object EnsureLength(this object value, int length)
        {
            if (length.ToString().Length == length) { }
            else
            {
                throw new FrameworkException("The length of {0} is not {1} characters.".Fill(value, length));
            }
            return value;
        }

        public static object EnsureMobileNumber(this object value)
        {
            string number = value.ToString(); // PhoneNumberNormalizer.Normalize(value.ToString());
            number.EnsureLength(11); // A normalized mobile number: 09195527768
            if (!number.ToString().StartsWith("09"))
            {
                throw new FrameworkException("Mobile number {0} doesn't start with 09 numbers".Fill(value));
            }
            return value;
        }

        public static object EnsureEnumMember(this object value, Type enumeration)
        {
            if (!enumeration.GetEnumNames().Any(i => i.ToLower() == value.ToString().ToLower()))
            {
                throw new FrameworkException("{0} is not a member of enum {1}".Fill(value, enumeration.Name));
            }
            return value;
        }
*/
