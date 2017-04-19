using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.OleDb;

namespace 心理测评软件.Librarys
{
    public class ExcelOperation
    {
        /// <summary>
        /// 导出简单的Excel文件，groupColumn不为空且是dt的列则分组导出
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="outputFile"></param>
        /// <param name="groupColumn">分组列</param>
        /// <param name="sheetPrefixName">sheet名，分组导出时不起作用</param>
        public static void ExportSimpleExcel(System.Data.DataTable dt, string outputFile)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            var xlApp = new Application { Visible = false, DisplayAlerts = false };
            var workbooks = xlApp.Workbooks;
            var workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            var worksheet = workbook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            worksheet.Name = "sheet";
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
            }
            for (var r = 0; r < dt.Rows.Count; r++)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    //前加单引号设置单元格格式为文本
                    worksheet.Cells[r + 2, i + 1] = "'" + dt.Rows[r][i].ToString();
                }
            }

            try
            {
                workbook.Saved = true;
                workbook.SaveAs(outputFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                try
                {
                    workbook.Close(true, Type.Missing, Type.Missing);
                    xlApp.Workbooks.Close();
                    xlApp.Application.Quit();
                    xlApp.Quit();
                }
                catch
                {
                }
                GC.Collect();
            }
        }
        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="dt"></param>
        public static System.Data.DataTable ImportSimpleExcel(string inputFile)
        {
            string extension = Path.GetExtension(inputFile);
            string strConn = string.Empty;

            if (extension == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + inputFile + ";Extended Properties ='Excel 8.0;HDR=NO;IMEX=1'";
            }
            else if (extension == ".xlsx")
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + inputFile + ";Extended Properties ='Excel 12.0;HDR=NO;IMEX=1'";
            }
            else
            {
                return null;
            }

            var conn = new OleDbConnection(strConn);
            conn.Open();

            var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            var ds = new DataSet();
            foreach (DataRow row in dt.Rows)
            {
                var strExcel = "select * from   [" + row["TABLE_NAME"].ToString() + "]"; ;
                var myCommand = new OleDbDataAdapter(strExcel, strConn);
                myCommand.Fill(ds);
            }
            conn.Close();
            GC.Collect();
            return ds.Tables[0];
        }
    }
}
