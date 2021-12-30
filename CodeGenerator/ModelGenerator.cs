using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.CodeGenerator
{
    public abstract class ModelGenerator : Generator
    {
        public ModelGenerator(string connectionString)
            : base(connectionString)
        {
        }

        public override List<Table> Generate()
        {
            foreach (var table in Tables)
            {
                table.GeneratedCode = GenerateClassForTable(table);
            }
            return Tables;
        }

        private string GenerateClassForTable(Table table)
        {
            string classTemplate = @"{0}

{1}
{{
    public class {2}
    {{
{3}
    }}
}}
";
            var @class = "";
            var properties = "";
            foreach (var column in table.Columns)
            {
                properties += GeneratePropertyForColumn(column) + "\r\n" + "\r\n";
            }
//            properties += "        public dynamic RelatedItems { get; set; }" + "\r\n" + "\r\n";

            string @namespace;
            if (table.Schema == "" || !CodeGeneratorConfig.Schemas.Contains(table.Schema.ToLower()))
            {
                @namespace = Namespace + (table.Schema.IsSomething() ? "." + table.Schema : "");
            }
            else
            {
                @namespace = GetNamespaceBySchema(table.Schema);
            }

            @namespace += table.IsView ? ".Views" : "";
            @class = classTemplate.Fill(UsingStatements, @namespace, table.SingularName, properties);
            @class = Regex.Replace(@class, @"(\r\n){2}\r\n", "$1");
            return @class;
        }

        private string GeneratePropertyForColumn(Column column)
        {
            string propertyTemplate = @"        public {0} {1} {{ get; set; }}";
            var property = propertyTemplate.Fill(column.DotNetDataType, column.Name);
            return property;
        }

        public abstract string UsingStatements { get; }

        public abstract string Namespace { get; }

        public abstract string GetNamespaceBySchema(string schema);
    }
}
