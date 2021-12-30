using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sepidar.Framework;

namespace Sepidar.BaseApi
{
    public class ListOptionsModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(ListOptions))
            {
                return new BinderTypeModelBinder(typeof(ListOptionsModelBinder));
            }

            return null;
        }
    }
}
