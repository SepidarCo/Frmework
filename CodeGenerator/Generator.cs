using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.CodeGenerator
{
    public abstract class Generator
    {
        public Generator(string connectionString)
        {
            InstantiateDatabase(connectionString);
            FetchTables();
            FetchColumns();
            FetchUniqueKeys();
            FetchStoredProcedure();
            FetchParameters();
        }

        private void PopulateUniqueKeys(Table table)
        {
            var uniqueKeysQuery = @"
                        select object_schema_name(i.[object_id]) as [Schema],
                         object_name(i.[object_id]) as [Table],
                         i.name as [Index],
                         stuff((
                          select '_And_' + c.name
                          from sys.index_columns ic
                          inner join sys.columns c
                          on ic.[object_id] = c.[object_id]
                          and ic.column_id = c.column_id
                          where ic.index_id = i.index_id
                          and ic.[object_id] = i.[object_id]
                          order by c.name
                          for xml path('')
                         ), 1, 5, '') as Columns
                        from sys.indexes i 
                        where is_unique = 1
                        and [type] = 2
                        and object_name(i.[object_id]) not in ('sysdiagrams')
                        and i.[object_id] in 
                        (
                         select [object_id]
                         from sys.tables 
                         where [type] = 'U'
                        )
                        and i.name like N'%Unique%'
                        and i.[object_id] = {0}
                        order by [Schema], [Table], [Index]
                        ".Fill(table.ObjectId);
            var uniqueKeys = Sepidar.Sql.Database
                .Open(Database.ConnectionString)
                .Get(uniqueKeysQuery);
            if (uniqueKeys.Rows.Count == 0)
            {
                table.UniqueKeys = new List<UniqueKey>();
                return;
            }

            var keys = new List<UniqueKey>();
            foreach (DataRow row in uniqueKeys.Rows)
            {
                var uniqeKey = new UniqueKey
                {
                    Name = row["index"].ToString(),
                    Columns = GetUniqueKeyColumns(table, row["columns"].ToString()),
                    Table = table
                };
                keys.Add(uniqeKey);
            }
            table.UniqueKeys = keys.ToList();
        }

        private List<Column> GetUniqueKeyColumns(Table table, string delimitedColumns)
        {
            var columns = new List<Column>();
            var columnNames = delimitedColumns.Split(new string[] { "_And_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var columnName in columnNames)
            {
                columns.Add(table.Columns.Single(i => i.Name == columnName));
            }
            return columns;
        }

        private void PopulateColumns(Table table)
        {
            var columnsQuery = @"
                        select type_name([system_type_id]) as [type], *
                        from sys.columns 
                        where [object_id] = {0}
                        ".Fill(table.ObjectId);
            var dataTable = Sql.Database
                .Open(Database.ConnectionString)
                .Get(columnsQuery);

            var columns = new List<Column>();
            foreach (DataRow row in dataTable.Rows)
            {
                var column = new Column
                {
                    Name = row["name"].ToString(),
                    SqlDataType = row["type"].ToString(),
                    DotNetDataType = MapSqlDataTypeToDotNetDataType(row["type"].ToString()),
                    IsNullable = row["is_nullable"].ToBoolean(),
                    IsComputed = row["is_computed"].ToBoolean(),
                    IsIdentity = row["is_identity"].ToBoolean(),
                    MaxLength = Convert.ToInt32(row["max_length"])
                };
                columns.Add(column);
            }

            table.Columns = columns.ToList();
            var nonNullableColumns = new List<string> { "string", "byte[]" };
            foreach (var column in table.Columns)
            {
                if (column.IsNullable && !nonNullableColumns.Contains(column.DotNetDataType))
                {
                    column.DotNetDataType += "?";
                }
                if (column.SqlDataType == "nvarchar" && column.MaxLength != -1)
                {
                    column.MaxLength = column.MaxLength / 2;
                }
            }

        }

        private string MapSqlDataTypeToDotNetDataType(string sqlDataType)
        {
            switch (sqlDataType)
            {
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "char":
                case "text":
                case "ntext":
                    return "string";
                case "decimal":
                case "numeric":
                case "float":
                    return "decimal";
                case "bigint":
                    return "long";
                case "int":
                    return "int";
                case "smallint":
                    return "Int16";
                case "tinyint":
                    return "byte";
                case "bit":
                    return "bool";
                case "varbinary":
                    return "byte[]";
                case "datetime":
                case "date":
                    return "DateTime";
                case "time":
                    return "TimeSpan";
                case "uniqueidentifier":
                    return "Guid";
                case "timestamp":
                    return "byte[]";
                default:
                    throw new FrameworkException("SQL data type '{0}' is not mapped to .NET data type".Fill(sqlDataType));
            }
        }

        private void FetchTables()
        {
            const string query = @"
                            select name, [object_id], schema_name([schema_id]) as [schema], 0 as IsView
                            from sys.tables 
                            where name not in ('sysdiagrams')
						
						    union
						
                            select name, [object_id], schema_name([schema_id]) as [schema], 1 as IsView
                            from sys.views 
                            where name not in ('sysdiagrams');
                            ";
            var dataTable = Sql.Database
                .Open(Database.ConnectionString)
                .Get(query);

            var tables = new List<Table>();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                var table = new Table();
                //{
                table.TableName = row["name"].ToString();
                table.ObjectId = Convert.ToInt32(row["object_id"]);
                table.Schema = row["schema"].ToString().ToPascalCase();
                table.IsView = Convert.ToBoolean(row["IsView"]);
                //};
                tables.Add(table);
            }

            foreach (var table in tables)
            {
                table.Schema = Regex.Replace(table.Schema, @"dbo", "", RegexOptions.IgnoreCase);
            }
            if (CodeGeneratorConfig.IncludedTables.Count > 0)
            {
                tables = tables.Where(i => CodeGeneratorConfig.IncludedTables.Contains(i.TableName.ToLower()) || CodeGeneratorConfig.IncludedTables.Contains(i.FullyQualifiedName.ToLower())).ToList();
                Tables = tables;
            }
            else
            {
                tables = tables.Where(i => !CodeGeneratorConfig.ExcludedTables.Contains(i.TableName.ToLower()) && !CodeGeneratorConfig.ExcludedTables.Contains(i.FullyQualifiedName.ToLower())).ToList();
                tables = tables.Where(i => !CodeGeneratorConfig.ExcludedSchemas.Contains(i.Schema.ToLower())).ToList();
                Tables = tables;
            }

        }

        private void FetchColumns()
        {
            foreach (var table in Tables)
            {
                PopulateColumns(table);
            }
        }

        private void FetchUniqueKeys()
        {
            foreach (var table in Tables)
            {
                PopulateUniqueKeys(table);
            }
        }

        private void FetchStoredProcedure()
        {
            var query = @"
                        select 
	                        sys.procedures.[object_id] as ObjectId,
	                        object_schema_name(sys.procedures.[object_id]) as [Schema],
	                        sys.procedures.name as ProcedureName,
	                        dbo.getFqn(sys.procedures.[object_id]) as ProcedureFullyQualifiedName, 
	                        sys.parameters.name as ParameterName, 
	                        type_name(system_type_id) as ParameterType, 
	                        is_output as IsOutput, 
	                        sys.parameters.has_default_value as HasDefaultValue, 
	                        default_value as DefaultValue
                        from sys.procedures 
                        inner join sys.parameters 
                        on sys.procedures.[object_id] = sys.parameters.[object_id]
                        order by sys.procedures.name, sys.parameters.name
                        ";
            var storedProcedures = Sql.Database.Open(Database.ConnectionString).Get(query);

            var storedProcedureList = new List<StoredProcedure>();

            for (int i = 0; i < storedProcedures.Rows.Count; i++)
            {
                var row = storedProcedures.Rows[i];

                var storedProcedure = new StoredProcedure
                {
                    Name = row["ProcedureName"].ToString(),
                    ObjectId = Convert.ToInt32(row["ObjectId"]),
                    Schema = row["Schema"].ToString().ToPascalCase()
                };
                storedProcedureList.Add(storedProcedure);
            }


            foreach (var storedProcedure in storedProcedureList)
            {
                storedProcedure.Schema = Regex.Replace(storedProcedure.Schema, @"dbo", "", RegexOptions.IgnoreCase);
            }
            storedProcedureList = storedProcedureList.Where(i => !CodeGeneratorConfig.ExcludedProcedures.Contains(i.Name) && !CodeGeneratorConfig.ExcludedProcedures.Contains(i.FullyQualifiedName)).ToList();
            storedProcedureList = storedProcedureList.Where(i => !CodeGeneratorConfig.ExcludedSchemas.Contains(i.Schema)).ToList();
            StoredProcedures = storedProcedureList;
        }

        private void FetchParameters()
        {
            foreach (var storedProcedure in StoredProcedures)
            {
                FetchParameters(storedProcedure);
            }
        }

        private void FetchParameters(StoredProcedure storedProcedure)
        {
            var parametersQuery = @"
                                select 
	                                sys.parameters.name as ParameterName, 
	                                type_name(system_type_id) as ParameterType, 
	                                is_output as IsOutput, 
	                                sys.parameters.has_default_value as HasDefaultValue, 
	                                default_value as DefaultValue,
	                                max_length as [MaxLength],
	                                is_nullable as IsNullable
                                from sys.parameters
                                where [object_id] = {0}
                                order by sys.parameters.name
                        ".Fill(storedProcedure.ObjectId);
            var Parameters = Sql.Database.Open(Database.ConnectionString).Get(parametersQuery);

            var parameterList = new List<Parameter>();

            for (int i = 0; i < Parameters.Rows.Count; i++)
            {
                var row = Parameters.Rows[i];
                var parameter = new Parameter
                {
                    Name = row["ParameterName"].ToString(),
                    SqlDataType = row["ParameterType"].ToString(),
                    DotNetDataType = MapSqlDataTypeToDotNetDataType(row["ParameterType"].ToString()),
                    MaxLength = Convert.ToInt32(row["MaxLength"]),
                    IsNullable = row["IsNullable"].ToBoolean()
                };
                parameterList.Add(parameter);
            }

            storedProcedure.Parameters = parameterList.ToList();

            var nonNullableParameters = new List<string> { "string", "byte[]" };
            foreach (var parameter in storedProcedure.Parameters)
            {
                if (parameter.IsNullable && !nonNullableParameters.Contains(parameter.DotNetDataType))
                {
                    parameter.DotNetDataType += "?";
                }
                if (parameter.SqlDataType == "nvarchar" && parameter.MaxLength != -1)
                {
                    parameter.MaxLength = parameter.MaxLength / 2;
                }
            }
        }

        private void InstantiateDatabase(string connectionString)
        {
            Database = new Database
            {
                ConnectionString = connectionString,
                Name = new SqlConnectionStringBuilder(connectionString).InitialCatalog
            };
        }

        public Database Database { get; private set; }

        public List<Table> Tables { get; set; }

        public List<StoredProcedure> StoredProcedures { get; set; }

        public abstract List<Table> Generate();

        protected string PrepareOutputFolder(string path)
        {
            var outputFolder = Environment.ExpandEnvironmentVariables(path);
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
                Directory.CreateDirectory(outputFolder);
            }
            return outputFolder;
        }
    }
}
