using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace APIServerNFC.API_Admin
{
    public class JsonMsgAndroid
    {
        public string topic { get; set; }
        public string clientid { get; set; }
        public string message { get; set; }
        public string usersend { get; set; }
        public string msgJson { get; set; }
        public List<string> lstuserrecive { get; set; } = new List<string>();
        public string ToJson()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
        public string typemsg { get; set; }
    }
    public class InPhieuJson
    {
        public string id { get; set; }
        public string topic { get; set; }
        public string tablename { get; set; }
        public string msg { get; set; }
        public string typename { get; set; }
        public string ketqua { get; set; }
        public string ToJson()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

        public DataTable dtsource { get; set; }
        public string dataset { get; set; }

    }
}
