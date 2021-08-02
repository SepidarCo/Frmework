using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Framework
{
    public class TemplateEngine
    {
        //public static string Render(string template, object model)
        //{
        //    var result = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);
        //    return result;
        //}

        public static string LightRender(string template, object model)
        {
            if (model.IsNull())
            {
                return template;
            }
            var parameters = Regex.Matches(template, @"(?<!@)@(\w+)").Cast<Match>().ToList();
            var properties = model.GetType().GetProperties().ToList();
            foreach (var parameter in parameters)
            {
                var property = properties.SingleOrDefault(i => i.Name == parameter.Groups[1].Value);
                if (property.IsNull())
                {
                    throw new FrameworkException("Property {0} does not exist in parameters".Fill(parameter));
                }
                var value = property.GetValue(model, null).ToString();
                template = template.Replace(parameter.Value, value);
            }
            template = template.Replace("@@", "@");
            return template;
        }
    }
}
