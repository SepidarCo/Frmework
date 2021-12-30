using System.Collections.Generic;
using System.Linq;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;

namespace Sepidar.CodeGenerator
{
    public class Table
    {
        EnglishPluralizationService englishPluralizationService = new EnglishPluralizationService();
        public int ObjectId { get; set; }

        public string TableName { get; set; }

        public string SingularName
        {
            get
            {
                if (CodeGeneratorConfig.ConsiderPluralization)
                {
                    return englishPluralizationService.Singularize(TableName);
                }
                return TableName;
            }
        }

        public string PluralName
        {
            get
            {
                if (CodeGeneratorConfig.ConsiderPluralization)
                {
                    
                    return englishPluralizationService.Pluralize(TableName);
                }
                return TableName;
            }
        }

        public List<Column> Columns { get; set; }

        public string GeneratedCode { get; set; }

        public string Schema { get; set; }

        public string GetSchemaPath()
        {
            var schemaParts = Schema.Split('.').ToList();
            var path = string.Join("\\", schemaParts.ToArray());
            return path;
        }

        public string FullyQualifiedName
        {
            get
            {
                if (Schema.ToLower() == "dbo" || Schema.IsNothing())
                {
                    return "[{0}]".Fill(PluralName);
                }
                return "[{0}].[{1}]".Fill(Schema, PluralName);
            }
        }

        public bool IsView { get; set; }

        public List<UniqueKey> UniqueKeys { get; set; }

        public bool HasUniqueKeys
        {
            get
            {
                return UniqueKeys.Count > 0;
            }
        }
    }
}