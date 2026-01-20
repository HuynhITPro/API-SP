using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Drawing.Imaging;
using Org.BouncyCastle.Asn1.X509;

namespace APIServerNFC.API_Admin
{
   
    public class BieuDoMatBangController
    {

        class ClassDieuKien
        {
            public string MaSP { get; set; }
            public string NhaMay { get; set; }
            public string KhachHang { get; set; }
        }
        ClassProcess prs = new ClassProcess();
     
        public async Task<IActionResult> GetChartEncrypt([FromBody] string dieukien)
        {
            string s= prs.Decrypt(dieukien);
            ClassDieuKien classDieuKien = JsonConvert.DeserializeObject<ClassDieuKien>(s);
            try
            {
                
                List<string> lstsp = new List<string>();
                if(classDieuKien.MaSP!="")
                {
                    lstsp.Add(classDieuKien.MaSP);
                }
                VeBieuDoFunc veBieuDoFunc = new VeBieuDoFunc();
                string jsonoutput=await veBieuDoFunc.Vebieudo(lstsp, classDieuKien.KhachHang, classDieuKien.MaSP);
                //string jsonoutput = JsonConvert.SerializeObject(lstimg);
              
                return new JsonResult(jsonoutput);
               
            }
            catch (Exception ex)
            {
                return new JsonResult(string.Format("Lỗi: {0}", ex.Message));
            }
            //object ob = new JsonResult(lstsp);
        }
       
    }
}
