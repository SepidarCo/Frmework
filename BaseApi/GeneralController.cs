using Microsoft.AspNetCore.Mvc;

namespace Sepidar.BaseApi
{
    public class GeneralController : ControllerBase
    {
        protected string GetQueryString()
        {
            var queryString = Request.QueryString.Value;
            return queryString;
        }

        protected string GetRemoteIpAddress()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            return remoteIpAddress;
        }

        [HttpGet]
        public dynamic Test()
        {
            return Ok("Ali Fallah");
        }
    }
}
