using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Normalization
{
    public class PersianNormalizer
    {
        public const string PersianAlphanumericPattern = @"[\p{IsArabic}0-9a-zA-Z-[\p{P}\p{M}]]+";

        public static string SelectPersianAlphanumericTextOnly(string text)
        {
            if (text.IsNothing())
            {
                return text;
            }
            var includedCharacters = new Regex(PersianAlphanumericPattern);
            var matches = includedCharacters.Matches(text).Cast<Match>().Select(i => i.Value);
            var result = string.Join(" ", matches.ToArray());
            result = Extensions.SafePersianEncode(result.ToLower());
            return result;
        }

        public static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            char ArabicYeChar = (char)1610;
            char PersianYeChar = (char)1740;

            char ArabicKeChar = (char)1603;
            char PersianKeChar = (char)1705;

            char arabicYChar2 = (char)1609;

            text = text.Replace(ArabicYeChar, PersianYeChar);
            text = text.Replace(arabicYChar2, PersianYeChar);
            text = text.Replace(ArabicKeChar, PersianKeChar);
            return text;
        }

        public static string ConvertArabicNumeralsToLatinNumerals(string text)
        {
            text = text.Replace("‏۱", "1");
            text = text.Replace('۱', '1');
            text = text.Replace('١', '1');
            text = text.Replace('٢', '2');
            text = text.Replace('۲', '2');
            text = text.Replace('۳', '3');
            text = text.Replace('٣', '3');
            text = text.Replace("‏۳", "3");
            text = text.Replace("‏‏٣", "3");
            text = text.Replace("‏۴", "4");
            text = text.Replace('۴', '4');
            text = text.Replace('٤', '4');
            text = text.Replace("‏۵", "5");
            text = text.Replace('۵', '5');
            text = text.Replace('٥', '5');
            text = text.Replace('۶', '6');
            text = text.Replace("‏۶", "6");
            text = text.Replace('٦', '6');
            text = text.Replace('٧', '7');
            text = text.Replace('۷', '7');
            text = text.Replace('٨', '8');
            text = text.Replace('۸', '8');
            text = text.Replace('۹', '9');
            text = text.Replace('٩', '9');
            text = text.Replace('٠', '0');
            text = text.Replace('۰', '0');

            return text;
        }
    }
}
