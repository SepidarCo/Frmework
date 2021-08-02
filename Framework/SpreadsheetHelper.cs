using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Sepidar.Framework
{
    public class SpreadsheetHelper
    {
        //public static DataTable ReadDataTableFromExcel(Stream stream)
        //{
        //    using (var excel = new ExcelPackage())
        //    {
        //        excel.Load(stream);
        //        var workSheet = excel.Workbook.Worksheets.First();
        //        DataTable table = new DataTable();
        //        bool hasHeader = true;
        //        foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
        //        {
        //            table.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
        //        }
        //        var startRow = hasHeader ? 2 : 1;
        //        for (var rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
        //        {
        //            var workSheetRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column];
        //            var row = table.NewRow();
        //            foreach (var cell in workSheetRow)
        //            {
        //                row[cell.Start.Column - 1] = cell.Text;
        //            }
        //            table.Rows.Add(row);
        //        }
        //        return table;
        //    }
        //}
    }
}
