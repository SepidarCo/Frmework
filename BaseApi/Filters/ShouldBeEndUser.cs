using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sepidar.BaseApi.Filters
{
    public class ShouldBeEndUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ServiceSecurity.IsEndUser)
            {
                context.Result = new UnauthorizedObjectResult("unauthorized access");
            }
        }
    }
}