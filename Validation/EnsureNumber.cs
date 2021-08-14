using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Linq;

namespace Sepidar.Validation
{
    public class EnsureNumber
    {
        decimal number;

        public EnsureNumber(decimal number)
        {
            this.number = number;
        }

        public EnsureNumber IsGreaterThanZero()
        {
            if (number <= 0)
            {
                throw new FrameworkException("Ensure validation: {0} should be greater than zero.".Fill(number));
            }
            return this;
        }

        public EnsureNumber And()
        {
            return this;
        }

        public EnsureNumber IsInteger()
        {
            if (!int.TryParse(number.ToString(), out int integer))
            {
                throw new FrameworkException("Ensure validation: object is not an integer.");
            }
            return this;
        }

        public EnsureNumber CanBeCastTo<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new FrameworkException("{0} is not an enum".Fill(type.FullName));
            }
            var canBeCast = Enum.GetValues(type).Cast<int>().ToList().Contains((int)number);
            if (!canBeCast)
            {
                throw new FrameworkException("{0} is not a valid member of {1}, thus can't be cast to it".Fill(number, type.FullName));
            }
            return this;
        }
    }
}
