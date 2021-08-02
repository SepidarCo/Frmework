using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class UriExtensions
    {
        public static long GetSize(this Uri uri)
        {
            var request = HttpWebRequest.Create(uri);
            request.Method = "HEAD";
            using (WebResponse response = request.GetResponse())
            {
                var size = response.Headers["Content-Length"];
                if (size.IsNull())
                {
                    throw new FrameworkException("Response head does not have Content-Length header field");
                }
                if (!size.IsNumeric())
                {
                    throw new FrameworkException("Content-Length header value is not valid");
                }
                return Convert.ToInt64(size);
            }
        }
    }
}
