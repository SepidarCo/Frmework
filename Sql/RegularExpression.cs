using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Sql
{
    public class RegularExpression
    {
       // [SqlFunction]
        public static SqlBoolean Matches(SqlString text, SqlString pattern)
        {
            var regex = new Regex(pattern.Value);
            return regex.IsMatch(text.Value);
        }

       // [SqlFunction]
        public static SqlString RegexReplace(SqlString text, SqlString pattern, SqlString replacement)
        {
            var regex = new Regex(pattern.Value);
            return regex.Replace(text.Value, replacement.Value);
        }
    }
}
