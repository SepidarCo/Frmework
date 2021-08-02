using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class SortExtensions
    {
        public static List<Sort> SafeAdd(this List<Sort> sorts, Sort sort)
        {
            if (sorts.IsNull())
            {
                sorts = new List<Sort>();
            }
            sorts.Add(sort);
            return sorts;
        }
    }
}
