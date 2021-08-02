using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework.AssemblyInitialization
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyInitializerAttribute : Attribute
    {
        public AssemblyInitializerAttribute()
        {
        }

        public AssemblyInitializerAttribute(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName;
    }
}
