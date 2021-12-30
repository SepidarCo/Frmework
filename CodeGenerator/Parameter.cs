using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public class Parameter
    {
        public StoredProcedure StoredProcedure { get; set; }

        public string Name { get; set; }

        public string DotNetDataType { get; set; }

        public string SqlDataType { get; set; }

        public int MaxLength { get; set; }

        public bool IsNullable { get; set; }
    }
}
