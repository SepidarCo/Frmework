using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public class StoredProcedure
    {
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; }

        public string GeneratedCode { get; set; }

        public string Schema { get; set; }

        public string FullyQualifiedName
        {
            get
            {
                if (Schema.ToLower() == "dbo" || Schema.IsNothing())
                {
                    return "[{0}]".Fill(Name);
                }
                return "[{0}].[{1}]".Fill(Schema, Name);
            }
        }
    }
}
