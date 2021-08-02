using System;
using System.Collections.Generic;
using System.Text;
//using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Sepidar.Framework
{
     public class LogActionFilter //: ActionFilterAttribute
    {
        //public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        //{
        //    Log(new
        //    {
        //        actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
        //        actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
        //        Request = actionExecutedContext.Request != null ? new
        //        {
        //            actionExecutedContext.Request.Content,
        //            actionExecutedContext.Request.Headers,
        //            actionExecutedContext.Request.Method,
        //            actionExecutedContext.Request.RequestUri,
        //            actionExecutedContext.Request.Version
        //        } : null,
        //        Response = actionExecutedContext.Response != null ? new
        //        {
        //            actionExecutedContext.Response.Content,
        //            actionExecutedContext.Response.Headers,
        //            actionExecutedContext.Response.IsSuccessStatusCode,
        //            actionExecutedContext.Response.ReasonPhrase,
        //            actionExecutedContext.Response.StatusCode,
        //            actionExecutedContext.Response.Version,
        //        } : null,
        //        Exception = actionExecutedContext.Exception != null ? new
        //          {
        //              actionExecutedContext.Exception.InnerException,
        //              actionExecutedContext.Exception.Message,
        //              actionExecutedContext.Exception.Source,
        //              actionExecutedContext.Exception.StackTrace,
        //              actionExecutedContext.Exception.TargetSite,
        //              actionExecutedContext.Exception.Data
        //          } : null
        //    });
        //}

        //public override void OnActionExecuting(HttpActionContext actionContext)
        //{
        //    Log(new
        //    {
        //        actionContext.ControllerContext.ControllerDescriptor.ControllerName,
        //        actionContext.ActionDescriptor.ActionName,
        //        Request = new
        //        {
        //            actionContext.Request.Content,
        //            actionContext.Request.Headers,
        //            actionContext.Request.Method,
        //            actionContext.Request.RequestUri,
        //            actionContext.Request.Version
        //        },

        //    });
        //}

        private void Log(dynamic obj)
        {
            Logger.LogInfo(obj);
        }
    }
}