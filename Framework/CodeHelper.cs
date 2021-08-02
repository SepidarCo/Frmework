using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sepidar.Framework
{
    public static class CodeHelper
    {
//        public static void Compile(string code, List<string> dlls = null, List<string> usings = null, string file = null, bool createExc = true)
//        {
//            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
//            CompilerParameters parameters = new CompilerParameters();
//            parameters.ReferencedAssemblies.AddRange(new[] {
//                "mscorlib.dll",
//                "System.dll",
//                "System.Data.dll",
//                "System.Core.dll"
//            });
//            string _file = Path.Combine(Path.GetTempPath(), "{0}.{1}".Fill(Guid.NewGuid().ToString(), createExc ? "exe" : "dll"));
//            parameters.OutputAssembly = _file;
//            if (Config.HasKey("AssembliedPath"))
//            {
//                string[] assemblies = Directory.GetFiles(Config.GetSetting("AssembliesPath"), "*.dll", SearchOption.AllDirectories);
//                parameters.ReferencedAssemblies.AddRange(assemblies);
//            }
//            if (dlls.IsNotNull())
//            {
//                dlls.ForEach(dll =>
//                {
//                    parameters.ReferencedAssemblies.Add(dll);
//                });
//            }
//            string _usings = "";
//            if (usings.IsNotNull())
//            {
//                usings.ForEach(@using =>
//                {
//                    _usings += "using {0};\r\n".Fill(@using);
//                });
//            }
//            parameters.GenerateExecutable = createExc;
//            string _code = string.Format(@"
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//{0}

//namespace TempNamespace
//{{
//    public class TempCode
//    {{
//        static void Main(string[] args)
//        {{
//            {1}
//        }}
//    }}
//}}
//", _usings, code);
//            var result = provider.CompileAssemblyFromSource(parameters, _code);
//            if (result.Errors.Count > 0)
//            {
//                string message = string.Empty;
//                foreach (CompilerError error in result.Errors)
//                {
//                    message += "{0} - {1} \r\n".Fill(error.ErrorNumber, error.ErrorText);
//                }
//                throw new FrameworkException(message);
//            }
//            if (file.IsNotNull())
//            {
//                File.Copy(_file, file, true);
//            }
//        }

        public static dynamic Inject(string assemblyName, string className, string methodName, dynamic parameters)
        {
            var assembly = Assembly.Load(assemblyName);
            var @class = assembly.CreateInstance(className);
            var method = @class.GetType().GetMethods().Where(m => m.Name == methodName).Single();
            return method.Invoke(@class, new object[] { parameters });
        }

        public static object InvokeMethod(MethodInvocation invocation)
        {
            //var obj = Activator.CreateInstance(Type.GetType(invocation.ClassFullyQualifiedName));
            //obj.GetType().GetMethods().Single(i => i.Name == invocation.MethodName).Invoke(obj, invocation.InputParameters);
            var assembly = Assembly.Load(invocation.AssemblyName);
            var type = assembly.CreateInstance(invocation.ClassFullyQualifiedName);
            var method = type.GetType().GetMethods().Single(i => i.Name == invocation.MethodName);
            return method.Invoke(type, invocation.InputParameters);
        }
    }
}
