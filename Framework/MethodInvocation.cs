using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class MethodInvocation
    {
        public string AssemblyName { get; set; }

        public string ClassFullyQualifiedName { get; set; }

        public string MethodName { get; set; }

        public object[] InputParameters { get; set; }

        public dynamic Return { get; set; }
    }
}
