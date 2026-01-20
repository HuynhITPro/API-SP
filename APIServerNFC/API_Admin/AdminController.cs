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
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Caching.Memory;



namespace APIServerNFC.API_Admin
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        ClassProcess prs = new ClassProcess();
        private readonly IHubContext<ChatHub> _hubContext;//ChatHub dùng để gửi tin nhắn

        public AdminController(IHubContext<ChatHub> hubContext)
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
                await using (SqlConnection sqlConnection = prs.Connect())
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
                await using (SqlConnection sqlConnection = prs.Connect())
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
                await using (SqlConnection sqlConnection = prs.Connect())
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
                await using (SqlConnection sqlConnection = prs.Connect())
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

        [HttpPost("UploadImgAvatar")]
        public async Task<IActionResult> UploadImgAvatar(IFormFile file, [FromForm] string foldername, [FromForm] string filehosojson)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new JsonResult("No file uploaded");
                }
                var pathforlder = Path.Combine("SupplierPublic/public/");
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
               
                string filenamehost = fileHoSo.UserInsert.ToLower() + Path.GetExtension(file.FileName);
                fileHoSo.UrlFile =foldername + "/" + filenamehost;

               

                var path = Path.Combine(Directory.GetCurrentDirectory(), pathforlder+fileHoSo.UrlFile);

                SqlConnection sqlConnection = prs.Connect();
                sqlConnection.Open();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                string sql = "UPDATE [dbo].[Users] SET [PathImg] =@PathImg   WHERE UsersName=@UserName";
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@PathImg", fileHoSo.UrlFile);
                sqlCommand.Parameters.AddWithValue("@UserName", fileHoSo.UserInsert);
               

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
                using (SqlConnection sqlConnection = prs.Connect())
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
        [Route("ExportToPdfWithSource")]
        public async Task<IActionResult> ExportToPdfWithSource([FromBody] GetDataFromSql lstParameter)
        {
            string sql = prs.Decrypt(lstParameter.sql);
            await using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(sql);
                    XuLyReport xuLyReport = new XuLyReport();
                    xuLyReport.GetReport(lstParameter, dt, stream);
                  
                    dt.Dispose();
                    if (stream.Length == 0)
                    {
                        return Content("File PDF trống.AAAA");
                    }
                    return File(stream.ToArray(), "application/pdf");

                    // return Content("OK rồi");

                }
                catch (Exception ex)
                {
                    return Content("File PDF trống: " + ex.Message);
                }
                //return File(stream.ToArray(), "application/pdf");
                return File(stream.ToArray(), "application/pdf");
            }
            //object ob = new JsonResult(lstsp);
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

        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

       

        [HttpGet("proxy-image")]
        public async Task<IActionResult> ProxyImage([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL required");

            using var http = new HttpClient();
            var bytes = await http.GetByteArrayAsync(url);
            return File(bytes, "image/jpeg");
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
