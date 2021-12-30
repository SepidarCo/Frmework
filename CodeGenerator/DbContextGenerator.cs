using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public abstract class DbContextGenerator : Generator
    {
        public DbContextGenerator(string connectionString)
            : base(connectionString)
        {
        }

        public override List<Table> Generate()
        {
            var createDbContextPerEntity = CodeGeneratorConfig.CreateDbContextPerModel;
            foreach (var table in Tables)
            {
                if (createDbContextPerEntity)
                {
                    table.GeneratedCode = GenerateDbContextPerEntity(table);
                }
                else
                {
                    table.GeneratedCode = GenerateCodeForConfiguringModel(table);
                }
            }
            return Tables;
        }

        private string GenerateDbContextPerEntity(Table table)
        {
            string classTemplate = @"{0}

{1}
{{
    public class {2}DbContext : DbContext
    {{
        public {2}DbContext()
            : base()
        {{
        }}

        public ICollection<{4}> {5} {{ get; set; }}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {{
            optionsBuilder.UseSqlServer(Config.GetConnectionString(""{3}""));
        }}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{
            modelBuilder.Entity<{4}>().ToTable(""{6}"",""{7}"");{8}
            base.OnModelCreating(modelBuilder);
        }}
    }}
}}
";
            var @class = "";
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

            var usingStatements = UsingStatements;
            usingStatements += "\r\n" + ModelNamespaceFinder(@namespace) + ";"; // todo: change this line to use an abstract ModelNamespaceFinder method
            @class = classTemplate.Fill(usingStatements,
                @namespace,
                table.SingularName,
                ConnectionStringName,
                GetTableName(table, @namespace),
                table.PluralName,
                table.TableName,
                table.Schema.IsNothing() ? "dbo" : table.Schema,
                GetPropertyConfigurations(table, @namespace));
            return @class;
        }

        StringBuilder dbcontextStringBuilder = new StringBuilder();

        protected string GenerateDbContextClass(List<Table> tables)
        {
            tables = tables.OrderBy(i => i.Schema).ThenBy(i => i.SingularName).ToList();
            AppendClassStart();
            foreach (var table in tables)
            {
                var @namespace = GetNamespace(table);

                dbcontextStringBuilder.Append(@"
        public DbSet<{0}> {1}{{ get; set; }}".Fill(GetTableName(table, @namespace), table.PluralName));
                dbcontextStringBuilder.AppendLine();
            }


            dbcontextStringBuilder.Append(@"
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {");

            foreach (var table in tables)
            {
                dbcontextStringBuilder.Append(table.GeneratedCode);
                dbcontextStringBuilder.AppendLine();
            }

            dbcontextStringBuilder.Append(@"
            base.OnModelCreating(modelBuilder);");

            dbcontextStringBuilder.Append(@"
        }
    }
}
");

            return dbcontextStringBuilder.ToString();
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

        private void AppendClassStart()
        {
            dbcontextStringBuilder.Append(@"{0}
{1}
{{
    public class AppDbContext : DbContext
    {{
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {{
        }}
        ".Fill(UsingStatements, Namespace));
        }

        private string GenerateCodeForConfiguringModel(Table table)
        {
            string classTemplate = @"
            modelBuilder.Entity<{0}>().ToTable(""{1}"",""{2}"");{3}";
            var @class = "";
            var @namespace = GetNamespace(table);
            @class = classTemplate.Fill(
                GetTableName(table, @namespace),
                table.TableName,
                table.Schema.IsNothing() ? "dbo" : table.Schema,
                GetPropertyConfigurations(table, @namespace));
            return @class;
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

        private string GetPropertyConfigurations(Table table, string @namespace)
        {
            var computedColumns = table.Columns.Where(i => i.IsComputed).ToList();
            var result = new StringBuilder();
            foreach (var computedColumn in computedColumns)
            {
                result.Append("\r\n\t\t\tmodelBuilder.Entity<{0}>().Property(i => i.{1}).ValueGeneratedOnAddOrUpdate();".Fill(GetTableName(table, @namespace), computedColumn.Name));
            }
            if (table.IsView)
            {
                result.Append("\r\n\t\t\tmodelBuilder.Entity<{0}>().HasKey(i => i.Id);".Fill(GetTableName(table, @namespace)));
            }
            var idColumn = table.Columns.SingleOrDefault(i => i.Name == "Id");
            if (idColumn.IsNotNull() && !idColumn.IsIdentity)
            {
                result.Append("\r\n\t\t\tmodelBuilder.Entity<{0}>().Property(i => i.{1}).ValueGeneratedOnAdd();".Fill(GetTableName(table, @namespace), idColumn.Name));
            }
            return result.ToString();
        }

        public abstract string UsingStatements { get; }

        public abstract string Namespace { get; }

        public abstract string GetNamespaceBySchema(string schema);

        public abstract string ConnectionStringName { get; }

        public virtual string ModelNamespaceFinder(string @contextNamespace)
        {
            var modelNamespace = @contextNamespace.Replace(".DbContexts", ".Models").Replace("namespace ", "using ");
            return modelNamespace;
        }
    }
}