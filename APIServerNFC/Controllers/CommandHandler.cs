using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Generic;
using DevExpress.XtraCharts;
using APIServerNFC.API_Admin;

namespace APIServerNFC.Controllers
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(string topic, string message);
    }
    public class CommandHandler : ICommandHandler
    {

        public class ElectricData
        {
            public string Id { get; set; }
            public double Diagnostic { get; set; }
            public double Kwh { get; set; }
            public double PF { get; set; }
            public double Kw { get; set; }
            public double Uan { get; set; }
            public double Ubn { get; set; }
            public double Ucn { get; set; }
            public double Uln { get; set; }
            public double Uab { get; set; }
            public double Ubc { get; set; }
            public double Uca { get; set; }
            public double UII { get; set; }
            public double Ia { get; set; }
            public double Ib { get; set; }
            public double Ic { get; set; }
            public double II { get; set; }
            public double KWa { get; set; }
            public double KWb { get; set; }
            public double KWc { get; set; }
            public double Kvara { get; set; }
            public double Kvarb { get; set; }
            public double Kvarc { get; set; }
            public double Kvart { get; set; }
            public double Kvarh { get; set; }
            public double KVAh { get; set; }
            public double KVAa { get; set; }
            public double KVAb { get; set; }
            public double KVAc { get; set; }
            public double KVAt { get; set; }
            public double PFa { get; set; }
            public double PFb { get; set; }
            public double PFc { get; set; }
            public double Ia_TDD { get; set; }
            public double Ib_TDD { get; set; }
            public double Ic_TDD { get; set; }
            public double Ia_K { get; set; }
            public double Ib_K { get; set; }
            public double Ic_K { get; set; }
            public double Ia_THD { get; set; }
            public double Ib_THD { get; set; }
            public double Ic_THD { get; set; }
            public DateTime? NgayInsert { get; set; }
        }


        List<ElectricData> lstcheck = new List<ElectricData>();
        //Xử lý trước khi gọi SQL để tránh làm phiền máy chủ nhiều lần
        public Task HandleCommandAsync(string topic, string message)
        {
            //Console.WriteLine(message);
            try
            {
                ElectricData data = JsonConvert.DeserializeObject<ElectricData>(message);

                data.NgayInsert = DateTime.Now;
                var querycheck=lstcheck.Find(x => x.Id == data.Id);
                if(querycheck == null)
                {
                    lstcheck.Add(data);
                    saveLog(data);
                }
                else
                {
                    if((data.NgayInsert.Value - querycheck.NgayInsert.Value).Minutes >= 15)//Cứ 15 phút gọi xuống sql 1 lần, để tránh làm phiền máy chủ
                    {
                        saveLog(data);
                        querycheck.NgayInsert = data.NgayInsert.Value;
                    }
                }
               

               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                StaticClassMethod.InsertLogErrElectric(ex);
            }
            //}
            //if (message.Equals("ON", StringComparison.OrdinalIgnoreCase))
            //{
            //    Console.WriteLine(">>> Thiết bị bật");
            //}
            //else if (message.Equals("OFF", StringComparison.OrdinalIgnoreCase))
            //{
            //    Console.WriteLine(">>> Thiết bị tắt");
            //}
            //else
            //{
            //    Console.WriteLine($">>> Lệnh không xác định: {message}");
            //}
            return Task.CompletedTask;
        }
        ClassProcess prs = new ClassProcess();
        private void saveLog(ElectricData data)
        {
            using (SqlConnection sqlConnection = prs.ConnectElectric())
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandText = "ElectricMeter.dbo.PowerData_Insert";
                // DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);//Sửa về chữ đầu tiên trong biến là viết thường cho trùng với kiểu Parse của retrofit2

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Serial", 0);
                cmd.Parameters.AddWithValue("@Id", data.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Diagnostic", data.Diagnostic);
                cmd.Parameters.AddWithValue("@Kwh", data.Kwh);
                cmd.Parameters.AddWithValue("@PF", data.PF);
                cmd.Parameters.AddWithValue("@Kw", data.Kw);
                cmd.Parameters.AddWithValue("@Uan", data.Uan);
                cmd.Parameters.AddWithValue("@Ubn", data.Ubn);
                cmd.Parameters.AddWithValue("@Ucn", data.Ucn);
                cmd.Parameters.AddWithValue("@Uln", data.Uln);
                cmd.Parameters.AddWithValue("@Uab", data.Uab);
                cmd.Parameters.AddWithValue("@Ubc", data.Ubc);
                cmd.Parameters.AddWithValue("@Uca", data.Uca);
                cmd.Parameters.AddWithValue("@UII", data.UII);
                cmd.Parameters.AddWithValue("@Ia", data.Ia);
                cmd.Parameters.AddWithValue("@Ib", data.Ib);
                cmd.Parameters.AddWithValue("@Ic", data.Ic);
                cmd.Parameters.AddWithValue("@II", data.II);
                cmd.Parameters.AddWithValue("@KWa", data.KWa);
                cmd.Parameters.AddWithValue("@KWb", data.KWb);
                cmd.Parameters.AddWithValue("@KWc", data.KWc);
                cmd.Parameters.AddWithValue("@Kvara", data.Kvara);
                cmd.Parameters.AddWithValue("@Kvarb", data.Kvarb);
                cmd.Parameters.AddWithValue("@Kvarc", data.Kvarc);
                cmd.Parameters.AddWithValue("@Kvart", data.Kvart);
                cmd.Parameters.AddWithValue("@Kvarh", data.Kvarh);
                cmd.Parameters.AddWithValue("@KVAh", data.KVAh);
                cmd.Parameters.AddWithValue("@KVAa", data.KVAa);
                cmd.Parameters.AddWithValue("@KVAb", data.KVAb);
                cmd.Parameters.AddWithValue("@KVAc", data.KVAc);
                cmd.Parameters.AddWithValue("@KVAt", data.KVAt);
                cmd.Parameters.AddWithValue("@PFa", data.PFa);
                cmd.Parameters.AddWithValue("@PFb", data.PFb);
                cmd.Parameters.AddWithValue("@PFc", data.PFc);
                cmd.Parameters.AddWithValue("@Ia_TDD", data.Ia_TDD);
                cmd.Parameters.AddWithValue("@Ib_TDD", data.Ib_TDD);
                cmd.Parameters.AddWithValue("@Ic_TDD", data.Ic_TDD);
                cmd.Parameters.AddWithValue("@Ia_K", data.Ia_K);
                cmd.Parameters.AddWithValue("@Ib_K", data.Ib_K);
                cmd.Parameters.AddWithValue("@Ic_K", data.Ic_K);
                cmd.Parameters.AddWithValue("@Ia_THD", data.Ia_THD);
                cmd.Parameters.AddWithValue("@Ib_THD", data.Ib_THD);
                cmd.Parameters.AddWithValue("@Ic_THD", data.Ic_THD);
                object result = cmd.ExecuteScalar();
                //Console.WriteLine(result?.ToString());
                //using (SqlDataReader reader = cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        //string ketqua = reader["ketqua"].ToString();
                //        //Console.WriteLine("Kết quả: " + ketqua);
                //    }
                //}

                sqlConnection.Close();

                cmd.Dispose();

            }
        }
    }
}
