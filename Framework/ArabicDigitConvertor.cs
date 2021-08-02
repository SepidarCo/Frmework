using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class ArabicDigitConvertor
    {
        public static int ConvertToLatinNumber(string text)
        {
            try
            {
                var characters = text.ToCharArray();
                var result = "";
                foreach (var character in characters)
                {
                    result += ConvertArabicIndicDigitToLatin(character);
                }
                return int.Parse(result);
            }
            catch (Exception ex)
            {
                throw new FrameworkException("Error converting Arabic number {0} to latin number.".Fill(text), ex);
            }
        }

        private static int ConvertArabicIndicDigitToLatin(char character)
        {
            switch ((int)character)
            {
                case 1777:
                    return 1;
                case 1778:
                    return 2;
                case 1779:
                    return 3;
                case 1780:
                    return 4;
                case 1781:
                    return 5;
                case 1782:
                    return 6;
                case 1783:
                    return 7;
                case 1784:
                    return 8;
                case 1785:
                    return 9;
                case 1776:
                    return 0;
                default:
                    throw new Exception(string.Format("{0} is not a valid Arabic-Indic digit", character));
            }
        }
    }
}
