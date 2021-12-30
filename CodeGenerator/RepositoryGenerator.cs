using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public abstract class RepositoryGenerator : Generator
    {

        string perModelClassTemplate = @"{0}

{1}
{{
    public partial class {2}Repository : {3}Repository<{8}>
    {{
        public {2}Repository()
            : base(new {4}DbContext())
        {{
        }}{5}{6}{7}
    }}
}}
";

        string generalclassTemplate = @"{0}

{1}
{{
    public partial class {2}Repository : {3}Repository<{8}>
    {{
        public {2}Repository(AppDbContext context)
            : base(context)
        {{
        }}{5}{6}{7}
    }}
}}
";

        string existenceFilter = @"

        public override Expression<Func<{0}, bool>> ExistenceFilter({0} t)
        {{
            Expression<Func<{0}, bool>> result = null;
            if (t.Id > 0)
            {{
                result = i => i.Id == t.Id;
            }}
            else
            {{
                {1}
            }}
            return result;
        }}
";

        string bulkUpdateUpdateClause = @"

        public override string BulkUpdateUpdateClause
        {{
            get
            {{
                return ""{0}"";
            }}
        }}";

        string bulkUpdateInsertClause = @"

        public override string BulkUpdateInsertClause
        {{
            get
            {{
                return ""{0}"";
            }}
        }}";

        string bulkUpdateComparisionKey = @"

        public override string BulkUpdateComparisonKey
        {{
            get
            {{
                {0};
            }}
        }}";

        string tempTableCreationScript = @"
        public override string TempTableCreationScript(string tempTableName)
        {{
            var tempTableScript =  @""
                    create table {{0}}
                    ({0}    
                    )
                    "".Fill(tempTableName);
            return tempTableScript;
        }}";

        public RepositoryGenerator(string connectionString)
            : base(connectionString)
        {
        }

        public override List<Table> Generate()
        {
            foreach (var table in Tables)
            {
                table.GeneratedCode = GenerateRepositoryForClass(table);
            }

//            GenerateRepositoryServicesForStartup();

            return Tables;
        }

        public void GenerateRepositoryServicesForStartup(string startupFileTemplatePath)
        {
            var text = File.ReadAllText(startupFileTemplatePath);

            var repositoryServicesStringBuilder = new StringBuilder();
            foreach (var table in Tables)
            {
                var @namespace = GetNamespace(table);
                var entityFullyQualifiedName = GetTableName(table, @namespace);
                var repositoryName = @namespace.Replace("namespace ", "") +  "." + table.SingularName +  "Repository";

                repositoryServicesStringBuilder.Append(@"
            services.AddScoped<Repository<{0}>, {1}>();".Fill(entityFullyQualifiedName, repositoryName));
                repositoryServicesStringBuilder.AppendLine();
            }

            text = text.Replace("// InjectRepositories", repositoryServicesStringBuilder.ToString());

            GeneratedInjectRepositoryServicesCode = text;
        }

        private string GenerateRepositoryForClass(Table table)
        {
            var @class = "";
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
            var usingStatements = UsingStatements;
            usingStatements += "\r\n" + ContextNamespaceFinder(@namespace, table.Schema) + ";";

            string classTemplate;
            if (CodeGeneratorConfig.CreateDbContextPerModel)
            {
                classTemplate = perModelClassTemplate;
            }
            else
            {
                classTemplate = generalclassTemplate;
            }

            @class = classTemplate.Fill(usingStatements,
                @namespace,
                table.SingularName,
//                table.IsView ? "View" : "",
                "",
                table.SingularName,
                table.IsView ? "" : BulkOperationMethods(table, @namespace),
                ExistenceFilter(table, @namespace),
                table.IsView ? "" : TempTableCreationScript(table),
                GetTableFqn(table, @namespace));
            return @class;
        }

        private string ExistenceFilter(Table table, string @namespace)
        {
            string comparisonExpression = "result = i => i.Id == t.Id;".Fill(table.PluralName);
            if (table.HasUniqueKeys)
            {
                if (table.UniqueKeys.Count > 1)
                {
                    comparisonExpression = "result = i => i.Id == t.Id;".Fill(table.PluralName);
                }
                else
                {
                    var uniqueKey = table.UniqueKeys[0];
                    if (uniqueKey.Columns.Count > 1)
                    {
                        //comparisonExpression = "result = i => " + table.UniqueKeys[0].Columns.Select(i => i.Name).Aggregate((a, b) => "i.{0} == t.{0} && i.{1} == t.{1}".Fill(a, b)) + ";";
                        comparisonExpression = "result = i => " + table.UniqueKeys[0].Columns.Select(i => "(i.{0} == t.{0} && i.{0} != null)".Fill(i.Name)).Aggregate((a, b) => "{0} && {1}".Fill(a, b)) + ";";
                    }
                    else
                    {
                        var columnName = uniqueKey.Columns[0].Name;
                        comparisonExpression = "result = i => i.{0} == t.{0} && i.{0} != null;".Fill(columnName);
                    }
                }
            }
            var result = existenceFilter.Fill(GetTableFqn(table, @namespace), comparisonExpression);
            return result;
        }

        private string BulkOperationMethods(Table table, string @namespace)
        {
            string configureDataTable = ConfigureDataTable(table);
            string addRecord = AddRecord(table, @namespace);
            string tableName = TableName(table);
            string addColumnMappings = AddColumnMappings(table);
            string bulkUpdateComparisonKey = BulkUpdateComparisonKey(table);
            string bulkUpdateInsertClause = BulkUpdateInsertClause(table);
            string bulkUpdateUpdateClause = BulkUpdateUpdateClause(table);

            return "{0}{1}{2}{3}{4}{5}{6}".Fill(configureDataTable, addRecord, tableName, addColumnMappings, bulkUpdateComparisonKey, bulkUpdateInsertClause, bulkUpdateUpdateClause);
        }

        private string BulkUpdateUpdateClause(Table table)
        {
            var columns = table.Columns.Where(i => !i.IsComputed && !i.IsIdentity).Select(i => "[{0}]".Fill(i.Name)).ToList();
            if (columns.Count == 0)
            {
                return "";
            }
            var updateClause = string.Join(", ", columns.Select(i => "t.{0} = s.{0}".Fill(i)).ToArray());
            var result = bulkUpdateUpdateClause.Fill(updateClause);
            return result;
        }

        private string BulkUpdateInsertClause(Table table)
        {
            var columns = table.Columns.Where(i => !i.IsComputed && !i.IsIdentity).Select(i => "[{0}]".Fill(i.Name)).ToList();
            if (columns.Count == 0)
            {
                return "";
            }
            var intoClause = columns.Aggregate((a, b) => "{0}, {1}".Fill(a, b));
            var fromClause = columns.Select(i => "s." + i).Aggregate((a, b) => "{0}, {1}".Fill(a, b));
            var insertClause = "({0}) values ({1})".Fill(intoClause, fromClause);
            var result = bulkUpdateInsertClause.Fill(insertClause);
            return result;
        }

        private string BulkUpdateComparisonKey(Table table)
        {
            string comparisonExpression = @"return ""(t.[Id] = s.[Id])";
            if (table.HasUniqueKeys && table.UniqueKeys.Count == 1)
            {
                var uniqueKey = table.UniqueKeys[0];
                if (uniqueKey.Columns.Count > 1)
                {
                    //comparisonExpression = @"return """ + table.UniqueKeys[0].Columns.Select(i => i.Name).Aggregate((a, b) => "t.[{0}] = s.[{0}] and t.[{1}] = s.[{1}]".Fill(a, b)) + @""";";
                    comparisonExpression += @" or (" + table.UniqueKeys[0].Columns.Select(i => "t.[{0}] = s.[{0}]".Fill(i.Name)).Aggregate((a, b) => "{0} and {1}".Fill(a, b)) + @")";
                }
                else
                {
                    var columnName = uniqueKey.Columns[0].Name;
                    comparisonExpression += @" or (t.[{0}] = s.[{0}])".Fill(columnName);
                }
            }
            comparisonExpression += "\";";
            var result = bulkUpdateComparisionKey.Fill(comparisonExpression);
            return result;
        }

        private string AddColumnMappings(Table table)
        {
            var columnMappings = new StringBuilder();
            foreach (var column in table.Columns)
            {
                if (column.IsComputed)
                {
                    continue;
                }
                columnMappings.Append("\r\n\t\t\tbulkOperator.ColumnMappings.Add(\"{0}\", \"[{0}]\");".Fill(column.Name));
            }

            var addColumnManppings = @"

        public override void AddColumnMappings(SqlBulkCopy bulkOperator)
        {{{0}
        }}".Fill(columnMappings);
            return addColumnManppings;
        }

        private string TableName(Table table)
        {
            var tableName = @"

        public override string TableName
        {{
            get
            {{
                return ""{0}"";
            }}
        }}".Fill(table.FullyQualifiedName);
            return tableName;
        }

        private string AddRecord(Table table, string @namespace)
        {
            var addRecord = @"

        public override void AddRecord(DataTable table, {0} {1})
        {{
            var row = table.NewRow();
{2}

            table.Rows.Add(row);
        }}";
            var columnAddition = new StringBuilder();
            foreach (var column in table.Columns)
            {
                if (column.IsComputed)
                {
                    continue;
                }
                columnAddition.Append("\r\n\t\t\trow[\"{0}\"] = (object){1}.{0} ?? DBNull.Value;".Fill(column.Name, table.SingularName.ToCamelCase()));
            }
            addRecord = addRecord.Fill(GetTableFqn(table, @namespace), table.SingularName.ToCamelCase(), columnAddition.ToString());
            return addRecord;
        }

        private string ConfigureDataTable(Table table)
        {
            var columnConfiguration = new StringBuilder();
            var configureDataTable = @"

        public override DataTable ConfigureDataTable()
        {{
            var table = new DataTable();
{0}

            return table;
        }}";
            columnConfiguration = new StringBuilder();
            foreach (var column in table.Columns)
            {
                if (column.IsComputed)
                {
                    continue;
                }
                columnConfiguration.Append("\r\n\t\t\ttable.Columns.Add(\"{0}\", typeof({1}));".Fill(column.Name, column.DotNetDataType.Replace("?", "")));
            }
            return configureDataTable.Fill(columnConfiguration.ToString());
        }

        private string TempTableCreationScript(Table table)
        {
            var columns = new StringBuilder();
            foreach (var column in table.Columns)
            {
                if (column.IsComputed)
                {
                    continue;
                }

                var dataType = column.SqlDataType;
                if (column.SqlDataType.Contains("char"))
                    dataType += "({0})".Fill(column.MaxLength == -1 ? "MAX" : column.MaxLength.ToString());

                columns.Append("\r\n\t\t\t\t\t\t[{0}] {1} {2},".Fill(column.Name, dataType, column.IsNullable ? "null" : "not null"));
            }
            return tempTableCreationScript.Fill(columns.ToString());
        }

        private string GetTableFqn(Table table, string @namespace)
        {
            var modelNamespace = ModelNamespaceFinder(@namespace).Replace("using", "").Trim();
            if (modelNamespace.IsSomething())
            {
                return modelNamespace + "." + table.SingularName;
            }
            return table.SingularName;
        }

        protected string GeneratedInjectRepositoryServicesCode { get; set; }

        public abstract string UsingStatements { get; }

        public abstract string Namespace { get; }

        public abstract string GetNamespaceBySchema(string schema);

        public virtual string ModelNamespaceFinder(string @repositoryNamespace)
        {
            var modelNamespace = @repositoryNamespace.Replace(".Repositories", ".Models").Replace("namespace ", "using ");
            return modelNamespace;
        }

        public virtual string ContextNamespaceFinder(string repositoryNamespace, string schema)
        {
            var contextNamespace = @repositoryNamespace.Replace(".Repositories", ".DbContexts").Replace("namespace ", "using ");
            if (!CodeGeneratorConfig.CreateDbContextPerModel)
            {
                contextNamespace = contextNamespace.Substring(0, contextNamespace.LastIndexOf("DbContexts") + "DbContexts.".Length - 1);
            }

            return contextNamespace;
        }

        private string GetNamespace(Table table)
        {
            string @namespace = Namespace + (table.Schema.IsSomething() ? "." + table.Schema : "");
            if (table.Schema == "" || !CodeGeneratorConfig.Schemas.Contains(table.Schema.ToLower()))
            {
                @namespace = Namespace + (table.Schema.IsSomething() ? "." + table.Schema : "");
            }
            else
            {
                @namespace = GetNamespaceBySchema(table.Schema);
            }

            @namespace += table.IsView ? ".Views" : "";
            return @namespace;
        }

        private string GetTableName(Table table, string @namespace)
        {
            var modelNamespace = ModelNamespaceFinder(@namespace).Replace("using", "").Trim();
            if (modelNamespace.IsSomething())
            {
                return modelNamespace + "." + table.SingularName;
            }
            return table.SingularName;
        }


    }
}
