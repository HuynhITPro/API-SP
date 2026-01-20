using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Data;
using System.IO;
using System.Linq;




namespace APIServerNFC.Api_Report
{
    public class XuLyReport
    {
        ClassProcess prs = new ClassProcess(); 
        public bool GetReport(GetDataFromSql getDataFromSql,DataTable dtsource,MemoryStream stream) {

            var report = CreateReportWithName.CreateReportInstance(getDataFromSql.reportname);
            //if (report == null)
            //    return false;
            //XtraReport report = new XtraReport();
            switch (getDataFromSql.reportname)
            {
                case "XtraRp_BaoCaoTonKho":
                    report= new XtraRp_BaoCaoTonKho();
                   XtraRp_BaoCaoTonKho(report, getDataFromSql, dtsource,stream);
                    break;
                case "XtraRp_PhieuNhapKho":
                    report= new XtraRp_PhieuNhapKho();
                    XtraRp_PhieuNhapKho(report, getDataFromSql, dtsource, stream);
                    break;
                case "Xtra_TheKhoHoaChatTheoMaHang":
                    report= new Xtra_TheKhoHoaChatTheoMaHang();
                    Xtra_TheKhoHoaChatTheoMaHang(report, getDataFromSql, dtsource, stream);
                    break;
                case "Xtra_TheKhoTheoMaHang":
                    report= new Xtra_TheKhoTheoMaHang();
                    Xtra_TheKhoTheoMaHang(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_BangKeTongHop":
                    report = new XtraRp_BangKeTongHop();
                    XtraRp_BangKeTongHop(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_PhieuXuatKho_KhongGia":
                    report = new XtraRp_PhieuXuatKho_KhongGia();
                    XtraRp_PhieuXuatKho_KhongGia(report, getDataFromSql, dtsource, stream);
                    break;
                case "XRp_UNC":
                    XRp_UNC(report, getDataFromSql, dtsource, stream);
                    break;
                //case "Xtra_InTemMayMoc":
                //    //report = new Xtra_InTemMayMoc();
                //    XtraRp_InTemHangHoa(report, getDataFromSql, dtsource, stream);
                //    break;
                case "Xtra_TheKhoHoaChatFIFO":
                    Xtra_TheKhoHoaChatTheoFIFO(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_UNCNgoaiTe":
                    report = new XtraRp_UNCNgoaiTe();
                    XtraRp_UNCNgoaiTe(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_PhieuNhanDang":
                    report = new XtraRp_PhieuNhanDang();
                    XtraRp_PhieuNhanDang(report, getDataFromSql, dtsource, stream);
                    break;
                default:
                   if(report!=null)
                    {
                        report.DataSource = dtsource;
                        report.ExportToPdf(stream);
                    }
                    break;
            }
            return true;
        }
        public bool GetReport(GetDataFromSql getDataFromSql, DataSet dtsource, MemoryStream stream)
        {

            var report = CreateReportWithName.CreateReportInstance(getDataFromSql.reportname);
            //if (report == null)
            //    return false;
            //XtraReport report = new XtraReport();
            switch (getDataFromSql.reportname)
            {
              
                case "XtraRp_BaoTriReportMayHu":
                    XtraRp_BaoTriReportMayHu(report, getDataFromSql, dtsource, stream);
                    break;
                default:
                    if (report != null)
                    {
                        report.DataSource = dtsource;
                        report.ExportToPdf(stream);
                    }
                    break;
            }
            return true;
        }
        public void XtraRp_BaoCaoTonKho(XtraReport report, GetDataFromSql classReport,DataTable dtsource, MemoryStream stream)
        {
            XtraRp_BaoCaoTonKho xtraRp_Bao = (XtraRp_BaoCaoTonKho)report;
            if (!string.IsNullOrEmpty(classReport.dtparameter))
            {
                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);
                if (dttmp.Rows.Count > 0)
                {
                    xtraRp_Bao.setGhiChu(dttmp.Rows[0]["GhiChu"].ToString());
                }
            }
            xtraRp_Bao.DataSource = dtsource;
            xtraRp_Bao.ExportToPdf(stream);
        }
        public void XtraRp_PhieuNhapKho(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_PhieuNhapKho xtraRp_PhieuNhapKho = (XtraRp_PhieuNhapKho)report;

            var querytotal = dtsource.Compute("sum(ThanhTien)", string.Empty);
            string bangchu = "";
            if (querytotal != null)
            {
                double d = double.Parse(querytotal.ToString());
                bangchu = string.Format("{0} đồng", prs.docsothapphan(d));
            }
            dtsource.Columns.Add("BangChu", typeof(string));
            dtsource.Rows[dtsource.Rows.Count - 1]["BangChu"] = bangchu;
            xtraRp_PhieuNhapKho.DataSource = dtsource;
            xtraRp_PhieuNhapKho.ExportToPdf(stream);
        }
        public void XtraRp_PhieuXuatKho_KhongGia(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_PhieuXuatKho_KhongGia xtraRp_PhieuNhapKho = (XtraRp_PhieuXuatKho_KhongGia)report;

            
            if(dtsource.Rows.Count>0)
            {
                //CheckTrung mã hàng để in subtong
                var query = dtsource.AsEnumerable().GroupBy(p => new { MaHang = p.Field<string>("MaHang"), TenHang = p.Field<string>("TenHang") })
                   .Select(g => new
                   {
                       MaHang = g.Key.MaHang,
                       TenHang = g.Key.TenHang,
                       SLNhap = g.Sum(p => p.Field<decimal>("SLNhap")),
                       Count=g.Count()

                   }).OrderBy(p => p.MaHang).ToList();
                var checkduplicate = query.Where(p => p.Count > 1).FirstOrDefault();
                XRSubreport xrqtitem = xtraRp_PhieuNhapKho.FindControl("xrSubreport1", true) as XRSubreport;
                if (checkduplicate != null)
                {

                    Xtra_PhieuXuatKho_Total xtraRp_DonDatHangItem = (Xtra_PhieuXuatKho_Total)xrqtitem.ReportSource;
                    xtraRp_DonDatHangItem.DataSource = query;
                }
                else
                    xrqtitem.Visible = false;
                //xtraRp_DonDatHangItem.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");



            }
           
            xtraRp_PhieuNhapKho.DataSource = dtsource;
            xtraRp_PhieuNhapKho.ExportToPdf(stream);
        }
        public void Xtra_TheKhoHoaChatTheoMaHang(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            Xtra_TheKhoHoaChatTheoMaHang xtra_TheKhoHoaChatTheoMaHang = (Xtra_TheKhoHoaChatTheoMaHang)report;
            if (!string.IsNullOrEmpty(classReport.dtparameter))
            {
                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                if (dttmp.Rows.Count > 0)
                {
                    ghichu += string.Format("{0}", dttmp.Rows[0]["TenHang"].ToString());
                    ghichu += Environment.NewLine + string.Format("{0}", dttmp.Rows[0]["TenKho"].ToString());
                    ghichu += Environment.NewLine + string.Format("{0}", dttmp.Rows[0]["Ngay"].ToString());
                    xtra_TheKhoHoaChatTheoMaHang.setGhiChu(ghichu);
                }
                

            }
            xtra_TheKhoHoaChatTheoMaHang.DataSource = dtsource;
            xtra_TheKhoHoaChatTheoMaHang.ExportToPdf(stream);

        }
        public void Xtra_TheKhoHoaChatTheoFIFO(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            Xtra_TheKhoHoaChatFIFO xtra_TheKhoHoaChatTheoMaHang = (Xtra_TheKhoHoaChatFIFO)report;
            if (!string.IsNullOrEmpty(classReport.dtparameter))
            {
                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                if (dttmp.Rows.Count > 0)
                {
                    ghichu = string.Format("{0}", dttmp.Rows[0]["Ngay"].ToString());
                  
                    //ghichu += Environment.NewLine + string.Format("{0}", dttmp.Rows[0]["Ngay"].ToString());
                    xtra_TheKhoHoaChatTheoMaHang.setGhiChu(ghichu, dttmp.Rows[0]["TenHang"].ToString(), dttmp.Rows[0]["DVT"].ToString());
                }


            }
            xtra_TheKhoHoaChatTheoMaHang.DataSource = dtsource;
            xtra_TheKhoHoaChatTheoMaHang.ExportToPdf(stream);

        }
        public void Xtra_TheKhoTheoMaHang(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            Xtra_TheKhoTheoMaHang xtra_TheKhoHoaChatTheoMaHang = (Xtra_TheKhoTheoMaHang)report;
            if (!string.IsNullOrEmpty(classReport.dtparameter))
            {
                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                if (dttmp.Rows.Count > 0)
                {
                    ghichu += string.Format("{0}", dttmp.Rows[0]["GhiChu"].ToString());
                   
                    xtra_TheKhoHoaChatTheoMaHang.setGhiChu(ghichu);
                }


            }
            if (dtsource.Rows.Count > 0)
            {

                xtra_TheKhoHoaChatTheoMaHang.setTonKho((decimal)(dtsource.Rows[dtsource.Rows.Count - 1]["SLTon"]), (decimal?)dtsource.Rows[dtsource.Rows.Count - 1]["TTTonKho"]);
            }
            xtra_TheKhoHoaChatTheoMaHang.DataSource = dtsource;
            xtra_TheKhoHoaChatTheoMaHang.ExportToPdf(stream);

        }

        public void XtraRp_BangKeTongHop(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            XtraRp_BangKeTongHop xtra_TheKhoHoaChatTheoMaHang = (XtraRp_BangKeTongHop)report;
            if (!string.IsNullOrEmpty(classReport.dtparameter))
            {
                DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                if (dttmp.Rows.Count > 0)
                {
                    ghichu += string.Format("{0}", dttmp.Rows[0]["GhiChu"].ToString());

                    xtra_TheKhoHoaChatTheoMaHang.setGhiChu(ghichu);
                }
            }
            xtra_TheKhoHoaChatTheoMaHang.DataSource = dtsource;
            xtra_TheKhoHoaChatTheoMaHang.ExportToPdf(stream);

        }
        public void XRp_UNC(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            if (dtsource.Rows.Count > 0)
            {
                DataRow row0 = dtsource.Rows[0];
                string nganhang = row0["NganHangTKNo"].ToString().ToLower();
                row0["BangChu"] = prs.FirstCharToUpper(prs.docsothapphan(double.Parse(row0["ThanhTien"].ToString())) + ((row0["DVT"].ToString() == "VND" || row0["DVT"].ToString() == "VNĐ") ? " đồng" : " ") + "./.");
                if (nganhang.Contains("vietcom"))
                {

                    XRp_UNCVietComBank xRp_UNCVietComBank = new XRp_UNCVietComBank();
                    xRp_UNCVietComBank.DataSource = dtsource;
                    xRp_UNCVietComBank.ExportToPdf(stream);
                    //prs.Showreport(xRp_UNCVietComBank);

                }
                if (nganhang.Contains("vietin"))
                {

                    XRp_UNCVietinBank xRp_UNCVietComBank = new XRp_UNCVietinBank();
                    xRp_UNCVietComBank.DataSource = dtsource;
                    xRp_UNCVietComBank.ExportToPdf(stream);
                    //prs.Showreport(xRp_UNCVietComBank);

                }
                if (nganhang.Contains("bidv"))
                {

                    XRp_UNCBIDVBank xRp_UNCVietComBank = new XRp_UNCBIDVBank();
                    xRp_UNCVietComBank.DataSource = dtsource;
                    xRp_UNCVietComBank.ExportToPdf(stream);
                    //prs.Showreport(xRp_UNCVietComBank);

                }
               
                //CheckTrung mã hàng để in subtong
            }

            
        }
      
        private void XtraRp_BaoTriReportMayHu(XtraReport report, GetDataFromSql classReport, DataSet dtsource, MemoryStream stream)
        {
            XtraRp_BaoTriReportMayHu xtraRp_PhieuNhapKho = (XtraRp_BaoTriReportMayHu)report;


            if (dtsource.Tables.Count > 0)
            {
                //CheckTrung mã hàng để in subtong
               
                XRSubreport xrqtitem = xtraRp_PhieuNhapKho.FindControl("xrSubreport1", true) as XRSubreport;


                XtraRp_BaoTri_ItemHeader xtraRp_BaoTri_ItemHeader = (XtraRp_BaoTri_ItemHeader)xrqtitem.ReportSource;
                xtraRp_BaoTri_ItemHeader.DataSource = dtsource.Tables[0];

                XRSubreport xrqtitem2 = xtraRp_PhieuNhapKho.FindControl("xrSubreport2", true) as XRSubreport;


                XtraRp_BaoTri_ItemHeader_ConTrong xtraRp_BaoTri_ItemHeader_ConTrong = (XtraRp_BaoTri_ItemHeader_ConTrong)xrqtitem2.ReportSource;
                xtraRp_BaoTri_ItemHeader_ConTrong.DataSource = dtsource.Tables[2];

                //xtraRp_DonDatHangItem.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
            }
            xtraRp_PhieuNhapKho.DataSource = dtsource.Tables[1];
            xtraRp_PhieuNhapKho.ExportToPdf(stream);

        }
        private void XtraRp_UNCNgoaiTe(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_UNCNgoaiTe xtraRp_PhieuNhapKho = (XtraRp_UNCNgoaiTe)report;
            if (dtsource.Rows.Count > 0)
            {
                //var queryUNCJson = JsonConvert.DeserializeObject<List<UyNhiemChi_NgoaiTeShow>>(json);
                string jsonUNC = dtsource.Rows[0]["JsonUNC"].ToString();

                    RemittanceRequest remittanceRequest = JsonConvert.DeserializeObject<RemittanceRequest>(jsonUNC);
                    //Chỉnh lại thành tiền theo ủy nhiệm chi gốc
                    //it.Amount.AmountInFigures = Convert.ToDecimal(uyNhiemChiShowcrr.ThanhTien);
                    //remittanceRequest.CopyFrom(it);
                    List<RemittanceRequest> lst = new List<RemittanceRequest>();
                    lst.Add(remittanceRequest);
                    XtraRp_UNCNgoaiTe xtraRp_UNCNgoaiTe = new XtraRp_UNCNgoaiTe();
                  
                    xtraRp_UNCNgoaiTe.SelectedDocumnet = remittanceRequest.SelectedDocumnet;
                    xtraRp_UNCNgoaiTe.DataSource = lst;
                    xtraRp_UNCNgoaiTe.ExportToPdf(stream);
                    lst.Clear();
                    //remittanceRequest = JsonConvert.DeserializeObject<RemittanceRequest>(queryUNCJson.FirstOrDefault().JsonUNC);
            }
          

        }
        private void XtraRp_PhieuNhanDang(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_PhieuNhanDang xtraRp_PhieuNhapKho = (XtraRp_PhieuNhanDang)report;
            if (dtsource.Rows.Count > 0)
            {
                
                //CheckTrung mã hàng để in subtong
                string json = "";
                var queryJson = dtsource.AsEnumerable().Where(p => p["Json"] != DBNull.Value).FirstOrDefault();
                if(queryJson!=null)
                {
                    json = queryJson["Json"].ToString();
                }

               
                
                XRSubreport xrqtitem = xtraRp_PhieuNhapKho.FindControl("xrSubreport1", true) as XRSubreport;
                //Tạo 1 Parameter MaChiTietInput để trong XtraRp_ChiTietItem để binding MaChiTiet vào
                xrqtitem.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("MaChiTietInput", null, "MaChiTiet"));
                if (!string.IsNullOrEmpty(json))
                {

                    DataTable dtitem = JsonConvert.DeserializeObject<DataTable>(json);
                    
                    XtraRp_ChiTietItem xtraRp_BaoTri_ItemHeader = (XtraRp_ChiTietItem)xrqtitem.ReportSource;
                    xtraRp_BaoTri_ItemHeader.FilterString = "[MaCT] = ?MaChiTietInput";//Binding vào subreport
                    xtraRp_BaoTri_ItemHeader.DataSource = dtitem;
                   
                }
                else
                {
                    xrqtitem.Visible = false;
                }
                var queryImg = dtsource.AsEnumerable().Where(p => p["ImageChiTiet"] != DBNull.Value).ToList();

                foreach (var it in queryImg)
                {
                    var queryMaCT = dtsource.Select(string.Format("MaChiTiet='{0}'", it.Field<string>("MaChiTiet")));
                    foreach (DataRow row2 in queryMaCT)
                    {
                        if(row2["ImageChiTiet"]==DBNull.Value)
                            row2["ImageChiTiet"] = it["ImageChiTiet"];
                    }


                }
                //xtraRp_DonDatHangItem.setGhiChu(dtmaster.Rows[0]["GhiChu"].ToString(), "");
            }
            xtraRp_PhieuNhapKho.DataSource = dtsource;
            xtraRp_PhieuNhapKho.ExportToPdf(stream);
            xtraRp_PhieuNhapKho.Dispose();
        }
    }
}
