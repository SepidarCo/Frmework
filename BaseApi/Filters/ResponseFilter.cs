using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;

namespace Sepidar.BaseApi.Filters
{
    public class ResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception.IsNotNull())
            {
                CreateErrorResponse(context);
            }
            else
            {
                CreateSuccessResponse(context);
            }
        }

        private static void CreateSuccessResponse(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var response = CreateResponseObject("success", "", objectResult.Value);
                objectResult.Value = response;
            }
        }

        private static void CreateErrorResponse(ActionExecutedContext context)
        {
            var response = CreateResponseObject("error", context.Exception.Message, "");

            context.Result = new OkObjectResult(context.ModelState);

            var objectResult = (ObjectResult) context.Result;
            objectResult.Value = response;

            Logger.Log(context.Exception);

            context.Exception = null;
            context.ExceptionDispatchInfo = null;
        }

        private static Response CreateResponseObject(string status, string message, object data)
        {
            if (data.IsNotNull() && data.ToString().IsNothing())
            {
                data = new object();
            }
            var response = new Response
            {
                Status = status,
                Message = message,
                Result = data
            };
            return response;
        }
    }
}
