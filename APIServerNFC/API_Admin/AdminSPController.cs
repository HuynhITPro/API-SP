using APIServerNFC.Api_Report;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APIServerNFC.API_Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSPController : ControllerBase
    {
        ClassProcess prs = new ClassProcess();
        private readonly IHubContext<ChatHub> _hubContext;//ChatHub dùng để gửi tin nhắn

        public AdminSPController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("ExcuteQuery")]
        public async Task<IActionResult> JsonFromSql([FromBody] GetDataFromSql lstParameter)
        {
            string sql = lstParameter.sql;
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            try
            {
                await using (SqlConnection sqlConnection = prs.ConnectSP())
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

                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    string jsonoutput = JsonConvert.SerializeObject(dt);
                    if (lstParameter.topic != null && lstParameter.id != null)//Có gửi message để in
                    {
                        _ = SendMsgPrintAsync(lstParameter.id, lstParameter.topic, dt);

                    }
                    dt.Dispose();
                    return Content(jsonoutput);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
            //object ob = new JsonResult(lstsp);
        }

        [HttpPost]
        [Route("ExcuteQueryEncrypt")]
        public async Task<IActionResult> JsonFromSqlEncrypt([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            try
            {
                await using (SqlConnection sqlConnection = prs.ConnectSP())
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

                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    string jsonoutput = JsonConvert.SerializeObject(dt);
                    if (lstParameter.topic != null && lstParameter.id != null)//Có gửi message để in
                    {
                        _ = SendMsgPrintAsync(lstParameter.id, lstParameter.topic, dt);

                    }
                    dt.Dispose();
                    return Content(jsonoutput);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
            //object ob = new JsonResult(lstsp);
        }

        [HttpPost]
        [Route("JsonFromSqlProcedureWithException")]
        public async Task<IActionResult> JsonFromSqlProcedureWithException([FromBody] GetDataFromSql lstParameter)
        {
            string sql = lstParameter.sql;
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            string jsonoutput = "";
            await using (SqlConnection sqlConnection = prs.ConnectSP())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                        {
                            if (it.Type == "DataTable")
                            {

                                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(it.ParameterValue.ToString());
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, dttmp));
                                dttmp.Clear();

                            }
                            else
                            {
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                            }
                        }
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                        //sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                    }
                    sqlCommand.CommandText = sql;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    jsonoutput = JsonConvert.SerializeObject(dt);
                    if (lstParameter.topic != null && lstParameter.id != null)//Có gửi message để in
                    {
                        _ = SendMsgPrintAsync(lstParameter.id, lstParameter.topic, dt);

                    }

                    dt.Dispose();

                }
                catch (Exception ex)
                {
                    DataTable dtex = new DataTable();
                    dtex.Columns.Add("ketquaexception", typeof(string));
                    DataRow dataRow = dtex.NewRow();
                    dataRow["ketquaexception"] = ex.Message;
                    dtex.Rows.Add(dataRow);
                    jsonoutput = JsonConvert.SerializeObject(dtex);
                }
                //object ob = new JsonResult(lstsp);
                return Content(jsonoutput);

            }
        }

        [HttpPost]
        [Route("JsonFromSqlProcedureWithExceptionEncrypt")]
        public async Task<IActionResult> JsonFromSqlProcedureWithExceptionEncrypt([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            //string sql = lstParameter.sql;
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            string jsonoutput = "";
            await using (SqlConnection sqlConnection = prs.ConnectSP())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                        {
                            if (it.Type == "DataTable")
                            {

                                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(it.ParameterValue.ToString());
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, dttmp));
                                //dttmp.Clear();

                            }
                            else
                            {
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                            }
                        }
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                        //sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                    }
                    sqlCommand.CommandText = sql;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    jsonoutput = JsonConvert.SerializeObject(dt);
                    if (lstParameter.topic != null && lstParameter.id != null)//Có gửi message để in
                    {
                        _ = SendMsgPrintAsync(lstParameter.id, lstParameter.topic, dt);

                    }

                    dt.Dispose();

                }
                catch (Exception ex)
                {
                    DataTable dtex = new DataTable();
                    dtex.Columns.Add("ketquaexception", typeof(string));
                    DataRow dataRow = dtex.NewRow();
                    dataRow["ketquaexception"] = ex.Message;
                    dtex.Rows.Add(dataRow);
                    jsonoutput = JsonConvert.SerializeObject(dtex);
                }
                //object ob = new JsonResult(lstsp);
                return Content(jsonoutput);

            }
        }



        [HttpPost]
        [Route("ExcuteQueryDatasetEncrypt")]
        public async Task<IActionResult> JsonFromSqlDatasetEncrypt([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            try
            {
                await using (SqlConnection sqlConnection = prs.ConnectSP())
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;

                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);
                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                        {
                            if (it.Type == "DataTable")
                            {

                                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(it.ParameterValue.ToString());
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, dttmp));
                                //dttmp.Clear();

                            }
                            else
                            {
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                            }
                        }
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                        //sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                    }
                    //foreach (var it in lstpara)
                    //{

                    //    if (it.ParameterValue != null)
                    //        sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                    //    else
                    //        sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                    //}
                    sqlCommand.CommandText = sql;

                    SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    string jsonoutput = JsonConvert.SerializeObject(ds);
                    if (lstParameter.topic != null && lstParameter.id != null)//Có gửi message để in
                    {
                        _ = SendMsgPrintDatasetAsync(lstParameter.id, lstParameter.topic, ds);

                    }
                    da.Dispose();
                    return Content(jsonoutput);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
            //object ob = new JsonResult(lstsp);
        }

        [HttpPost]
        [Route("JsonFromSqlProcedureDatasetEncrypt")]
        public async Task<IActionResult> JsonFromSqlProcedureDatasetEncrypt([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            //string sql = lstParameter.sql;
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            string jsonoutput = "";
            await using (SqlConnection sqlConnection = prs.ConnectSP())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                        {
                            if (it.Type == "DataTable")
                            {

                                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(it.ParameterValue.ToString());
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, dttmp));
                                //dttmp.Clear();

                            }
                            else
                            {
                                sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                            }
                        }
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));

                        //sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                    }
                    sqlCommand.CommandText = sql;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();


                    jsonoutput = JsonConvert.SerializeObject(ds);


                    ds.Dispose();

                }
                catch (Exception ex)
                {
                    DataTable dtex = new DataTable();
                    dtex.Columns.Add("ketquaexception", typeof(string));
                    DataRow dataRow = dtex.NewRow();
                    dataRow["ketquaexception"] = ex.Message;
                    dtex.Rows.Add(dataRow);
                    jsonoutput = JsonConvert.SerializeObject(dtex);
                }
                //object ob = new JsonResult(lstsp);
                return Content(jsonoutput);

            }
        }


        [HttpPost]
        [Route("ExcuteProcedure")]
        public async Task<IActionResult> ExcuteProcedure(GetDataFromSql lstParameter)
        {
            string sql = lstParameter.sql;
            string json = lstParameter.json;
            try
            {
                await using (SqlConnection sqlConnection = prs.ConnectSP())
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
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    //object ob = new JsonResult(lstsp);
                    return new JsonResult("OK");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
        }

        [HttpPost]
        [Route("JsonFromSqlProcedure")]
        public async Task<IActionResult> JsonFromSqlProcedure([FromBody] GetDataFromSql lstParameter)
        {
            string sql = lstParameter.sql;
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            string jsonoutput = "";
            await using (SqlConnection sqlConnection = prs.ConnectSP())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));


                    }
                    sqlCommand.CommandText = sql;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    jsonoutput = JsonConvert.SerializeObject(dt);
                    dt.Dispose();
                }
                catch (Exception ex)
                {
                    return new JsonResult("Lỗi: " + ex.Message); //string s = ex.Message;
                }
                //object ob = new JsonResult(lstsp);
                return Content(jsonoutput);

            }
        }

        [Route("Upload")]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(pathBuilt, formFile.FileName);// Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

        [Route("UploadFileImg")]
        [HttpPost]
        public async Task<IActionResult> UploadProfileAndName([FromForm] FileViewModel fileviewmodel)
        {
            try
            {
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic", "NFC/Document/ImgFile");
                DateTime now = DateTime.Now;
                Model.FileHoSo fileHoSo = JsonConvert.DeserializeObject<Model.FileHoSo>(fileviewmodel.name);
                using (Model.NFCDBContext nFCDBContext = new Model.NFCDBContext())
                {
                    string pathngay = Path.Combine(pathBuilt, now.ToString("yyyy.MM.dd"));
                    if (!Directory.Exists(pathngay))
                    {
                        Directory.CreateDirectory(pathngay);
                    }
                    string pathitem = Path.Combine(pathngay, fileHoSo.SerialLink.Value.ToString());
                    if (!Directory.Exists(pathitem))
                    {
                        Directory.CreateDirectory(pathitem);
                    }

                    string pathsave = "Document/ImgFile/" + now.ToString("yyyy.MM.dd") + "/" + fileHoSo.SerialLink.Value.ToString();

                    fileHoSo.UrlFile = pathsave + '/' + fileviewmodel.file.FileName;
                    var filePath = Path.Combine(pathitem, fileviewmodel.file.FileName);// Path.GetTempFileName();
                    fileHoSo.Dvt = "MB";

                    var stream = new FileStream(filePath, FileMode.Create);
                    await fileviewmodel.file.CopyToAsync(stream);
                    nFCDBContext.FileHoSo.Add(fileHoSo);
                    nFCDBContext.SaveChanges();

                    // Process uploaded files
                    // Don't rely on or trust the FileName property without validation.
                    return new JsonResult("done");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("Lỗi lưu ảnh " + ex.Message);
            }
        }




        [Route("DeleteFileHoSo")]
        [HttpPost]
        public async Task<IActionResult> DeleteFileHoSo([FromBody] string filehoso)
        {
            try
            {


                //var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic", "NFC/Document/ImgFile");
                DateTime now = DateTime.Now;
                Model.FileHoSo fileHoSo = JsonConvert.DeserializeObject<Model.FileHoSo>(filehoso);
                SqlConnection sqlConnection = prs.Connect();
                sqlConnection.Open();
                string ketqua = "lỗi xóa ảnh";
                ketqua = StaticClassMethod.DeleteFile(fileHoSo, sqlConnection);

                sqlConnection.Close();


                // Process uploaded files
                // Don't rely on or trust the FileName property without validation.
                return new JsonResult(ketqua);

            }
            catch (Exception ex)
            {
                return new JsonResult("Lỗi xóa ảnh " + ex.Message);
            }
        }

        [Route("DeleteFileHoSoEncrypt")]
        [HttpPost]
        public async Task<IActionResult> DeleteFileHoSoEncrypt([FromBody] GetDataFromSql lstParameter)
        {
            try
            {
                string json = prs.Decrypt(lstParameter.sql);

                //var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic", "NFC/Document/ImgFile");
                DateTime now = DateTime.Now;
                Model.FileHoSo fileHoSo = JsonConvert.DeserializeObject<Model.FileHoSo>(json);
                SqlConnection sqlConnection = prs.Connect();
                sqlConnection.Open();
                string ketqua = "lỗi xóa file";
                ketqua = StaticClassMethod.DeleteFile(fileHoSo, sqlConnection);

                sqlConnection.Close();


                // Process uploaded files
                // Don't rely on or trust the FileName property without validation.
                return new JsonResult(ketqua);

            }
            catch (Exception ex)
            {
                return new JsonResult("Lỗi xóa ảnh " + ex.Message);
            }
        }

        [HttpPost("Uploadfilewithgroup")]
        public async Task<IActionResult> UploadFileWithGroup(IFormFile file, [FromForm] string foldername, [FromForm] string filehosojson)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new JsonResult("No file uploaded");
                }
                var pathforlder = Path.Combine("SupplierPublic/document/", foldername);
                if (!Directory.Exists(pathforlder))
                {
                    Directory.CreateDirectory(pathforlder);
                }
                Model.FileHoSoAPI fileHoSo = JsonConvert.DeserializeObject<Model.FileHoSoAPI>(filehosojson);
                //Tạo tên file riêng để tránh bị trùng
                if (fileHoSo == null)
                {
                    return new JsonResult("Class File API bị NULL ");
                }
                if (fileHoSo.TableNameRoot == null || fileHoSo.NoiDungRoot == null || fileHoSo.SerialRoot == null)
                {
                    return new JsonResult("TableNameRoot hoặc  NoiDungRoot  hoặc SerialRoot không được NULL");
                }
                string filenamehost = prs.RandomString(20) + Path.GetExtension(file.FileName);
                fileHoSo.UrlFile = "document/" + foldername + "/" + filenamehost;

                string filepath = fileHoSo.UrlFile;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic/" + fileHoSo.UrlFile);

                SqlConnection sqlConnection = prs.Connect();
                sqlConnection.Open();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                SqlCommand sqlCommand = new SqlCommand("FileHoSoAPI_Insert", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@TableName", fileHoSo.TableName);
                sqlCommand.Parameters.AddWithValue("@TenFile", fileHoSo.TenFile);
                sqlCommand.Parameters.AddWithValue("@UrlFile", fileHoSo.UrlFile);
                sqlCommand.Parameters.AddWithValue("@DienGiai", fileHoSo.DienGiai);
                sqlCommand.Parameters.AddWithValue("@DungLuong", (fileHoSo.DungLuong == null) ? 0 : Math.Round(fileHoSo.DungLuong.Value, 4));
                sqlCommand.Parameters.AddWithValue("@DVT", "MB");
                sqlCommand.Parameters.AddWithValue("@UserInsert", fileHoSo.UserInsert);
                sqlCommand.Parameters.AddWithValue("@SerialLink", fileHoSo.SerialLink);

                sqlCommand.Parameters.AddWithValue("@SerialRoot", fileHoSo.SerialRoot);
                sqlCommand.Parameters.AddWithValue("@TableNameRoot", fileHoSo.TableNameRoot);
                sqlCommand.Parameters.AddWithValue("@NoiDungRoot", fileHoSo.NoiDungRoot);

                sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
                //StaticClassMethod.SaveFileHoSo(fileHoSo, sqlConnection);
                sqlConnection.Close();
                return new JsonResult("done");
            }
            catch (Exception ex)
            {
                return new JsonResult("Lỗi lưu file " + ex.Message);
            }
        }

        [HttpPost("GetBase64Image")]
        public async Task<IActionResult> GetBase64Image(string pathimg)
        {
            // Đường dẫn tới ảnh (ví dụ: wwwroot/images/checkicon.png)
            // var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/checkicon.png");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic/" + pathimg);
            if (!System.IO.File.Exists(pathimg))
            {
                return new JsonResult("Image not found.");
            }

            // Đọc ảnh và chuyển đổi sang base64
            byte[] imageBytes = System.IO.File.ReadAllBytes(pathimg);
            string base64String = Convert.ToBase64String(imageBytes);

            return new JsonResult(base64String);
        }




        [HttpPost]
        [Route("ExportToPdf")]
        public async Task<IActionResult> ExportToPdf([FromBody] GetDataFromSql input)
        {
            try
            {
                string sql = prs.Decrypt(input.sql);
                string jsonParams = input.json;

                // Chuẩn bị MemoryStream để chứa PDF
                using var pdfStream = new MemoryStream();

                // Kết nối SQL
                using (SqlConnection sqlConnection = prs.ConnectSP())
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        sqlCommand.CommandType =
                            input.typequery == "procedure" ? CommandType.StoredProcedure : CommandType.Text;

                        // ----------- Gán tham số ----------
                        if (!string.IsNullOrEmpty(jsonParams))
                        {
                            var parameters = JsonConvert.DeserializeObject<List<ParameterDefine>>(jsonParams);

                            foreach (var p in parameters)
                            {
                                object value = p.ParameterValue ?? DBNull.Value;

                                if (p.Type == "DataTable" && p.ParameterValue != null)
                                {
                                    value = JsonConvert.DeserializeObject<DataTable>(p.ParameterValue.ToString());
                                }

                                sqlCommand.Parameters.Add(new SqlParameter(p.ParameterName, value));
                            }
                        }

                        // ---------- Lấy dữ liệu ----------
                        DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);

                        // ---------- Xuất PDF ----------
                        XuLyReport xuLyReport = new XuLyReport();
                        xuLyReport.GetReport(input, dt, pdfStream);

                        byte[] pdfBytes = pdfStream.ToArray();

                        // ---------- Gửi message nếu cần ----------
                        if (input.topic != null && input.id != null)
                        {
                            if (input.IsPDFDirect)
                            {
                                // Gửi byte[] PDF
                                DataTable dtpdf = new DataTable();
                                dtpdf.Columns.Add("filepdf", typeof(byte[]));

                                DataRow row = dtpdf.NewRow();
                                row["filepdf"] = pdfBytes;
                                dtpdf.Rows.Add(row);

                                _ = SendMsgPrintAsync(input.id, input.topic, dtpdf);
                            }
                            else
                            {
                                // Gửi bảng để client tự tạo report
                                _ = SendMsgPrintAsync(input.id, input.topic, dt);
                            }
                        }

                        if (pdfBytes.Length == 0)
                            return Content("File PDF trống.");

                        return File(pdfBytes, "application/pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi tạo PDF: " + ex.Message);
            }
        }


        [HttpPost]
        [Route("exportpdf")]
        public async Task<IActionResult> exportpdf([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            // List<SqlParameter> lst = listParameter.lstparameter;
            string json = lstParameter.json;
            string jsonoutput = "";
            await using (SqlConnection sqlConnection = prs.Connect())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    List<ParameterDefine> lstpara = JsonConvert.DeserializeObject<List<ParameterDefine>>(json);

                    foreach (var it in lstpara)
                    {
                        if (it.ParameterValue != null)
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, it.ParameterValue));
                        else
                            sqlCommand.Parameters.Add(new SqlParameter(it.ParameterName, DBNull.Value));


                    }
                    sqlCommand.CommandText = sql;
                    if (lstParameter.typequery == "procedure")
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                    }
                    else
                    {
                        sqlCommand.CommandType = CommandType.Text;
                    }
                    DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();
                    jsonoutput = JsonConvert.SerializeObject(dt);
                    dt.Dispose();
                }
                catch (Exception ex)
                {
                    return new JsonResult("Lỗi: " + ex.Message); //string s = ex.Message;
                }
                //object ob = new JsonResult(lstsp);
                return Content(jsonoutput);

            }
        }

        [Route("getchart")]
        [HttpPost]
        public async Task<IActionResult> getchart([FromBody] GetDataFromSql lstParameter)
        {
            string s = prs.Decrypt(lstParameter.sql);

            ClassDieuKien classDieuKien = JsonConvert.DeserializeObject<ClassDieuKien>(s);
            try
            {

                List<string> lstsp = new List<string>();
                if (classDieuKien.MaSP != "")
                {
                    lstsp.Add(classDieuKien.MaSP);
                }
                VeBieuDoFunc veBieuDoFunc = new VeBieuDoFunc();
                string jsonoutput = await veBieuDoFunc.Vebieudo(lstsp, classDieuKien.KhachHang, classDieuKien.NhaMay);
                // string jsonoutput = JsonConvert.SerializeObject(lstimg);

                return new JsonResult(jsonoutput);

            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
            //object ob = new JsonResult(lstsp);
        }

        [Route("gettonmatbang")]
        [HttpPost]
        public async Task<IActionResult> gettonmatbang([FromBody] GetDataFromSql lstParameter)
        {
            string s = prs.Decrypt(lstParameter.sql);
            TonMatBangList tonMatBangList = JsonConvert.DeserializeObject<TonMatBangList>(s);
            //TonMatBangList tonMatBangList =  prs.Decrypt(lstParameter.sql);
            string sql = "TonMatBangTong_Ver2";
            // string sql = "TonMatBangTong";
            SqlConnection sqlConnection = prs.ConnectSP();
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@NhaMay", tonMatBangList.NhaMay);
            sqlCommand.Parameters.AddWithValue("@DongBoOrChiTiet", tonMatBangList.DongBo);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            DataTable dttonallkhuvuc = prs.dt_sqlcmd(sqlCommand, sqlConnection);

            sqlCommand.CommandType = CommandType.Text;
            string dieukiensp = "";
            string dieukien = "";
            if (tonMatBangList.lstmasp.Count > 0)
            {
                foreach (string it in tonMatBangList.lstmasp)
                {
                    if (dieukiensp == "")
                        dieukiensp = string.Format("N'{0}'", it);
                    else
                        dieukiensp = string.Format(",N'{0}'", it);
                }
                if (dieukien == "")
                {
                    dieukien = string.Format(" where Load_cbSP.MaSP in ({0})", dieukiensp);
                }
                else
                    dieukien += string.Format(" and Load_cbSP.MaSP in ({0})", dieukiensp);
            }
            if (tonMatBangList.KhachHang != "")
            {
                if (dieukien == "")
                    dieukien = string.Format(" where Load_cbSP.KhachHang=N'{0}'", tonMatBangList.KhachHang);
                else
                    dieukien += string.Format(" and Load_cbSP.KhachHang=N'{0}'", tonMatBangList.KhachHang);
            }
            if (tonMatBangList.SPMua == 1)
            {
                if (dieukien == "")
                {
                    dieukien = string.Format(" where Load_cbSP.Mua =1 and Load_cbSP.MaSP not in (select MaSP from SanPham_NgungSX where NhaMay=@NhaMay)");
                }
                else
                    dieukien += string.Format(" and Load_cbSP.Mua =1 and Load_cbSP.MaSP not in (select MaSP from SanPham_NgungSX where NhaMay=@NhaMay)");
            }


            sql = string.Format(@"select qry.MaSP,qry.SoLuong as SLDH,isnull(qrydaxuat.SLDaXuat,0) as SLDaXuat,isnull(qryktp.SLNhap,0) as TonKho
                                ,qry.SoLuong-isnull(qrydaxuat.SLDaXuat,0)-isnull(qryktp.SLNhap,0) as SLCanNK,
								isnull(qrysksp.SoKhoi,0) as SoKhoi
                                from (SELECT   [MaSP],sum([SoLuong]) as SoLuong
                                  FROM [DonHangMua]
                                  group by MaSP) as qry left join
                                 (select art.MaSP,sum(qry.SLDaXuat) as SLDaXuat from (SELECT [ArticleNumber]
                                      ,sum([SLDaXuat]) as SLDaXuat
                                  FROM [KeHoachXuatHang]
                                group by ArticleNumber) as qry
                                inner join dbo.ArticleNumberProduct art on qry.ArticleNumber=art.ArticleNumber
                                group by MaSP) as qrydaxuat on qry.MaSP=qrydaxuat.MaSP
                                left join 
                                (select MaSP,sum(SLNhap) as SLNhap from KhoTP_NK where IDNumber not in (select IDNumber from KhoTP_XK) group by MaSP) as qryktp
                                on qry.MaSP=qryktp.MaSP
								left join (
								select MaSP,sum(ChieuDayTC*ChieuRongTC*ChieuDaiTC*SoLuongCT)/1000000000 as SoKhoi from ChiTietSP where MaChiTiet in (
								SELECT [MaCT] FROM [ChiTiet_KhuVuc]
								  where KhuVuc='KV1X')  group by MaSP) as qrysksp on qryktp.MaSP=qrysksp.MaSP
                                ", tonMatBangList.NhaMay);

            DataTable dt_tonktp = prs.dt_Connect(sql, sqlConnection);


            //Lấy các chi tiết của KV4
            //Lấy chi tiết của khu vực KV1X,nếu chi tiết thuộc KV1X mà ko có ở KV4 tức là chi tiết này đã bị ráp, còn vẫn xuất hiện ở KV4 tức là Chi tiết rời
            sql = string.Format(@"
                --Lấy danh sách Các sản phẩm đang chạy
                        declare @NhaMay nvarchar(100)=N'{1}'
                       declare @datemin datetime=dateadd(MM,-6,getdate())
                        declare @tblsp as Table (MaSP nvarchar(100))
                        insert into @tblsp(MaSP)
                        select MaSP from ChiTietSP where MaChiTiet in (SELECT [MaCT]
                          FROM [KV1_KTG] where NhaMay=@NhaMay and NgayInsert>=@datemin and SLNhap>0
                          group by MaCT
						  union all
						  SELECT [MaCT]
                          FROM KV4_KhoNhung where NhaMay=@NhaMay and NgayInsert>=@datemin and SLNhap>0
                          group by MaCT
						  ) group by MaSP

                    declare @tblKV4 as Table(MaSP nvarchar(100),MaCT nvarchar(100),MaVe nvarchar(100),SoLuongCT float,CheckVe int)
                    declare @tblKV1 as Table(MaSP nvarchar(100),MaCT nvarchar(100),MaVe nvarchar(100),SoLuongCT float,CheckVe int)
                    insert into @tblKV4(MaSP,MaCT,MaVe,SoLuongCT,CheckVe)

                    SELECT MaSP,[MaCT],[MaCT] as MaVe,SLCT_QuyDoi,0
                    FROM [ChiTiet_KhuVuc] where KhuVuc='KV4'

                    insert into @tblKV1(MaSP,MaCT,MaVe,SoLuongCT,CheckVe)
                    --Chi tiết thuộc cụm
                    SELECT MaSP,[MaCT],MaCT_link as MaVe,SLCT_QuyDoi as SoLuongCT,1
                    FROM [ChiTiet_KhuVuc] where KhuVuc='KV1X' and MaCT not in (select MaCT from @tblKV4) 
                    union all
                    --Chi tiết rời
                    SELECT MaSP,[MaCT],MaSP+'CTR' as MaVe,SLCT_QuyDoi as SoLuongCT,1
                    FROM [ChiTiet_KhuVuc] where KhuVuc='KV1X' and MaCT in (select MaCT from @tblKV4)

                    select Load_cbSP.KhachHang,ctsp.MaChiTiet,Load_cbSP.MaSP,Load_cbSP.TenSP,ctsp.TenChiTiet,qry.SoLuongCT as SoLuongCT,case when ctspve.MaChiTiet is null then qry.SoLuongCT else  qry.SoLuongCT/ctspve.SoLuongCT end as SLCTTrongVe,
                                           round(ctsp.ChieuDayTC*ctsp.ChieuRongTC*ctsp.ChieuDaiTC/1000000000,5) as SoKhoi,qry.MaVe,CheckVe as checkve  
                                            ,case when  ctsp.ChieuDaiTC>3000 then 0 else  ctsp.ChieuDaiTC end as ChieuDaiTC
											
											,case when  ctsp.ChieuDaiTC>3000 then 0 else  ctsp.ChieuRongTC end as ChieuRongTC,
											
											case when  ctsp.ChieuDaiTC>3000 then 0 else  ctsp.ChieuDayTC end as ChieuDayTC
                    from
                    (select MaSP,[MaCT],MaVe,SoLuongCT,CheckVe from @tblKV4 where MaCT not in (select MaCT from @tblKV1)
                    union all
                    select MaSP,[MaCT],MaVe,SoLuongCT,CheckVe from @tblKV1) as qry
                    inner join ChiTietSP ctsp on ctsp.MaChiTiet=qry.MaCT inner join Load_cbSP on Load_cbSP.MaSP=ctsp.MaSP
                    left join ChiTietSP ctspve on qry.MaVe=ctspve.MaChiTiet
                    {0} 
                    and Load_cbSP.MaSP in (select MaSP from @tblsp)
                    order by Load_cbSP.MaSP,MaVe,checkve,ctsp.MaChiTiet
                    ", dieukien, tonMatBangList.NhaMay);

            DataTable dtSP = prs.dt_Connect(sql, sqlConnection);
            var query_groupSP = dtSP.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP"), TenSP = p.Field<string>("TenSP"), KhachHang = p.Field<string>("KhachHang") }).Select(p => new { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, KhachHang = p.Key.KhachHang });
            var qrysokhoiSP = dttonallkhuvuc.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP"), KhuVuc = p.Field<string>("KhuVuc") }).Select(p => new { MaSP = p.Key.MaSP.ToString(), KhuVuc = p.Key.KhuVuc, SoKhoi = Math.Round(p.Sum(n => n.Field<double>("SoKhoi")), 3) }).ToList();

            foreach (var it in query_groupSP)
            {
                DataRow rownew = dtSP.NewRow();
                rownew["MaSP"] = it.MaSP;
                rownew["TenSP"] = it.TenSP;
                rownew["KhachHang"] = tonMatBangList.KhachHang;
                rownew["MaChiTiet"] = it.MaSP + "CTR";
                rownew["SoLuongCT"] = 1;
                rownew["TenChiTiet"] = "Chi tiết rời";
                rownew["checkve"] = 0;
                rownew["MaVe"] = it.MaSP + "CTR";
                dtSP.Rows.Add(rownew);
            }
            DataView dv = dtSP.DefaultView;
            dv.Sort = "MaSP asc,MaVe asc,checkve asc";
            DataTable dtSPnew = dv.ToTable();

            DataTable dtresult = new DataTable();
            dtresult.Columns.Add("checkve", typeof(int));
            dtresult.Columns.Add("MaVe", typeof(string));
            dtresult.Columns.Add("KhachHang", typeof(string));
            dtresult.Columns.Add("MaCT", typeof(string));
            dtresult.Columns.Add("MaSP", typeof(string));
            dtresult.Columns.Add("TenCT", typeof(string));
            dtresult.Columns.Add("SoLuongCT", typeof(double));
            dtresult.Columns.Add("SLCTTrongVe", typeof(double));
            dtresult.Columns.Add("SoKhoiCT", typeof(double));
            dtresult.Columns.Add("ChieuDaiTC", typeof(double));
            dtresult.Columns.Add("ChieuRongTC", typeof(double));
            dtresult.Columns.Add("ChieuDayTC", typeof(double));

            var qrylistcolumn = dttonallkhuvuc.AsEnumerable().GroupBy(p => p.Field<string>("KhuVuc")).Select(p => new { KhuVuc = p.Key.ToString() }).OrderBy(p => p.KhuVuc);
            foreach (var it in qrylistcolumn)
            {
                dtresult.Columns.Add(it.KhuVuc, typeof(double));
            }
            dtresult.Columns.Add("TongDongBo", typeof(string));
            dtresult.Columns.Add("TonKTP", typeof(double));
            dtresult.Columns.Add("SLNKTP", typeof(double));
            string masp = "", maspold = "", tenspold = "";
            foreach (DataRow row in dtSPnew.Rows)
            {
                masp = row["MaSP"].ToString();
                if (maspold != masp)
                {
                    DataRow rownew = dtresult.NewRow();
                    rownew["KhachHang"] = row["KhachHang"];
                    rownew["MaSP"] = row["MaSP"];
                    rownew["TenCT"] = row["TenSP"];
                    rownew["MaCT"] = "SP";
                    rownew["MaVe"] = "";
                    rownew["checkve"] = -1;
                    tenspold = row["TenSP"].ToString();
                    maspold = masp;
                    var querydt = dt_tonktp.AsEnumerable().Where(p => p.Field<string>("MaSP").Equals(masp)).Select(p => new { TonKho = p["TonKho"], CanNK = p.Field<double>("SLCanNK"), SKTK = Math.Round(p.Field<double>("TonKho") * p.Field<double>("SoKhoi"), 3), SKCanNK = Math.Round(p.Field<double>("SLCanNK") * p.Field<double>("SoKhoi"), 3) });


                    DataRow rownewsokhoi = dtresult.NewRow();
                    rownewsokhoi["KhachHang"] = row["KhachHang"];
                    rownewsokhoi["MaSP"] = row["MaSP"];
                    rownewsokhoi["TenCT"] = "Tổng khối: " + tenspold;
                    rownewsokhoi["MaCT"] = "";
                    rownewsokhoi["MaVe"] = "zzz";
                    rownewsokhoi["checkve"] = 2;

                    var querysokhoisp = qrysokhoiSP.Where(p => p.MaSP.Equals(masp)).ToList();
                    foreach (var it in querysokhoisp)
                    {
                        rownewsokhoi[it.KhuVuc] = it.SoKhoi;
                    }

                    if (querydt.Count() > 0)
                    {
                        foreach (var it in querydt)
                        {
                            rownew["TonKTP"] = it.TonKho;
                            rownew["SLNKTP"] = it.CanNK < 0 ? 0 : it.CanNK;
                            rownewsokhoi["TonKTP"] = it.SKTK;
                            rownewsokhoi["SLNKTP"] = it.SKCanNK;
                            break;
                        }
                    }

                    dtresult.Rows.Add(rownew);
                    dtresult.Rows.Add(rownewsokhoi);
                }
                DataRow rowitem = dtresult.NewRow();
                rowitem["KhachHang"] = row["KhachHang"];
                rowitem["MaCT"] = row["MaChiTiet"];
                rowitem["MaSP"] = row["MaSP"];
                rowitem["MaVe"] = row["MaVe"];
                rowitem["TenCT"] = row["TenChiTiet"];
                rowitem["SoLuongCT"] = row["SoLuongCT"];
                rowitem["SLCTTrongVe"] = row["SLCTTrongVe"];
                rowitem["SoKhoiCT"] = row["SoKhoi"];
                rowitem["checkve"] = row["checkve"];
                rowitem["ChieuDayTC"] = row["ChieuDayTC"];
                rowitem["ChieuDaiTC"] = row["ChieuDaiTC"];
                rowitem["ChieuRongTC"] = row["ChieuRongTC"];
                //mact = rowitem["MaCT"].ToString();
                masp = rowitem["MaSP"].ToString();
                dtresult.Rows.Add(rowitem);
            }
            //Thêm tổng khối
            DataRow rownewsokhoitong = dtresult.NewRow();
            rownewsokhoitong["KhachHang"] = "";
            rownewsokhoitong["MaSP"] = "";
            rownewsokhoitong["TenCT"] = "Tổng khối: ";
            rownewsokhoitong["MaCT"] = "";
            rownewsokhoitong["MaVe"] = "";
            rownewsokhoitong["checkve"] = -1;

            var querysokhoitong = qrysokhoiSP.GroupBy(p => p.KhuVuc).Select(p => new { KhuVuc = p.Key.ToString(), SoKhoi = Math.Round(p.Sum(n => n.SoKhoi), 3) }).ToList();

            foreach (var it in querysokhoitong)
            {
                rownewsokhoitong[it.KhuVuc] = it.SoKhoi;
            }
            dtresult.Rows.Add(rownewsokhoitong);
            //
            string MaCT = "";
            foreach (DataRow rowtonmb in dttonallkhuvuc.Rows)
            {
                MaCT = rowtonmb["MaCT"].ToString();
                if (tonMatBangList.DongBo == 1)
                {
                    foreach (DataRow row in dtresult.Rows)
                    {
                        if (row["MaCT"].ToString() == MaCT)
                        {
                            row[rowtonmb["KhuVuc"].ToString()] = rowtonmb["SoLuong"];
                            //if(rowtonmb.Field<string>("KhuVuc")== "TongTonTinhChe")
                            break;
                        }
                    }
                }
                if (tonMatBangList.DongBo == 0)
                {
                    foreach (DataRow row in dtresult.Rows)
                    {
                        if (row["MaCT"].ToString() == MaCT)
                        {
                            row[rowtonmb["KhuVuc"].ToString()] = rowtonmb["SoLuong"];
                            if (rowtonmb.Field<string>("KhuVuc") == "TongTonTinhChe")
                            {
                                row["TongDongBo"] = (row["TongTonTinhChe"] != DBNull.Value) ? (row.Field<double>("TongTonTinhChe") / row.Field<double>("SoLuongCT")) : 0;
                            }
                            break;
                        }
                    }
                }
            }
            DataView dvresult = dtresult.DefaultView;
            dvresult.Sort = "MaSP asc,MaVe,checkve asc";
            dtresult = dvresult.ToTable();

            //Xử lý thêm option quy đổi vế ra chi tiết
            if (tonMatBangList.Quydoichitiet == true)
            {
                var queryve = dttonallkhuvuc.Select("(KhuVuc='KV4 KhoNhung' or KhuVuc='KV4 TonMB') and SoLuong<>0");

                if (queryve.Any())
                {

                    DataTable dtquydoichitiet = queryve.AsEnumerable().CopyToDataTable();
                    var querykhonhung = dtquydoichitiet.Select("KhuVuc='KV4 KhoNhung'").ToList();
                    var querymb = dtquydoichitiet.Select("KhuVuc='KV4 TonMB'").ToList();


                    string MaVe = "";
                    if (tonMatBangList.DongBo == 0)//Không đồng bộ
                    {
                        foreach (DataRow row in dtresult.Rows)
                        {

                            if (row.Field<int>("checkve") > 0)
                            {
                                MaVe = row.Field<string>("MaVe");//Lấy mã vế

                                foreach (DataRow rowitem in querykhonhung)
                                {
                                    if (MaVe == rowitem.Field<string>("MaCT"))
                                    {
                                        row[rowitem.Field<string>("KhuVuc")] = double.Parse(rowitem["SoLuong"].ToString()) * row.Field<double>("SLCTTrongVe");
                                        break;
                                    }
                                }
                                foreach (DataRow rowitem in querymb)
                                {
                                    if (MaVe == rowitem.Field<string>("MaCT"))
                                    {
                                        row[rowitem.Field<string>("KhuVuc")] = double.Parse(rowitem["SoLuong"].ToString()) * row.Field<double>("SLCTTrongVe");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else//Có đồng bộ
                    {
                        foreach (DataRow row in dtresult.Rows)
                        {

                            if (row.Field<int>("checkve") > 0)
                            {
                                MaVe = row.Field<string>("MaVe");//Lấy mã vế

                                foreach (DataRow rowitem in querykhonhung)
                                {
                                    if (MaVe == rowitem.Field<string>("MaCT"))
                                    {
                                        row[rowitem.Field<string>("KhuVuc")] = rowitem["SoLuong"];
                                        break;
                                    }
                                }
                                foreach (DataRow rowitem in querymb)
                                {
                                    if (MaVe == rowitem.Field<string>("MaCT"))
                                    {
                                        row[rowitem.Field<string>("KhuVuc")] = rowitem["SoLuong"];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    dtquydoichitiet.Dispose();
                    querykhonhung.Clear();
                    querymb.Clear();
                }
            }

            // string pathexcelexport = Path.Combine(Model.ModelAdmin.pathappdocument, "TonMatBang_Ver2.xlsx");
            //prs.ExportExcel_HeaderwithNote(dtfinal, Model.ModelAdmin.pathexcelexport, 2, 1, "AAAAa", 1);
            string GhiChu = "";

            if (tonMatBangList.DongBo == 1)
                GhiChu = "ĐVT: bộ";
            if (tonMatBangList.DongBo == 0)
                GhiChu = "ĐVT: cái";
            if (tonMatBangList.KhachHang != "")
                GhiChu += "; Khách hàng: " + tonMatBangList.KhachHang;
            if (tonMatBangList.NhaMay != "")
                GhiChu += "; Nhà máy: " + tonMatBangList.NhaMay;




            dtSP.Clear();
            //dtfinal.Clear();
            dttonallkhuvuc.Clear();
            dt_tonktp.Clear();
            dtSPnew.Clear();
            sqlCommand.Dispose();
            sqlConnection.Close();
            string jsonoutput = JsonConvert.SerializeObject(dtresult);
            dtresult.Clear();
            dtresult.Dispose();
            return new JsonResult(jsonoutput);
        }


        //[Route("zk")]
        //[HttpPost]
        //public async Task<IActionResult> ReceiveAsync([FromForm] AttendanceModel data)
        //{
        //    await using (SqlConnection sqlConnection = prs.Connect())
        //    {
        //        sqlConnection.Open();
        //        SqlCommand sqlCommand = new SqlCommand();
        //        sqlCommand.Connection = sqlConnection;
        //        try
        //        {
        //            string s = $"Log từ {data.deviceSN} - {data.userID} lúc {data.time}";
        //            sqlCommand.CommandText = string.Format(@"INSERT INTO [test].[dbo].[dbtest]([Test])VALUES(N'{0}')",s);
        //            sqlCommand.Connection=sqlConnection;
        //            sqlCommand.ExecuteNonQuery();
        //            sqlConnection.Close();
        //            sqlCommand.Parameters.Clear();
        //            sqlCommand.Dispose();


        //        }
        //        catch (Exception ex)
        //        {
        //            return new JsonResult("Lỗi: " + ex.Message); //string s = ex.Message;
        //        }
        //    }
        //        //object ob = new 
        //        Console.WriteLine($"Log từ {data.deviceSN} - {data.userID} lúc {data.time}");
        //    // Lưu vào DB hoặc xử lý logic tại đây
        //    return Ok("Received");
        //}
        [HttpPost("zk")]
        public async Task<IActionResult> Receive()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            Console.WriteLine("Received from ZKTeco: " + body);
            await using (SqlConnection sqlConnection = prs.Connect())
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                try
                {
                    string s = $"Log nhé {body}";
                    sqlCommand.CommandText = string.Format(@"INSERT INTO [test].[dbo].[dbtest]([Test])VALUES(N'{0}')", s);
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Dispose();


                }
                catch (Exception ex)
                {
                    return new JsonResult("Lỗi: " + ex.Message); //string s = ex.Message;
                }
            }
            return Ok("OK");
        }

        public class AttendanceModel
        {
            public string deviceSN { get; set; }
            public string userID { get; set; }
            public string time { get; set; }
            public string Event { get; set; }
            public IFormFile image { get; set; } // nếu máy gửi kèm ảnh
        }

        public class FileInfo
        {
            public string userid { get; set; }
            public IFormFile file { get; set; }
        }
        public class FileViewModel
        {
            public string name { get; set; }
            public IFormFile file { get; set; }
        }

        class ClassDieuKien
        {
            public string MaSP { get; set; }
            public string NhaMay { get; set; }
            public string KhachHang { get; set; }
        }

        public class TonMatBangList
        {
            public string NhaMay { get; set; }
            public string KhachHang { get; set; }
            public List<string> lstmasp { get; set; }
            public int DongBo { get; set; }
            public int SPMua { get; set; }
            public bool Quydoichitiet { get; set; }
        }

        [HttpPost]
        public async Task SendMsgPrintAsync(string id, string topic, DataTable dt)
        {
            InPhieuJson inPhieuJson = new InPhieuJson();
            try
            {
                inPhieuJson.ketqua = "OK";
                inPhieuJson.dtsource = dt;
                inPhieuJson.id = id;
                inPhieuJson.topic = topic;
                string jsonoutputprint = JsonConvert.SerializeObject(inPhieuJson);
                JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
                jsonMsgAndroid.typemsg = "print";
                jsonMsgAndroid.message = inPhieuJson.ToJson();

                if (inPhieuJson.topic != null)
                {
                    await _hubContext.Clients.Group(topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                }
            }
            catch (Exception ex)
            {
                inPhieuJson.msg = "Lưu thành công nhưng không in được :" + ex.Message;
            }

        }
        [Route("sendMsgdataset")]
        [HttpPost]
        public async Task SendMsgPrintDatasetAsync(string id, string topic, DataSet ds)
        {
            InPhieuJson inPhieuJson = new InPhieuJson();
            try
            {
                inPhieuJson.ketqua = "OK";
                inPhieuJson.dataset = JsonConvert.SerializeObject(ds);
                inPhieuJson.id = id;
                inPhieuJson.topic = topic;
                string jsonoutputprint = JsonConvert.SerializeObject(inPhieuJson);
                JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
                jsonMsgAndroid.typemsg = "print";
                jsonMsgAndroid.message = inPhieuJson.ToJson();

                if (inPhieuJson.topic != null)
                {
                    await _hubContext.Clients.Group(topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                }
            }
            catch (Exception ex)
            {
                inPhieuJson.msg = "Lưu thành công nhưng không in được :" + ex.Message;
            }

        }



    }
}
