using System;
using System.Linq;

namespace Sepidar.Validation
{
    public static class NationalCodeValidator
    {
        public static bool IsNationalCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }
            if (code.Length != 10)
            {
                return false;
            }
            if (!long.TryParse(code, out long number))
            {
                return false;
            }
            var numbers = code.ToCharArray().Select(i => Convert.ToInt32(i.ToString())).ToList();
            var checkNumber = numbers.Last();
            numbers.RemoveAt(9);
            numbers.Reverse();
            var sum = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                sum += numbers[i] * (i + 2);
            }
            var remaining = sum % 11;
            if (remaining < 2)
            {
                if (remaining != checkNumber)
                {
                    return false;
                }
            }
            else
            {
                if ((11 - remaining) != checkNumber)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
