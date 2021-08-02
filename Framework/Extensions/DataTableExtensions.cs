using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class DataTableExtensions
    {
        public static List<Dictionary<string, object>> ToList(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().ToList();
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dictionary = columns.ToDictionary(c => c.ColumnName, c => row[c.ColumnName]);
                result.Add(dictionary);
            }
            return result;
        }
    }
}
