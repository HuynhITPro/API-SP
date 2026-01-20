using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using static APIServerNFC.Controllers.CommandHandler;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static APIServerNFC.API_Admin.StaticClassMethod;
using APIServerNFC.API_Admin;

namespace APIServerNFC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricController : ControllerBase
    {
       
        public class TableMaster
        {
            public int Serial { get; set; }
            public int Id { get; set; }
            public string NameMaster { get; set; }
            public int? IdRs { get; set; }
            public DateTime? NgayInsert { get; set; }
        }
        public class TableItem
        {
            public int Serial { get; set; }
            public int SerialLink { get; set; }
            public int IdItem { get; set; }
            public string NameItem { get; set; }
            public DateTime? NgayInsert { get; set; }
        }
        ClassProcess prs = new ClassProcess();

        [HttpPost]
        [Route("GetInfoTable")]
        public async Task<IActionResult> GetInfoTable([FromBody] string id)
        {
            using (SqlConnection sqlConnection = prs.ConnectElectric())
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandText = "ElectricMeter.dbo.GetTableElectricInfo";
                // DataTable dt = prs.dt_sqlcmd(sqlCommand, sqlConnection);//Sửa về chữ đầu tiên trong biến là viết thường cho trùng với kiểu Parse của retrofit2

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);
               DataTable dt=prs.dt_sqlcmd(cmd, sqlConnection);
                string jsonoutput = JsonConvert.SerializeObject(dt);
                sqlConnection.Close();

                cmd.Dispose();
                return Content(jsonoutput);

            }
        }

        [HttpPost]
        [Route("WriteLog")]
        public async Task<IActionResult> WriteLog([FromBody] dbLogErr logerr)
        {
            try
            {
                StaticClassMethod.InsertLogErrElectric(logerr);
                return Content("OK");
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }


    }
}
