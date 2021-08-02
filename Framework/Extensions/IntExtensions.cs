using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class IntExtensions
    {
        public static T ToEnum<T>(this int value)
        {
            if (typeof(T).BaseType.Name != typeof(Enum).Name)
            {
                throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
            }
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static bool IsZero(this int number)
        {
            return number == 0;
        }

        public static string ToSplittedRelativePath(this int number, bool trimSlashes = true)
        {
            string path = string.Empty;
            number.ToString().ToCharArray().ToList().ForEach(c =>
            {
                path += "/{0}".Fill(c.ToString());
            });
            return path.Trim('/');
        }

        public static bool IsValidFor<T>(this int value, bool enumHasZero = false)
        {
            if (!enumHasZero && value == 0)
            {
                return false;
            }
            int all = 0;
            foreach (string name in Enum.GetNames(typeof(T)))
            {
                all |= (int)Enum.Parse(typeof(T), name);
            }
            var result = value & ~all;
            return result == 0;
        }

        public static bool CanBeCastTo<T>(this int value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return false;
            }
            var canBeCast = Enum.GetValues(type).Cast<int>().ToList().Contains((int)value);
            if (!canBeCast)
            {
                return false;
            }
            return true;
        }
        
        public static string ToCommaSeparated(this int value)
        {
            return value.ToString().ToCommaSeparated();
        }
    }
}
