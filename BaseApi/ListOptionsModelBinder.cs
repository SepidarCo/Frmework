using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepidar.BaseApi
{
    public class ListOptionsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ListOptions))
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var valueProvider = bindingContext.ValueProvider;
            var listOptions = TryGetListOptions(valueProvider);
            if (listOptions.IsNull())
            {

                listOptions = ListOptions.Create();
                TryGetPageSize(listOptions, valueProvider);
                TryGetPageNumber(listOptions, valueProvider);
                TryGetSorts(listOptions, valueProvider);
                TryGetFilters(listOptions, valueProvider);
                FallbackNullValues(listOptions);
            }
            bindingContext.Result = ModelBindingResult.Success(listOptions);
            return Task.CompletedTask;

        }

        private ListOptions TryGetListOptions(IValueProvider valueProvider)
        {
            ValueProviderResult listOptionsJson = valueProvider.GetValue("listOptions");
            if (listOptionsJson.IsNotNull())
            {
                try
                {
                    var listOptions = listOptionsJson.FirstValue.JsonDeserialize<ListOptions>();
                    FallbackNullValues(listOptions);
                    return listOptions;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        private static void FallbackNullValues(ListOptions listOptions)
        {
            listOptions.PageNumber = listOptions.PageNumber ?? 1;
            listOptions.PageSize = listOptions.PageSize ?? Framework.Config.DefaultPageSize;
            listOptions.Sorts = listOptions.Sorts ?? new List<Sort>();
            listOptions.Filters = listOptions.Filters ?? new List<Filter>();
        }

        private static void TryGetFilters(ListOptions listOptions, IValueProvider valueProvider)
        {
            var filtersJson = valueProvider.GetValue("filters");
            if (filtersJson.IsNotNull())
            {
                try
                {
                    listOptions.Filters = filtersJson.FirstValue.ToString().JsonDeserialize<List<Filter>>();
                }
                catch (Exception)
                {
                    listOptions.Filters = new List<Filter>();
                }
            }
        }

        private void TryGetSorts(ListOptions listOptions, IValueProvider valueProvider)
        {
            var sortsJson = valueProvider.GetValue("sorts");
            if (sortsJson.IsNotNull())
            {
                try
                {
                    listOptions.Sorts = sortsJson.FirstValue.ToString().JsonDeserialize<List<Sort>>();
                }
                catch (Exception)
                {
                    listOptions.Sorts = new List<Sort>();
                }
            }
        }

        private static void TryGetPageNumber(ListOptions listOptions, IValueProvider valueProvider)
        {
            var pageNumber = valueProvider.GetValue("pageNumber");
            if (pageNumber.IsNotNull() && pageNumber.FirstValue.IsNotNull() && pageNumber.FirstValue.IsNumeric())
            {
                listOptions.PageNumber = Convert.ToInt32(pageNumber.FirstValue);
                if (listOptions.PageNumber < 1)
                {
                    listOptions.PageNumber = 1;
                }
            }
            else
            {
                listOptions.PageNumber = 1;
            }
        }

        private static void TryGetPageSize(ListOptions listOptions, IValueProvider valueProvider)
        {
            var pageSize = valueProvider.GetValue("pageSize");
            if (pageSize.IsNotNull() && pageSize.FirstValue.IsNotNull() && pageSize.FirstValue.IsNumeric())
            {
                listOptions.PageSize = Convert.ToInt32(pageSize.FirstValue);
            }
            else
            {
                listOptions.PageSize = Framework.Config.DefaultPageSize;
            }
        }
    }
}
