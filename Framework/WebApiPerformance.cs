using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Sepidar.Framework
{
    public class WebApiPerformance //: ActionFilterAttribute, IActionFilter
    {
        //public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        //{
        //    dynamic @object = new ExpandoObject();
        //    @object.ControllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
        //    @object.ActionName = actionContext.ActionDescriptor.ActionName;
        //    @object.ExecutingTime = DateTime.Now.ToLongTimeString() + " - " + DateTime.Now.Millisecond;
        //    Logger.LogPerformance(@object);
        //    base.OnActionExecuting(actionContext);
        //}

        //public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        //{
        //    dynamic @object = new ExpandoObject();
        //    @object.ControllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
        //    @object.ActionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
        //    @object.ExecutingTime = DateTime.Now.ToLongTimeString() + " - " + DateTime.Now.Millisecond;
        //    Logger.LogPerformance(@object);
        //    base.OnActionExecuted(actionExecutedContext);
        //}
    }
}
