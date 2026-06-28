using Microsoft.AspNetCore.Hosting;
using Quartz;
using System.Threading.Tasks;
using System;
using APIServerNFC.Controllers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text;


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
        private bool CheckGioLamViec(DateTime dateTime)
        {
            int hour = dateTime.Hour;
            return hour >= 7 && hour <= 17
                   && dateTime.DayOfWeek != DayOfWeek.Sunday;
        }

        public async Task Execute(IJobExecutionContext context)
        {


            DateTime dateTime = DateTime.Now;
            bool giolamviec = CheckGioLamViec(dateTime);
            
            int hour = dateTime.Hour;
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
                if(minute%30==0&& giolamviec)//30 phút gửi tin nhắn nhắc nhở 1 lần
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
                if(minute==1 && giolamviec && hour%2==1)
                {
                    using (SqlConnection sqlConnection = prs.Connect())
                    {
                        try
                        {
                            sqlConnection.Open();
                            SqlCommand sqlCommand = new SqlCommand("[ElectricMeter].[dbo].CheckDongHoHoatDong", sqlConnection);
                            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            DataSet ds = prs.dts_sqlcmd(sqlCommand, sqlConnection);
                            sqlCommand.Dispose();
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach(DataRow row in ds.Tables[0].Rows) 
                                {
                                    stringBuilder.Append(string.Format("Đồng hồ:{0} bị hư lúc {1}, ",row.Field<string>("MaHang"),row.Field<DateTime>("Ngay").ToString("dd/MM/yy HH:mm")));
                                    
                                }
                                foreach (DataRow dr in ds.Tables[1].Rows)
                                {
                                    if (dr["UserName"] != DBNull.Value)
                                    {
                                        _ = _fcmSender.SendNotificationToTopicAsync(dr["UserName"].ToString(), "Đồng hồ điện bị hư", stringBuilder.ToString(), "");
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
                if (minute == 58 && giolamviec)
                {
                    using (SqlConnection sqlConnection = prs.Connect())
                    {
                        try
                        {
                            sqlConnection.Open();
                            SqlCommand sqlCommand = new SqlCommand("[ElectricMeter].[dbo].AlarmDienNangTheoGio", sqlConnection);
                            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            DataSet ds = prs.dts_sqlcmd(sqlCommand, sqlConnection);
                            sqlCommand.Dispose();
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    stringBuilder.Append(string.Format("Đồng hồ {0}: {1} Kwh, {2}, ", row.Field<string>("Id"), row.Field<double>("Kwh").ToString("#,#.#"),(row.Field<double>("TyLe")>0)?("tăng "+ row.Field<double>("TyLe").ToString("P0")):("giảm "+ row.Field<double>("TyLe").ToString("P0"))));

                                }
                                foreach (DataRow dr in ds.Tables[1].Rows)
                                {
                                    if (dr["UserName"] != DBNull.Value)
                                    {
                                        _ = _fcmSender.SendNotificationToTopicAsync(dr["UserName"].ToString(), string.Format("Điện năng tiêu thụ {0}-{1} giờ",hour,hour+1), stringBuilder.ToString(), "");
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
