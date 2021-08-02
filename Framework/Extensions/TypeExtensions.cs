using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterface("IEnumerable") != null;
        }
    }
}
