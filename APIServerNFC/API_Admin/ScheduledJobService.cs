using Microsoft.AspNetCore.Hosting;
using Quartz;
using System.Threading.Tasks;
using System;
using APIServerNFC.Controllers;
using Microsoft.Data.SqlClient;


namespace APIServerNFC.API_Admin
{
    public class ScheduledJobService : IJob
    {
        private readonly MqttService _mqttService;
        ClassProcess prs=new ClassProcess();
        public ScheduledJobService(MqttService mqttService)
        {
            _mqttService = mqttService;
        }

        public async Task Execute(IJobExecutionContext context)
        {


            DateTime dateTime = DateTime.Now;
            int minute = dateTime.Minute;
            try
            {
                Console.WriteLine("Scheduled job executed at: " + dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if(_mqttService._mqttClient.IsConnected)
                {
                    Console.WriteLine("MQTT đang connected");
                }
               
                if (minute % 2 == 0)// Every 2 minutes
                {
                   _= _mqttService.ReconnectAsync();
                }
                if(minute==0)//Kiểm tra 1 tiếng 1 lần, xem có đề nghị sửa máy nào chưa đóng không
                {
                    using (SqlConnection sqlConnection = prs.Connect())
                    {
                        try
                        {
                            sqlConnection.Open();
                            SqlCommand sqlCommand = new SqlCommand("[NVLDB].[dbo].NvlBaoTriMaster_AutoCommit", sqlConnection);
                            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCommand.ExecuteNonQuery();
                            sqlCommand.Dispose();

                            sqlConnection.Close();

                        }
                        catch (Exception ex)
                        {
                           
                        }

                    }
                }
                //Kiem
              
            }
            catch (Exception ex)
            {
                StaticClassMethod.InsertLogErrElectric(new StaticClassMethod.dbLogErr(ex));
                //StartUp.InsertLogErr(ex);
            }
            // Thêm logic xử lý của bạn ở đây
            //return Task.CompletedTask;
        }
    }
}
