using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sepidar.Framework
{
    public class ReflectionHelper
    {
        static ReflectionHelper()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainReflectionOnlyAssemblyResolve;
        }

        public static Assembly SafeLoad(string path)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(path);
            return assembly;
        }

        private static Assembly CurrentDomainReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var dllName = args.Name.Split(',')[0];
            var dllFiles = Directory.GetFiles(Environment.ExpandEnvironmentVariables("%SunProjectsRoot%"), "{0}.dll".Fill(dllName), SearchOption.AllDirectories);
            var dllFile = dllFiles.FirstOrDefault(i => File.Exists(i));
            if (dllFile.IsNull())
            {
                throw new FrameworkException("{0} does not exist anywhere in the {1} folder".Fill(dllName, Environment.ExpandEnvironmentVariables("%SunProjectsRoot%")));
            }
            return Assembly.ReflectionOnlyLoadFrom(dllFile);
        }
    }
}
