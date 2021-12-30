using Sepidar.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public class StoredProcedureGenerator : Generator
    {
        public StoredProcedureGenerator(string connectionString)
            : base(connectionString)
        {
        }

        public override List<Table> Generate()
        {
            throw new FrameworkException("This is stored procedure generator. Please call GeneratorProcedures method.");
        }

        public List<StoredProcedure> GenerateProcedures()
        {
            foreach (var storedProcedure in StoredProcedures)
            {
                storedProcedure.GeneratedCode = GeneratorFunctionForStoredProcedure(storedProcedure);
            }
            return StoredProcedures;
        }

        private string GeneratorFunctionForStoredProcedure(StoredProcedure storedProcedure)
        {
            //string procedureTemplate = @"exec {0} {1}";
            //var @class = "";
            //var properties = "";
            //foreach (var column in table.Columns)
            //{
            //    properties += GeneratePropertyForColumn(column) + "\r\n" + "\r\n";
            //}
            //properties += "        public dynamic RelatedItems { get; set; }" + "\r\n" + "\r\n";
            //var @namespace = Namespace + (table.Schema.IsSomething() ? "." + table.Schema : "");
            //@namespace += table.IsView ? ".Views" : "";
            //@class = procedureTemplate.Fill(UsingStatements, @namespace, table.SingularName, properties);
            //@class = Regex.Replace(@class, @"(\r\n){2}\r\n", "$1");
            //return procedureTemplate;
            throw new NotImplementedException();
        }
    }
}
