using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace APIServerNFC
{
    public class GetDataFromSql
    {
        public GetDataFromSql()
        {

        }
        public string json { get; set; }
        public string sql { get; set; }

        //Sử dụng để gửi tin nhắn
        public string topic { get; set; }
        public string id { get; set; }
        public string reportname { get; set; }
        public string dtparameter { get; set; }
        public string typequery { get; set; }
        public bool IsPDFDirect { get; set; } = false;
    }
    public class ClassReport
    {

        public GetDataFromSql getDataFromSql { get; set; } = new GetDataFromSql();
        public List<ParameterDefine> lstpara { get; set; } = new List<ParameterDefine>();
        public string reportname { get; set; }
        public string dtparameter { get; set; }

    }
    public class ParameterDefine
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public ParameterDefine(string name, object value)
        {
            ParameterName = name;
            ParameterValue = value;
        }
        public string Type { get; set; }
    }
}
