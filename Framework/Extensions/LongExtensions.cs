using System;

namespace Sepidar.Framework.Extensions
{
    public static class LongExtensions
    {
        public static T ToEnum<T>(this long value)
        {
            if (typeof(T).BaseType.Name != typeof(Enum).Name)
            {
                throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
            }
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static string ToCommaSeparated(this long value)
        {
            return value.ToString().ToCommaSeparated();
        }

    }
}
