using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;


namespace Sepidar.Sql
{
    public class HelperFunctions
    {
       // [SqlFunction]
        public static SqlString OrderCharacters(SqlString text)
        {
            if (text.IsNull)
            {
                return "";
            }
            var result = string.Join("", text.Value.ToCharArray().OrderBy(i => i).ToArray());
            return result;
        }

       // [SqlFunction]
        public static void ClearDatabase(string dataFolder, string connectionString)
        {
            var staticData = GetStaticData(dataFolder);
            TruncateAllTablesAndInsertStaticData(staticData, connectionString);
        }

       // [SqlFunction(TableDefinition = "[Character] nvarchar(100)", FillRowMethodName = "GetCharactersFillRow")]
        public static IEnumerable GetCharacters(SqlString text)
        {
            if (text.IsNull)
            {
                return new List<SqlString>();
            }
            var result = text.Value.ToCharArray().Select(i => i.ToString()).ToList();
            return result;
        }

        private static void GetCharactersFillRow(Object @object, out SqlString result)
        {
            string value = (string)@object;
            result = new SqlString(value);
        }

        private static string GetStaticData(string dataFolder)
        {
            if (string.IsNullOrWhiteSpace(dataFolder))
            {
                return "";
            }
            dataFolder = Environment.ExpandEnvironmentVariables(dataFolder);
            if (!Directory.Exists(dataFolder))
            {
                return "";
            }
            var dataFiles = Directory.GetFiles(Environment.ExpandEnvironmentVariables(dataFolder));
            var query = new StringBuilder();
            foreach (var dataFile in dataFiles)
            {
                query.Append(File.ReadAllText(dataFile) + "\r\n");
            }
            return query.ToString();
        }

        private static void TruncateAllTablesAndInsertStaticData(string staticData, string connectionString)
        {
            var query = string.Format(@"
                    /* TRUNCATE ALL TABLES IN A DATABASE */
                    DECLARE @dropAndCreateConstraintsTable TABLE
                            (
                                DropStmt VARCHAR(MAX)
                            ,CreateStmt VARCHAR(MAX)
                            )
                    /* Gather information to drop and then recreate the current foreign key constraints  */
                    INSERT  @dropAndCreateConstraintsTable
                            SELECT  DropStmt = 'ALTER TABLE [' + ForeignKeys.ForeignTableSchema
                                    + '].[' + ForeignKeys.ForeignTableName + '] DROP CONSTRAINT ['
                                    + ForeignKeys.ForeignKeyName + ']; '
                                    ,CreateStmt = 'ALTER TABLE [' + ForeignKeys.ForeignTableSchema
                                    + '].[' + ForeignKeys.ForeignTableName
                                    + '] WITH CHECK ADD CONSTRAINT [' + ForeignKeys.ForeignKeyName
                                    + '] FOREIGN KEY([' + ForeignKeys.ForeignTableColumn
                                    + ']) REFERENCES [' + SCHEMA_NAME(sys.objects.schema_id)
                                    + '].[' + sys.objects.[name] + ']([' + sys.columns.[name]
                                    + ']) on update ' + ForeignKeys.OnUpdate + ' on delete ' + ForeignKeys.OnDelete
				                    + ' ALTER TABLE [' + ForeignKeys.ForeignTableSchema
                                    + '].[' + ForeignKeys.ForeignTableName
                                    + '] CHECK CONSTRAINT [' + ForeignKeys.ForeignKeyName
                                    + ']'
                            FROM    sys.objects
                            INNER JOIN sys.columns
                                    ON ( sys.columns.[object_id] = sys.objects.[object_id] )
                            INNER JOIN ( SELECT sys.foreign_keys.[name] AS ForeignKeyName
                                                ,SCHEMA_NAME(sys.objects.schema_id) AS ForeignTableSchema
                                                ,sys.objects.[name] AS ForeignTableName
                                                ,sys.columns.[name] AS ForeignTableColumn
                                                ,sys.foreign_keys.referenced_object_id AS referenced_object_id
                                                ,sys.foreign_key_columns.referenced_column_id AS referenced_column_id
						                        ,replace(sys.foreign_keys.update_referential_action_desc, '_', ' ') as OnUpdate
						                        ,replace(sys.foreign_keys.delete_referential_action_desc, '_', ' ') as OnDelete
                                            FROM   sys.foreign_keys
                                            INNER JOIN sys.foreign_key_columns
                                                ON ( sys.foreign_key_columns.constraint_object_id = sys.foreign_keys.[object_id] )
                                            INNER JOIN sys.objects
                                                ON ( sys.objects.[object_id] = sys.foreign_keys.parent_object_id )
                                            INNER JOIN sys.columns
                                                ON ( sys.columns.[object_id] = sys.objects.[object_id] )
                                                    AND ( sys.columns.column_id = sys.foreign_key_columns.parent_column_id )
                                        ) ForeignKeys
                                    ON ( ForeignKeys.referenced_object_id = sys.objects.[object_id] )
                                        AND ( ForeignKeys.referenced_column_id = sys.columns.column_id )
                            WHERE   ( sys.objects.[type] = 'U' )
                                    AND ( sys.objects.[name] NOT IN ( 'sysdiagrams' ) )
                    /* SELECT * FROM @dropAndCreateConstraintsTable AS DACCT  --Test statement*/
                    DECLARE @DropStatement NVARCHAR(MAX)
                    DECLARE @RecreateStatement NVARCHAR(MAX)
                    /* Drop Constraints */
                    DECLARE Cur1 CURSOR READ_ONLY
                    FOR
                            SELECT  DropStmt
                            FROM    @dropAndCreateConstraintsTable
                    OPEN Cur1
                    FETCH NEXT FROM Cur1 INTO @DropStatement
                    WHILE @@FETCH_STATUS = 0
                            BEGIN
                                PRINT 'Executing ' + @DropStatement
                                EXECUTE sp_executesql @DropStatement
                                FETCH NEXT FROM Cur1 INTO @DropStatement
                            END
                    CLOSE Cur1
                    DEALLOCATE Cur1
                    /* Truncate all tables in the database in  */
                    DECLARE @DeleteTableStatement NVARCHAR(MAX)
                    DECLARE Cur2 CURSOR READ_ONLY
                    FOR
                            SELECT  'TRUNCATE TABLE ' + quotename(object_schema_name([object_id])) + '.' + quotename(object_name([object_id]))
                            FROM    sys.tables
                            WHERE   [type] = 'U'
                    OPEN Cur2
                    FETCH NEXT FROM Cur2 INTO @DeleteTableStatement
                    WHILE @@FETCH_STATUS = 0
                            BEGIN
                                PRINT 'Executing ' + @DeleteTableStatement
                                EXECUTE sp_executesql @DeleteTableStatement
                                FETCH NEXT FROM Cur2 INTO @DeleteTableStatement
                            END
                    CLOSE Cur2
                    DEALLOCATE Cur2
                    /* insert static data */
                    {0}
                    /* Recreate foreign key constraints  */
                    DECLARE Cur3 CURSOR READ_ONLY
                    FOR
                            SELECT  CreateStmt
                            FROM    @dropAndCreateConstraintsTable
                    OPEN Cur3
                    FETCH NEXT FROM Cur3 INTO @RecreateStatement
                    WHILE @@FETCH_STATUS = 0
                            BEGIN
                                PRINT 'Executing ' + @RecreateStatement
                                EXECUTE sp_executesql @RecreateStatement
                                FETCH NEXT FROM Cur3 INTO @RecreateStatement
                            END
                    CLOSE Cur3
                    DEALLOCATE Cur3
                    ", staticData);
            Database.Open(connectionString).Run(query);
        }

       // [SqlFunction(TableDefinition = "[Character] nvarchar(max)", FillRowMethodName = "GetSplitFillRow")]
        public static IEnumerable SplitCsv(SqlString text)
        {
            if (text.IsNull)
            {
                return new List<SqlString>();
            }
            var tokens = text.Value.Split(',').Select(i => i.Trim()).Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            return tokens;
        }

        private static void GetSplitFillRow(Object @object, out SqlString result)
        {
            string value = (string)@object;
            result = new SqlString(value);
        }
    }
}
