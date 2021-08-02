using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sepidar.Framework.AssemblyInitialization
{
    public class Helper
    {
        public static void InitializeLoadedAssemblies(object sender, AssemblyLoadEventArgs args)
        {
            Assembly anAssembly = args.LoadedAssembly;
            AssemblyInitializerAttribute[] initializers = (AssemblyInitializerAttribute[])anAssembly.GetCustomAttributes(typeof(AssemblyInitializerAttribute), false);
            foreach (AssemblyInitializerAttribute anInit in initializers)
            {
                Type initType = anInit.TypeName != null ? anAssembly.GetType(anInit.TypeName) : null;
                if (initType != null && initType.GetInterface("IAssemblyInitializer") != null)
                {
                    IAssemblyInitializer anInitializer = (IAssemblyInitializer)Activator.CreateInstance(initType);
                    anInitializer.Initialize();
                }
            }
        }
    }
}
