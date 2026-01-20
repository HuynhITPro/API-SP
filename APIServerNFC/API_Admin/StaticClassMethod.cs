using APIServerNFC.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace APIServerNFC.API_Admin
{
    public class StaticClassMethod
    {
        public static string DeleteFile(Model.FileHoSo fileHoSo, SqlConnection sqlConnection)
        {
           
            SqlCommand sqlCommand = new SqlCommand("FileHoSo_Delete", sqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            sqlCommand.Transaction = sqlTransaction;
            try
            {
                sqlCommand.Parameters.AddWithValue("@Serial", fileHoSo.Serial);
                sqlCommand.ExecuteNonQuery();
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic/", fileHoSo.UrlFile);
                FileInfo file = new FileInfo(pathBuilt);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
               
                }
                //App_ClassDefine.ClassProcess.DeleteFileishost(Path.Combine(Model.ModelAdmin.pathhostfile, fileHoSo.UrlFile));
                sqlTransaction.Commit();
                sqlCommand.Dispose();
                sqlTransaction.Dispose();
                return "OK";
            }
            catch(Exception ex)
            {
                sqlTransaction.Rollback();
                sqlTransaction.Dispose();
                return ex.Message;
            }


            //SqlConnection sqlConnection=
        }
        public static string SaveFileHoSo(Model.FileHoSo filehoso, SqlConnection sqlConnection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand("FileHoSo_Insert", sqlConnection);
                sqlCommand.CommandType=System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@TableName", filehoso.TableName);
                sqlCommand.Parameters.AddWithValue("@TenFile", filehoso.TenFile);
                sqlCommand.Parameters.AddWithValue("@UrlFile", filehoso.UrlFile);
                sqlCommand.Parameters.AddWithValue("@DienGiai", filehoso.DienGiai);
                sqlCommand.Parameters.AddWithValue("@DungLuong", (filehoso.DungLuong==null)?0:Math.Round(filehoso.DungLuong.Value, 3));
                sqlCommand.Parameters.AddWithValue("@DVT", "MB");
                sqlCommand.Parameters.AddWithValue("@UserInsert", filehoso.UserInsert);
                sqlCommand.Parameters.AddWithValue("@SerialLink", filehoso.SerialLink);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }



            //SqlConnection sqlConnection=
        }
       
        public class dbLogErr
        {
            public string ErrMsg { get; set; }
            public string NameMethod { get; set; }
            public string? IdMachine { get; set; }
            Exception Exception { get; set; }
            public dbLogErr(Exception ex)
            {
                ErrMsg = string.Format("Lỗi {0}: dòng {1}", ex.Message, new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                NameMethod = ex.TargetSite.Name;

            }
            public dbLogErr(string errMsg, string nameMethod)
            {
                ErrMsg = errMsg;
                NameMethod = nameMethod;

            }
            public dbLogErr()
            {

            }
        }
        static ClassProcess prs = new ClassProcess();
        public static void InsertLogErrElectric(dbLogErr dbLogErr)
        {
            SqlConnection sqlConnection = prs.ConnectElectric();
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "ElectricMeter_Log.dbo.dbLogErr_Insert";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new SqlParameter("@ErrMsg", dbLogErr.ErrMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@NameMethod", dbLogErr.NameMethod));
            sqlCommand.Parameters.Add(new SqlParameter("@IDMachine", (object?)dbLogErr.IdMachine ?? DBNull.Value));
            prs.dt_sqlcmd(sqlCommand, sqlConnection);
            sqlConnection.Close();
            sqlCommand.Dispose();
        }
        public static void InsertLogErrElectric(Exception ex)
        {
            try
            {
                SqlConnection sqlConnection = prs.ConnectElectric();
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                dbLogErr dbLogErr = new dbLogErr(ex);
                sqlCommand.CommandText = "ElectricMeter_Log.dbo.dbLogErr_Insert";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ErrMsg", dbLogErr.ErrMsg));
                sqlCommand.Parameters.Add(new SqlParameter("@NameMethod", dbLogErr.NameMethod));
                sqlCommand.Parameters.Add(new SqlParameter("@IDMachine", (object?)dbLogErr.IdMachine ?? DBNull.Value));
                prs.dt_sqlcmd(sqlCommand, sqlConnection);
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            catch (Exception exms)
            {
                Console.WriteLine(exms.Message.ToString());
            }
        }
    }
}
