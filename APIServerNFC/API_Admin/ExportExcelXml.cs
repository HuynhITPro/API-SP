using ClosedXML.Excel;

using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace APIServerNFC.API_Admin
{
    public class ExportExcelXml
    {
       
        ClassProcess prs = new ClassProcess();
        public byte[] ExportToExcel(ExcelExportDefine excelExportDefine)
        {
            using (var stream = new MemoryStream())
            {
               
                string sql = prs.Decrypt(excelExportDefine.getDataFromSql.sql);
                // List<SqlParameter> lst = listParameter.lstparameter;
                string json = excelExportDefine.getDataFromSql.json;

                using (SqlConnection sqlConnection = prs.Connect())
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;

                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {

                        if (it.ParameterValue != null)
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                    }
                    sqlCommand.CommandText = sql;
                    DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);//Sửa về chữ đầu tiên trong biến là viết thường cho trùng với kiểu Parse của retrofit2
                    
                    if (excelExportDefine.lstheader != null)
                    {
                        bool check = false;
                        foreach (var it in excelExportDefine.lstheader)
                        {
                           
                            if (dt.Columns.Contains(it.HeaderName))
                            {
                                dt.Columns[it.HeaderName].Caption = it.HeaderCaption;
                            }
                            
                        }

                    }
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    if(excelExportDefine.RowBegin<3)
                        excelExportDefine.RowBegin=3;
                    int rowEnd = excelExportDefine.RowBegin + dt.Rows.Count;
                    //int rowEnd = rowStart + dt.Columns.Count - 1;
                    var workbook = new XLWorkbook();
                    int columnEnd = excelExportDefine.ColumnBegin + dt.Columns.Count - 1;
                    var worksheet = workbook.AddWorksheet("Export");

                    worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;


                    List<string> lst_rowgroup = new List<string>();
                    int countrowrangebegin = excelExportDefine.RowBegin + 1;//Vùng dữ liệu bắt đầu
                    int countrowrangeend = excelExportDefine.RowBegin;
                    object[,] arr = new object[dt.Rows.Count + 1, dt.Columns.Count];//Tính luôn dòng Header
                                                                                    //Chuyển dữ liệu từ DataTable vào mảng đối tượng
                                                                                    //MessageBox.Show("test");
                    //for (int c = 0; c < dt.Columns.Count; c++)
                    //{

                    //    //arr[0, c] = dt.Columns[c].ColumnName;
                    //    var rowtieude = worksheet.Row(excelExportDefine.RowBegin);
                    //    rowtieude.Cell(excelExportDefine.ColumnBegin + c).Value = dt.Columns[c].ColumnName;

                    //}
                    int rowbegindata = excelExportDefine.RowBegin + 1;

                    //double d = 0;


                    worksheet.Cell(rowbegindata, excelExportDefine.ColumnBegin).InsertTable(dt, false); // false = không tạo tên bảng Excel

                   
                    var rangeborder = worksheet.RangeUsed();
                    rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    IXLCell xLCelltittle = worksheet.Row(excelExportDefine.RowBegin - 1).Cell(excelExportDefine.ColumnBegin);
                    xLCelltittle.Value = string.Format("{0}", excelExportDefine.GhiChu);
                    xLCelltittle.Style.Font.Bold = true;
                    xLCelltittle.Style.Font.FontSize = 20;
        
                    IXLCell c1 = worksheet.Row(excelExportDefine.RowBegin).Cell(excelExportDefine.ColumnBegin);
                    IXLCell c2 = worksheet.Row(excelExportDefine.RowBegin).Cell(excelExportDefine.ColumnBegin);

                    IXLRange range = worksheet.Range(excelExportDefine.RowBegin, excelExportDefine.ColumnBegin, rowbegindata + dt.Rows.Count, columnEnd);

                    //range.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;
                    foreach (var column in worksheet.ColumnsUsed())
                    {
                        column.AdjustToContents();
                    }
                    IXLRow row0 = worksheet.Row(excelExportDefine.RowBegin);
                    row0.Style.Font.Bold = true;

                    workbook.SaveAs(stream);
                    return stream.ToArray();


                }


            }
        }
    }
}
