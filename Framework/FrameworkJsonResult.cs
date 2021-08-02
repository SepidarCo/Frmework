using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class FrameworkJsonResult //: JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException("context");
        //    }

        //    HttpResponseBase response = context.HttpContext.Response;

        //    if (!String.IsNullOrEmpty(ContentType))
        //    {
        //        response.ContentType = ContentType;
        //    }
        //    else
        //    {
        //        response.ContentType = "application/json";
        //    }
        //    if (ContentEncoding != null)
        //    {
        //        response.ContentEncoding = ContentEncoding;
        //    }
        //    if (Data != null)
        //    {
        //        var isoConvert = new IsoDateTimeConverter();
        //        isoConvert.DateTimeFormat = _dateFormat;
        //        response.Write(JsonConvert.SerializeObject(Data, isoConvert));
        //    }
        //}
    }
}
