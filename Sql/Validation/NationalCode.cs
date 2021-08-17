using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Data;

namespace Sepidar.Sql.Validation
{
    public class NationalCode
    {
      // [SqlFunction]
        public static SqlBoolean IsNationalCode(SqlString text)
        {
            if (text.IsNull)
            {
                return false;
            }
            var @string = text.Value;
            if (string.IsNullOrWhiteSpace(@string))
            {
                return false;
            }
            if (@string.Length != 10)
            {
                return false;
            }
            long number;
            if (!long.TryParse(@string, out number))
            {
                return false;
            }
            var numbers = @string.ToCharArray().Select(i => Convert.ToInt32(i.ToString())).ToList();
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
