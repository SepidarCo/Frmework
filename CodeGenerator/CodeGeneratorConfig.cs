using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.CodeGenerator
{
    public class CodeGeneratorConfig : Config
    {
        public static List<string> IncludedTables
        {
            get
            {
                List<string> includedTables = new List<string>();
                if (HasSetting("CodeGenerator:IncludedTables"))
                {
                    includedTables = GetSetting("CodeGenerator:IncludedTables").SplitCsv<string>().Select(i => i.ToLower()).ToList();
                }
                return includedTables;
            }
        }

        public static List<string> ExcludedTables
        {
            get
            {
                List<string> excludedTables = new List<string>();
                if (HasSetting("CodeGenerator:ExcludedTables"))
                {
                    excludedTables = GetSetting("CodeGenerator:ExcludedTables").SplitCsv<string>().Select(i => i.ToLower()).ToList();
                }
                return excludedTables;
            }
        }

        public static List<string> Schemas
        {
            get
            {
                List<string> schemas = new List<string>();
                if (HasSetting("CodeGenerator:Schemas"))
                {
                    schemas = GetSetting("Schemas").SplitCsv<string>().Select(i => i.ToLower()).ToList();
                }
                return schemas;
            }
        }

        public static List<string> ExcludedSchemas
        {
            get
            {
                List<string> excludedSchemas = new List<string>();
                if (HasSetting("CodeGenerator:ExcludedSchemas"))
                {
                    excludedSchemas = GetSetting("ExcludedSchemas").SplitCsv<string>().Select(i => i.ToLower()).ToList();
                }
                return excludedSchemas;
            }
        }

        public static List<string> ExcludedProcedures
        {
            get
            {
                List<string> excludedProcedures = new List<string>();
                if (HasSetting("CodeGenerator:ExcludedProcedures"))
                {
                    excludedProcedures = GetSetting("ExcludedProcedures").SplitCsv<string>();
                }
                return excludedProcedures;
            }
        }

        public static bool ConsiderPluralization
        {
            get
            {
                var key = "CodeGenerator:ConsiderPluralization";
                if (HasSetting(key))
                {
                    return GetSetting(key).ToBoolean();
                }
                return true;
            }
        }

        public static bool CreateDbContextPerModel
        {
            get
            {
                var key = "CodeGenerator:CreateDbContextPerModel";
                if (HasSetting(key))
                {
                    return GetSetting(key).ToBoolean();
                }
                return false;
            }
        }
    }
}