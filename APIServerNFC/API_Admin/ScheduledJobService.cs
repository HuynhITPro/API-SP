using Microsoft.AspNetCore.Hosting;
using Quartz;
using System.Threading.Tasks;
using System;
using APIServerNFC.Controllers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;


namespace APIServerNFC.API_Admin
{
    public class ScheduledJobService : IJob
    {
        private readonly MqttService _mqttService;
        private readonly FcmSender _fcmSender;

        ClassProcess prs =new ClassProcess();
        public ScheduledJobService(MqttService mqttService, FcmSender fcmSender)
        {
            _mqttService = mqttService;
            _fcmSender = fcmSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {


            DateTime dateTime = DateTime.Now;
            int hour = dateTime.Hour;
             bool ho= hour>=7&&hour<=17;//Giờ hành chính
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
                if(minute%30==0&& ho)//30 phút gửi tin nhắn nhắc nhở 1 lần
                {
                    using (SqlConnection sqlConnection = prs.ConnectSP())
                    {
                        try
                        {
                            sqlConnection.Open();
                            SqlCommand sqlCommand = new SqlCommand("[DataBase_ScansiaPacific2014].[dbo].GiaoNhanSanXuat_SendMsgRemind", sqlConnection);
                            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);
                            sqlCommand.Dispose();
                            if(dt.Rows.Count>0)
                            {
                                List<string> token = new List<string>();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dr["UserQuanLy"]!=DBNull.Value)
                                    {
                                       _= _fcmSender.SendNotificationToTopicAsync(dr["UserQuanLy"].ToString(), "Lời nhắc: Xác nhận nhận hàng", string.Format("Bạn có {0} phiếu cần xác nhận", dr["SoPhieu"].ToString()),"");
                                    }
                                      
                                }
                              
                                //fcmController.sendlisttopic(token,"Xác nhận nhận hàng",string.Format("Có {0} phiếu giao hàng cần bạn xác nhận",ro)

                            }
                            sqlConnection.Close();

                        }
                        catch (Exception ex)
                        {
                            StaticClassMethod.InsertLogErrElectric(new StaticClassMethod.dbLogErr(ex));
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
