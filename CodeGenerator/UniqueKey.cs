using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public class UniqueKey
    {
        public string Name { get; set; }

        public List<Column> Columns { get; set; }

        public Table Table { get; set; }
    }
}
