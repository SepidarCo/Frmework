using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sepidar.BaseApi
{
    public class NeedsLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Sepidar.BaseApi.Security.IsAuthenticated)
            {
                return;
            }
            if (IsPublic(context))
            {
                return;
            }

            context.Result = new UnauthorizedObjectResult("unauthorized access");
        }

        private bool IsPublic(ActionExecutingContext context)
        {
            var isPublic = context.ActionDescriptor.GetType().GetCustomAttributes(typeof(IsPublic), true).Any();
            isPublic |= context.Controller.GetType().GetCustomAttributes(typeof(IsPublic), true).Any();
            return isPublic;
        }
    }
}
