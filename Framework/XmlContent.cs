using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Sepidar.Framework
{
    public class XmlContent : StringContent
    {
        public XmlContent(string content)
            : base(content, Encoding.UTF8, "application/xml")
        {
        }
    }
}