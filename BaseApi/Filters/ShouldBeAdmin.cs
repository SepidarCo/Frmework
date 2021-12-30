using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sepidar.BaseApi.Filters
{
    public class ShouldBeAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ServiceSecurity.IsAdmin)
            {
                context.Result = new UnauthorizedObjectResult("unauthorized access");
            }
        }
    }
}