using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class ListExtensions
    {
        private static List<string> operatorList = new List<string> { "<", ">", "<=", ">=", "=", "<>", "!=" };

        public static T Random<T>(this List<T> list)
        {
            var randomItem = list.OrderBy(i => Guid.NewGuid()).FirstOrDefault();
            return randomItem;
        }

        public static string ToDelimitedSeparatedValues(this List<object> items, string delimiter)
        {
            return string.Join(delimiter, items.ToArray());
        }

        public static string ToDelimitedSeparatedValues(this List<string> items, string delimiter)
        {
            return items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues(delimiter);
        }

        public static string ToCsv(this List<string> items)
        {
            var csv = items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues(",");
            csv = csv.TrimStart(',').TrimEnd(',').Trim();
            return csv;
        }

        public static string Merge(this List<string> items)
        {
            var merged = items.Select(i => (object)i).ToList().ToDelimitedSeparatedValues("\r\n");
            return merged;
        }

        public static IQueryable<T> RandomRecords<T>(this IQueryable<T> query, int count)
        {
            var result = query.OrderBy(i => Guid.NewGuid()).Take<T>(count);
            return result;
        }

        public static ListResult<T> ApplyListOptionsAndGetTotalCount<T>(this IQueryable<T> query, ListOptions listOptions)
        {
            ListOptions.ProvideDefaultValues(listOptions);
            var listResult = new ListResult<T>();
            var totalQueryable = FilterAndSort(query, listOptions.Filters, listOptions.Sorts);
            if (listOptions.ReturnAll)
            {
                listResult.Data = totalQueryable.ToList();
            }
            else
            {
                listResult.Data = totalQueryable.Skip<T>((listOptions.PageNumber.Value - 1) * listOptions.PageSize.Value).Take<T>(listOptions.PageSize.Value).ToList();
            }
            listResult.TotalCount = totalQueryable.Count();
            listResult.PageSize = listOptions.PageSize;
            listResult.PageNumber = listOptions.PageNumber;
            return listResult;
        }

        public static IQueryable<T> FilterAndSort<T>(this IEnumerable<T> query, List<Filter> filter, List<Sort> sort)
        {
            return FilterAndSort(query.AsQueryable(), filter, sort);
        }

        public static IQueryable<T> FilterAndSort<T>(this IQueryable<T> query, List<Filter> filters, List<Sort> sorts)
        {
            query = ApplyFilters<T>(query, filters);
            query = ApplySorts<T>(query, sorts);
            return query;
        }

        private static IQueryable<T> ApplySorts<T>(IQueryable<T> query, List<Sort> sorts)
        {
            if (sorts.IsNull())
            {
                query = query.OrderBy("Id asc");
                return query;
            }
            sorts = sorts.Where(s => s.Property != null && s.Direction != null).ToList();
            if (sorts.Count == 0)
            {
                query = query.OrderBy("Id asc");
                return query;
            }

            string orderParams = "";
            sorts.ForEach(i => orderParams += i.Property + " " + i.Direction + ",");
            orderParams = orderParams.TrimEnd(',');
            query = query.OrderBy(orderParams);

            return query;
        }

        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> items, List<Filter> filters)
        {
            if (filters == null)
                return items;
            filters = GetValidFilters(filters);
            NormalizeFilters(filters);

            foreach (var filter in filters)
            {
                items = ApplyFilter<T>(items, filter);
            }
            return items;
        }

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> items, Filter filter)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperties().FirstOrDefault(i => i.Name.ToLower() == filter.Property.ToLower());
            if (propertyInfo.IsNull())
            {
                return items;
            }
            if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                items = FilterDate<T>(items, filter);
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                items = FilterString<T>(items, filter);
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                items = FilterEnum<T>(items, propertyInfo, filter);
            }
            else if (propertyInfo.PropertyType == typeof(Boolean))
            {
                items = items.FilterBoolean<T>(filter);
            }
            else
            {
                if (filter.Property.Contains(' ') || filter.Value.Contains(' '))
                    throw new FrameworkException("Request is invalid");
                var filterClause = "{0} {1} {2}".Fill(filter.Property, filter.Operator, filter.Value);
                items = items.Where(filterClause);
            }
            return items;
        }

        private static IQueryable<T> FilterBoolean<T>(this IQueryable<T> items, Filter filter)
        {
            if (filter.Operator != "=")
                throw new FrameworkException("Filter operator is not valid");
            var filterClause = "{0} = {1}".Fill(filter.Property, filter.Value.ToBoolean());
            items = items.Where(filterClause);
            return items;
        }

        private static IQueryable<T> FilterEnum<T>(this IQueryable<T> items, PropertyInfo propertyInfo, Filter filter)
        {
            if (filter.Operator != "=")
                throw new FrameworkException("Filter operator is not valid");
            try
            {
                var value = Enum.Parse(propertyInfo.PropertyType, filter.Value);
                var filterClause = "{0} = \"{1}\"".Fill(filter.Property, value);
                items = items.Where(filterClause);
                return items;
            }
            catch (Exception ex)
            {
                throw new FrameworkException("{0} is not a valid value for {1}".Fill(filter.Value, propertyInfo.PropertyType.Name), ex);
            }
        }

        private static IQueryable<T> FilterString<T>(IQueryable<T> items, Filter filter)
        {
            var values = filter.Value.Split(' ');
            var whereCluase = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    whereCluase += " AND ";
                }
                if (filter.Operator == "=")
                    whereCluase += (filter.Property + ".Contains(@" + i + ")");
                else
                    whereCluase += (filter.Property + " != @" + i + "");
            }
            items = items.Where(whereCluase, values);
            return items;
        }

        private static IQueryable<T> FilterDate<T>(IQueryable<T> items, Filter filter)
        {
            DateTime date;
            if (DateTime.TryParse(filter.Value, out date))
            {
                items = items.Where(filter.Property + " " + filter.Operator + " (@0)", date);
            }
            return items;
        }

        private static void NormalizeFilters(List<Filter> filters)
        {
            foreach (var filter in filters)
            {
                filter.Property = filter.Property.Trim();
                filter.Value = filter.Value.Trim();
            }
        }

        private static List<Filter> GetValidFilters(List<Filter> filters)
        {
            // todo: how can we prevent any type of attach here?
            foreach (var filter in filters)
            {
                if (filter.Operator.IsNothing())
                    filter.Operator = "=";
            }
            var validFilters = filters.Where(i => IsValid(i)).ToList();
            return validFilters;
        }

        private static bool IsValid(Filter i)
        {
            var isValid = i.Property.IsSomething() && operatorList.Contains(i.Operator) && i.Value.IsSomething();
            return isValid;
        }

        public static List<List<T>> GetCombinations<T>(this List<T> list)
        {
            var result = Enumerable
                .Range(1, (1 << list.Count) - 1)
                .Select(index => list.Where((item, idx) => ((1 << idx) & index) != 0).ToList())
                .ToList();
            return result;
        }

        public static string ToJavaScriptArray<T>(this List<T> list)
        {
            List<T> trimmedList = list.Select(i => (T)Convert.ChangeType(i.ToString().Trim(), typeof(T))).ToList<T>();
            bool isString = typeof(T) == typeof(string);
            string separator = isString ? "','" : ",";
            string start = isString ? "['" : "[";
            string end = isString ? "']" : "]";
            return "{0}{1}{2}".Fill(start, string.Join(separator, trimmedList.ToArray<T>()), end);
        }

        public static string ToCsv<T>(this List<T> list)
        {
            var stringList = list.Select(i => i.ToString()).ToList();
            return stringList.ToCsv();
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } : elements.SelectMany((e, i) => elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return Permutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
