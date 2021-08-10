using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.Normalization
{
    public static class NumericNormalizer
    {
        public static string ExtractNumbers(this string text)
        {
            text = new string(text.ToCharArray().Where(char.IsDigit).ToArray());
            return text;
        }

        public static string ConvertEnglishNumberstoPersianNumbers(this string text)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            for (int j = 0; j < persian.Length; j++)
                text = text.Replace(j.ToString(), persian[j]);
            return text;
        }
    }
}
