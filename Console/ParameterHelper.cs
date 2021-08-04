using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Linq;

namespace Sepidar.Console
{
    public class ParameterHelper
    {
        public static string Get(string[] args, string key)
        {
            var argument = args.FirstOrDefault(i => i.Contains(key));
            if (argument.IsNull())
            {
                throw new FrameworkException("Parameter '{0}' is not provided.".Fill(key));
            }
            if (!argument.Contains("="))
            {
                throw new FrameworkException("Parameter '{0}' is has no value. Specify its value like {0}=value.".Fill(key));
            }
            return argument.Split('=')[1];
        }
    }
}
