using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Normalization
{
    public static class Extensions
    {
        public static string SafePersianEncode(this string text)
        {
            return PersianNormalizer.Normalize(text);
        }
    }
}
