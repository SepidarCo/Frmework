using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.Framework
{
    public class ReportHelper
    {
        public static T[] FillMissingDates<T>(T[] items, Func<T, DateTime> datePropertySelector, Func<T, object> defaultValueProvider, DateTime? from = null, DateTime? to = null) where T : new()
        {
            var fromDate = from ?? items.Min(datePropertySelector);
            var toDate = to ?? items.Max(datePropertySelector);
            var sortedItems = items.OrderBy(datePropertySelector);
            var date = fromDate;
            var result = sortedItems.ToList();
            while (date <= toDate)
            {
                if (sortedItems.Any(i => datePropertySelector(i) == date))
                {
                    continue;
                }
                var t = new T();
                //datePropertySelector(t) = date;
                result.Add(t);
            }
            return null;
            /*
             * create a table out of items
             * add missing dates
             * sort by date
             * create array of table rows
             * return arrayGetList
             */
        }
    }
}
