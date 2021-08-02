using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Sepidar.Framework
{
    public class ListOptions
    {
        private ListOptions()
        {

        }

        public List<Sort> Sorts { get; set; }

        public List<Filter> Filters { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public bool ReturnAll { get; set; }

        public bool HasSorts
        {
            get
            {
                return Sorts.Count > 0;
            }
        }

        public bool HasFilters
        {
            get
            {
                return Filters.Count > 0;
            }
        }

        public void AddFilter(Filter filter)
        {
            if (Filters.IsNull())
            {
                Filters = new List<Filter>();
            }
            Filters.Add(filter);
        }

        public Filter GetFilter(string property)
        {
            var filter = Filters.FirstOrDefault(i => i.Property.ToLower() == property.ToLower());
            return filter;
        }

        public void AddFilter<T>(Expression<Func<T, object>> propertySelector, string value, FilterOperator filterOperator = FilterOperator.Equal)
        {
            string property;
            if (propertySelector.Body is MemberExpression)
            {
                property = ((MemberExpression)propertySelector.Body).Member.Name;
            }
            else
            {
                var operand = ((UnaryExpression)propertySelector.Body).Operand;
                property = ((MemberExpression)operand).Member.Name;
            }
            AddFilter(new Filter
            {
                Property = property,
                Value = value,
                Operator = FilterOperatorNormalizer.NormalizeFilterOperator(filterOperator)
            });
        }

        public Filter GetFilter<T>(Expression<Func<T, object>> propertySelector)
        {
            string property;
            if (propertySelector.Body is MemberExpression)
            {
                property = ((MemberExpression)propertySelector.Body).Member.Name;
            }
            else
            {
                var operand = ((UnaryExpression)propertySelector.Body).Operand;
                property = ((MemberExpression)operand).Member.Name;
            }

            var filter = Filters.Find(i => i.Property == property);
            return filter;
        }

        public void AddSort(Sort sort)
        {
            if (Sorts.IsNull())
            {
                Sorts = new List<Sort>();
            }
            Sorts.Add(sort);
        }

        public void AddSort<T>(Expression<Func<T, object>> propertySelector, string direction)
        {
            string property;
            if (propertySelector.Body is MemberExpression)
            {
                property = ((MemberExpression)propertySelector.Body).Member.Name;
            }
            else
            {
                var operand = ((UnaryExpression)propertySelector.Body).Operand;
                property = ((MemberExpression)operand).Member.Name;
            }
            AddSort(new Sort
            {
                Property = property,
                Direction = direction
            });
        }

        public static ListOptions Create()
        {
            var listOptions = new ListOptions();
            listOptions.PageSize = Config.DefaultPageSize;
            listOptions.PageNumber = 1;
            listOptions.Filters = new List<Filter>();
            listOptions.Sorts = new List<Sort>();
            return listOptions;
        }

        public static void ProvideDefaultValues(ListOptions listOptions)
        {
            if (listOptions == null)
            {
                listOptions = ListOptions.Create();
            }
            listOptions.PageNumber = listOptions.PageNumber ?? 1;
            if (listOptions.PageNumber < 1)
            {
                listOptions.PageNumber = 1;
            }
            listOptions.PageSize = listOptions.PageSize ?? Config.DefaultPageSize;
            if (listOptions.Filters == null)
            {
                listOptions.Filters = new List<Filter>();
            }
            if (listOptions.Sorts == null)
            {
                listOptions.Sorts = new List<Sort>();
            }
        }
    }
}
