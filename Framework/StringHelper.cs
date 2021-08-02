using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class StringHelper
    {
        public static string ExtractXmlTagContent(string soap, string tagName)
        {
            int startPos = soap.IndexOf("<" + tagName + ">");
            if (startPos < 0)
                return "";

            int endPos = soap.IndexOf("</" + tagName + ">");

            if (endPos < 0)
                return "";

            int length = endPos - startPos - tagName.Length - 2;
            return soap.Substring(startPos + tagName.Length + 2, length);
        }
    }
}
