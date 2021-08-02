using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class StringListExtensions
    {
        public static string ToJsonArray(this List<string> list)
        {
            string result = string.Empty;
            list.ForEach(i =>
            {
                result += "'{0}',".Fill(i);
            });
            result = "[{0}]".Fill(result.Trim(','));
            return result;
        }
    }
}
