using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIServerNFC
{
    public static class ConnectionService
    {
       // static string server = @".\SQLEXPRESS";
        //static string server = @"
        //192.168.1.155";
        //static string server = @".";
        static string server = @"123.31.41.25,1988";
       

        static string serverSP = @"113.161.144.105,3389";//Server 2, dùng cho API cty
        static string serverSPSer10 = @"113.161.144.105,4827";//Server 2, dùng cho API cty
        static string DataBaseMain = "DBMaster";//Server 2, dùng cho API cty
        static string DataBaseMainSP = "DataBase_ScansiaPacific2014";
        static string DataBaseMainElectric = "ElectricMeter";
        static string userSql = "vuthaihuynh";
        static string passSql = "@vuthaihuynh@123456";

        public static string connstring = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", server, DataBaseMain, userSql, passSql);
        public static string connstringSP = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", serverSP, DataBaseMainSP, userSql, passSql);
        public static string connstringElectric = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", server, DataBaseMainElectric, userSql, passSql);
        //public static string connstring = ClassProcess.Connect
    }
    public class ClassProcess
    {
        public ClassProcess()
        {

        }

        public SqlConnection Connect()//Ket Noi
        {
            //string Ketnoi = string.Format(@"Data Source={0};Initial Catalog={3};User ID={1};Password={2};", server, userSql, passSql, DataBaseMain);

            SqlConnection conn = new SqlConnection(ConnectionService.connstring);
            return conn;
        }
        public SqlConnection ConnectSP()//Ket Noi
        {
            //string Ketnoi = string.Format(@"Data Source={0};Initial Catalog={3};User ID={1};Password={2};", server, userSql, passSql, DataBaseMain);

            SqlConnection conn = new SqlConnection(ConnectionService.connstringSP);
            return conn;
        }
        public SqlConnection ConnectElectric()//Ket Noi
        {
            //string Ketnoi = string.Format(@"Data Source={0};Initial Catalog={3};User ID={1};Password={2};", server, userSql, passSql, DataBaseMain);

            SqlConnection conn = new SqlConnection(ConnectionService.connstringElectric);
            return conn;
        }
        public DataTable dt_sqlcmd(SqlCommand cmd, SqlConnection Conn)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            da.Dispose();
            //Conn.Close();
            return dt;
        }
        public DataTable dt_Connect(string sql, SqlConnection Conn)//Khoi tao dataset
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, Conn);
            DataTable ds1 = new DataTable();
            da.Fill(ds1);
            da.Dispose();
            return ds1;
        }
        public DataSet dts_sqlcmd(SqlCommand cmd, SqlConnection Conn)
        {
            DataSet dt = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            da.Dispose();
            //Conn.Close();
            return dt;
        }
        public string Decrypt(string cipherText)
        {
            string key = "huynhit1111111111111111111111111";
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Convert.FromBase64String(cipherText);

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7Padding");
            cipher.Init(false, new KeyParameter(keyBytes));

            byte[] outputBytes = cipher.DoFinal(inputBytes);

            return Encoding.UTF8.GetString(outputBytes);
        }
        public string RandomString(int length)
        {

            Random random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        private string ChuyenSoThanhChu(string gNumber)
        {
            string result = "";
            switch (gNumber)
            {
                case "0":
                    result = "không";
                    break;

                case "1":
                    result = "một";
                    break;

                case "2":
                    result = "hai";
                    break;
                case "3":
                    result = "ba";
                    break;
                case "4":
                    result = "bốn";
                    break;
                case "5":
                    result = "năm";
                    break;

                case "6":
                    result = "sáu";
                    break;
                case "7":
                    result = "bảy";
                    break;

                case "8":
                    result = "tám";
                    break;

                case "9":
                    result = "chín";
                    break;
            }
            return result;
        }
        private string Docphanthapphan(int n)//phần thập phân được tách ra thành số nguyên để đọc
        {
            string text = "";
            string so = n.ToString();
            for (int i = 0; i < so.Length; i++)
            {

                text += ChuyenSoThanhChu(so[i].ToString()) + " ";
            }
            return text.Trim();
        }
        public string dochangchuc(double so, bool daydu)
        {
            string chuoi = "";
            int chuc = (int)Math.Floor(so / 10);
            int donvi = (int)so % 10;
            if (chuc > 1)
            {
                chuoi = " " + mangso[chuc] + " mươi";
                if (donvi == 1)
                {
                    chuoi += " mốt";
                }
            }
            else if (chuc == 1)
            {
                chuoi = " mười";
                if (donvi == 1)
                {
                    chuoi += " một";
                }
            }
            else if (daydu && donvi > 0)
            {
                chuoi = " lẻ";
            }
            if (donvi == 5 && chuc >= 1)
            {
                chuoi += " lăm";
            }
            else if (donvi > 1 || (donvi == 1 && chuc == 0))
            {
                chuoi += " " + mangso[donvi];
            }
            return chuoi;
        }
        //Đọc block 3 số
        public string docblock(double so, bool daydu)
        {
            string chuoi = "";
            int tram = (int)Math.Floor(so / 100);
            so = so % 100;
            if (daydu || tram > 0)
            {
                chuoi = " " + mangso[tram] + " trăm";
                chuoi += dochangchuc(so, true);
            }
            else
            {
                chuoi = dochangchuc(so, false);
            }
            return chuoi;
        }
        //Đọc số hàng triệu
        public string dochangtrieu(double so, bool daydu)
        {
            string chuoi = "";
            int trieu = (int)Math.Floor(so / 1000000);
            so = so % 1000000;
            if (trieu > 0)
            {
                chuoi = docblock(trieu, daydu) + " triệu, ";
                daydu = true;
            }
            double nghin = Math.Floor(so / 1000);
            so = so % 1000;
            if (nghin > 0)
            {
                chuoi += docblock(nghin, daydu) + " nghìn, ";
                daydu = true;
            }
            if (so > 0)
            {
                chuoi += docblock(so, daydu);
            }
            return chuoi;
        }
        private string[] mangso = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        public string docsonguyen(double so)
        {
            if (so == 0) return mangso[0];
            string chuoi = "", hauto = "";
            do
            {
                double ty = so % 1000000000;
                so = Math.Floor(so / 1000000000);
                if (so > 0)
                {
                    chuoi = dochangtrieu(ty, true) + hauto + chuoi;
                }
                else
                {
                    chuoi = dochangtrieu(ty, false) + hauto + chuoi;
                }
                hauto = " tỷ, ";
            } while (so > 0);
            try
            {
                if (chuoi.Trim().Substring(chuoi.Trim().Length - 1, 1) == ",")
                { chuoi = chuoi.Trim().Substring(0, chuoi.Trim().Length - 1); }
            }
            catch { }
            return chuoi.Trim();
        }
        public string docsothapphan(double d)//Tối đa lấy 3 số sau dấu phẩy thôi
        {
            string chu = "";
            string so = d.ToString("######0.###");
            string[] arr = so.Split('.');
            chu = docsonguyen(double.Parse(arr[0]));
            if (arr.Length == 2)
            {
                chu += ", " + Docphanthapphan(int.Parse(arr[1].ToString()));
            }
            return FirstCharToUpper(chu);
        }
        public string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Chuyển đổi ký tự đầu tiên thành chữ in hoa
            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);

            return new string(charArray);
        }
      
    }
}
