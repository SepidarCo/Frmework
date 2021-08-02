using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sepidar.Framework.Extensions
{
    public static class XElementExtensions
    {
        public static XElement RemoveAllNamespaces(this XElement element)
        {
            if (!element.HasElements)
            {
                XElement xElement = new XElement(element.Name.LocalName);
                xElement.Value = element.Value;

                foreach (XAttribute attribute in element.Attributes())
                {
                    xElement.Add(attribute);
                }

                return xElement;
            }
            var result = new XElement(element.Name.LocalName, element.Elements().Select(el => RemoveAllNamespaces(el)));
            return result;
        }
    }
}
