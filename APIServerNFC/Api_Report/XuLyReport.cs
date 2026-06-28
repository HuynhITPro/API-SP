using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static APIServerNFC.Api_Report.ClassReportAllcs;




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
                case "XtraRp_BangKeTongHop_NCC":
                 report = new XtraRp_BangKeTongHop_NCC();
                    XtraRp_BangKeTongHop_NCC(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_PhieuXuatKho_KhongGia":
                    report = new XtraRp_PhieuXuatKho_KhongGia();
                    XtraRp_PhieuXuatKho_KhongGia(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_PhieuXuatKho_KhongGiaIway":
                    report = new XtraRp_PhieuXuatKho_KhongGiaIway();
                    XtraRp_PhieuXuatKho_KhongGiaIway(report, getDataFromSql, dtsource, stream);
                    break;
                case "XRp_UNC":
                    XRp_UNC(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_BienBanGiaoNhanMaster":
                    report = new XtraRp_BienBanGiaoNhanMaster();
                    XtraRp_BienBanGiaoNhanMaster(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_BienBanGiaoNhanMaster_NotPrice":
                 report = new XtraRp_BienBanGiaoNhanMaster_NotPrice();
                    XtraRp_BienBanGiaoNhanMaster_NotPrice(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_PhieuGiaoNhanNoiBo":
                    //report = new Xtra_InTemMayMoc();
                    XtraRp_PhieuGiaoNhanNoiBo(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_CongDoanChiTiet":
                    //report = new Xtra_InTemMayMoc();
                    XtraRp_CongDoanChiTiet(report, getDataFromSql, dtsource, stream);
                    break;
                case "XtraRp_GiaoNhanNoiBo_BangKe":
                    //report = new Xtra_InTemMayMoc();
                    XtraRp_GiaoNhanNoiBo_BangKe(report, getDataFromSql, dtsource, stream);
                    break;
              
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
                case "XtraRp_PhieuGiaoHang":
                    report = new XtraRp_PhieuGiaoHang();
                    XtraRp_PhieuGiaoHang(report, getDataFromSql, dtsource, stream);
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
                case "XtraRp_PhieuGiaoNhanNoiBo_TonKho":
                    report = new XtraRp_PhieuGiaoNhanNoiBo_TonKho();
                    XtraRp_PhieuGiaoNhanNoiBo_TonKho(report, getDataFromSql, dtsource, stream);
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
        public void XtraRp_PhieuGiaoHang(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_PhieuGiaoHang xtraRp_PhieuNhapKho = (XtraRp_PhieuGiaoHang)report;

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
        public void XtraRp_PhieuXuatKho_KhongGiaIway(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            XtraRp_PhieuXuatKho_KhongGiaIway xtraRp_PhieuNhapKho = (XtraRp_PhieuXuatKho_KhongGiaIway)report;


            if (dtsource.Rows.Count > 0)
            {
                //CheckTrung mã hàng để in subtong
                var query = dtsource.AsEnumerable().GroupBy(p => new { MaHang = p.Field<string>("MaHang"), TenHang = p.Field<string>("TenHang") })
                   .Select(g => new
                   {
                       MaHang = g.Key.MaHang,
                       TenHang = g.Key.TenHang,
                       SLNhap = g.Sum(p => p.Field<decimal>("SLNhap")),
                       Count = g.Count()

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
        public void XtraRp_BangKeTongHop_NCC(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            XtraRp_BangKeTongHop_NCC xtra_TheKhoHoaChatTheoMaHang = (XtraRp_BangKeTongHop_NCC)report;
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
        public void XtraRp_PhieuGiaoNhanNoiBo(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            XtraRp_PhieuGiaoNhanNoiBo xtra_TheKhoHoaChatTheoMaHang = (XtraRp_PhieuGiaoNhanNoiBo)report;
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
        public void XtraRp_GiaoNhanNoiBo_BangKe(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            XtraRp_GiaoNhanNoiBo_BangKe xtra_TheKhoHoaChatTheoMaHang = (XtraRp_GiaoNhanNoiBo_BangKe)report;
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
        public void XtraRp_CongDoanChiTiet(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            string ghichu = "";
            XtraRp_CongDoanChiTiet xtra_TheKhoHoaChatTheoMaHang = (XtraRp_CongDoanChiTiet)report;
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
        public void XtraRp_PhieuGiaoNhanNoiBo_TonKho(XtraReport report, GetDataFromSql classReport, DataSet dtsource, MemoryStream stream)
        {
            string ghichu = "";
           
            int countcolumn=dtsource.Tables[0].Columns.Count;

            //Xử lý gán dữ liệu mã màu vào từng cột 
            XuLyTonKhoGiaoNhan xuLyTonKhoGiaoNhan = new XuLyTonKhoGiaoNhan();
            xuLyTonKhoGiaoNhan.XuLyBang(dtsource);
            //List<string> lstaddfield = new List<string>();
            //for(int i=countcolumn;i< dtsource.Tables[0].Columns.Count; i++)
            //{
            //    string columnname = dtsource.Tables[0].Columns[i].ColumnName;
            //    lstaddfield.Add(columnname);

            //}
            //xtraRp_PhieuGiaoNhanNoiBo_TonKho.InitColumn(lstaddfield);
            var maxmau =  xuLyTonKhoGiaoNhan.querymau
                    .Select(p => p.Index)
                    .DefaultIfEmpty(0)
                    .Max();//Lấy số lượng màu của sản phẩm có nhiều màu nhất để tạo cột
            if (maxmau==0)
            {
                XtraRp_PhieuGiaoNhanNoiBo_TonKho_OneColor xtraRp_PhieuGiaoNhanNoiBo_TonKho = new XtraRp_PhieuGiaoNhanNoiBo_TonKho_OneColor();
                if (!string.IsNullOrEmpty(classReport.dtparameter))
                {
                    DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                    if (dttmp.Rows.Count > 0)
                    {
                        ghichu += string.Format("{0}", dttmp.Rows[0]["GhiChu"].ToString());

                        xtraRp_PhieuGiaoNhanNoiBo_TonKho.setGhiChu(ghichu);
                    }
                }
                //xtraRp_PhieuGiaoNhanNoiBo_TonKho.InitDynamicColors(xuLyTonKhoGiaoNhan.querymau);

                xtraRp_PhieuGiaoNhanNoiBo_TonKho.DataSource = dtsource.Tables[0];
                //Xử lý màu
                //XuLyTonKhoGiaoNhan.querymau.Clear();
                xtraRp_PhieuGiaoNhanNoiBo_TonKho.ExportToPdf(stream);
            }
            else
            {
                XtraRp_PhieuGiaoNhanNoiBo_TonKho xtraRp_PhieuGiaoNhanNoiBo_TonKho =new XtraRp_PhieuGiaoNhanNoiBo_TonKho();
                if (!string.IsNullOrEmpty(classReport.dtparameter))
                {
                    DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                    if (dttmp.Rows.Count > 0)
                    {
                        ghichu += string.Format("{0}", dttmp.Rows[0]["GhiChu"].ToString());

                        xtraRp_PhieuGiaoNhanNoiBo_TonKho.setGhiChu(ghichu);
                    }
                }
                xtraRp_PhieuGiaoNhanNoiBo_TonKho.InitDynamicColors(xuLyTonKhoGiaoNhan.querymau);

                xtraRp_PhieuGiaoNhanNoiBo_TonKho.DataSource = dtsource.Tables[0];
                //Xử lý màu
                //XuLyTonKhoGiaoNhan.querymau.Clear();
                xtraRp_PhieuGiaoNhanNoiBo_TonKho.ExportToPdf(stream);
               
            }
            xuLyTonKhoGiaoNhan.querymau.Clear();

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

                XRSubreport xrqtitemcongdoan = xtraRp_PhieuNhapKho.FindControl("xrSubreport2", true) as XRSubreport;
                //Tạo 1 Parameter MaChiTietInput để trong XtraRp_ChiTietItem để binding MaChiTiet vào
                xrqtitemcongdoan.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("Barcode", null, "Serial"));

                string jsoncongdoan = "";
                var queryJsoncongdoan = dtsource.AsEnumerable().Where(p => p["JsonCongDoan"] != DBNull.Value).FirstOrDefault();
                if (queryJsoncongdoan != null)
                {
                    jsoncongdoan = queryJsoncongdoan["JsonCongDoan"].ToString();
                }
                if (!string.IsNullOrEmpty(jsoncongdoan))
                {

                    DataTable dtitemcongdoan = JsonConvert.DeserializeObject<DataTable>(jsoncongdoan);
                    XtraRp_CongDoanItem xtraRp_BaoTri_ItemHeader = (XtraRp_CongDoanItem)xrqtitemcongdoan.ReportSource;
                    xrqtitemcongdoan.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("Barcode", null, "Serial"));
                    //xtraRp_BaoTri_ItemHeader.FilterString = "[MaCT] = ?MaChiTietInput";//Binding vào subreport
                    xtraRp_BaoTri_ItemHeader.FilterString = "[Barcode] = ?Barcode";
                    xtraRp_BaoTri_ItemHeader.DataSource = dtitemcongdoan;

                }
                else
                {
                    xrqtitemcongdoan.Visible = false;
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
        public void XtraRp_BienBanGiaoNhanMaster(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            using (XtraRp_BienBanGiaoNhanMaster xtraRp_PhieuNhapKho = (XtraRp_BienBanGiaoNhanMaster)report)
            {
                try
                {

                    string json = "";
                    if (dtsource.Rows.Count > 0)
                    {
                        json = dtsource.Rows[0]["JsonDescription"].ToString();
                    }
                    if (!string.IsNullOrEmpty(json))
                    {

                        ModelBienBan modelBienBan = JsonConvert.DeserializeObject<ModelBienBan>(json);
                        XRSubreport xrqtitembengiao = xtraRp_PhieuNhapKho.FindControl("xrSubreport1", true) as XRSubreport;


                        XtraRp_BienBanGiaoNhan_BenGiao rp_BienBanGiaoNhan_BenGiao = (XtraRp_BienBanGiaoNhan_BenGiao)xrqtitembengiao.ReportSource;
                        rp_BienBanGiaoNhan_BenGiao.DataSource = modelBienBan.ListBenGiao;

                        XRSubreport xrqtitembennhan = xtraRp_PhieuNhapKho.FindControl("xrSubreport2", true) as XRSubreport;


                        XtraRp_BienBanGiaoNhan_BenNhan rp_BienBanGiaoNhan_BenNhan = (XtraRp_BienBanGiaoNhan_BenNhan)xrqtitembennhan.ReportSource;
                        rp_BienBanGiaoNhan_BenNhan.DataSource = modelBienBan.ListBenNhan;


                        XRSubreport xrqtitemthongtinchung = xtraRp_PhieuNhapKho.FindControl("xrSubreport3", true) as XRSubreport;
                        XtraRp_NvlNhapXuat_ChiPhiKhac rp_NvlNhapXuat_ChiPhiKhac = (XtraRp_NvlNhapXuat_ChiPhiKhac)xrqtitemthongtinchung.ReportSource;
                        List<NvlNhapXuat_PhatSinhKhacShow> lstchiphikhac = new List<NvlNhapXuat_PhatSinhKhacShow>();
                        lstchiphikhac.AddRange(modelBienBan.LstChiPhiKhac);
                        var querysource = lstchiphikhac
                   .Where(x => x.TypeName != "ThanhTienHoaDon");

                        if (querysource != null)
                        {
                            rp_NvlNhapXuat_ChiPhiKhac.DataSource = querysource;
                        }
                        else
                            rp_NvlNhapXuat_ChiPhiKhac.DataSource = lstchiphikhac;
                       var querythanhtien = lstchiphikhac.Where(p => p.TypeName == "ThanhTienHoaDon").FirstOrDefault();

                        decimal? thanhtien = 0;
                        string bangchu = "";
                        if (querythanhtien != null)
                        {
                            if (querythanhtien.ThanhTien !=null)
                            {
                                thanhtien = querythanhtien.ThanhTien;
                                bangchu = string.Format("{0} đồng", prs.docsothapphan(Convert.ToDouble(thanhtien)));
                            }

                        }
                        //if (dtnoidung.Rows.Count > 0)
                        //{
                        //    var querytotal = dtnoidung.Compute("sum(ThanhTien)", string.Empty);

                        //    if (querytotal != null)
                        //    {
                        //        double d = double.Parse(querytotal.ToString());
                        //        bangchu = string.Format("{0} đồng", prs.docsothapphan(d));
                        //    }
                        //}

                        rp_NvlNhapXuat_ChiPhiKhac.setSoTien(thanhtien, bangchu);

                       // rp_NvlNhapXuat_ChiPhiKhac.DataSource = lstchiphikhac;

                       // XtraRp_BienBanGiaoNhan_BenNhan rp_BienBanGiaoNhan_BenNhan = (XtraRp_BienBanGiaoNhan_BenNhan)xrqtitembennhan.ReportSource;

                        if (!string.IsNullOrEmpty(classReport.dtparameter))
                        {
                            DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                            if (dttmp.Rows.Count > 0)
                            {
                                string JsonFooter = "";
                                string JsonHeader = "";
                                if (dttmp.Columns.Contains("JsonFooter"))
                                {
                                    if (dttmp.Rows[0]["JsonFooter"] != DBNull.Value && dttmp.Rows[0]["JsonFooter"] != null)
                                    {
                                        JsonFooter = dttmp.Rows[0]["JsonFooter"].ToString();
                                    }
                                }
                                if (dttmp.Columns.Contains("JsonHeader"))
                                {
                                    if (dttmp.Rows[0]["JsonHeader"] != DBNull.Value && dttmp.Rows[0]["JsonHeader"] != null)
                                    {
                                        JsonHeader = dttmp.Rows[0]["JsonHeader"].ToString();
                                    }
                                }
                                xtraRp_PhieuNhapKho.setGhiChu(modelBienBan.ThongTinChung.GhiChu, modelBienBan.ThongTinChung.NgayGioBienBan);


                                xtraRp_PhieuNhapKho.XuLyHeader(JsonHeader);
                                if (modelBienBan.ListNguoiKy != null)
                                {
                                    if (modelBienBan.ListNguoiKy.Count > 0)
                                    {
                                        List<JsonFooter> lst = modelBienBan.ListNguoiKy.Select(p => new JsonFooter { HoVaTen = p.HoTen, TenBoPhan = p.TieuDe }).ToList();
                                        JsonFooter = JsonConvert.SerializeObject(lst);
                                    }
                                }
                                xtraRp_PhieuNhapKho.XuLyFooter(JsonFooter);
                            }
                        }

                        //dtsource.Columns.Add("BangChu", typeof(string));
                        //dtsource.Rows[dtsource.Rows.Count - 1]["BangChu"] = bangchu;
                        xtraRp_PhieuNhapKho.DataSource = modelBienBan.ListHangHoa;
                        xtraRp_PhieuNhapKho.ExportToPdf(stream);
                        dtsource.Dispose();

                    }
                    else
                    {
                        Err("Không có dữ liệu để in", stream);
                    }
                }
                catch (Exception ex)
                {
                    Err("Lỗi:" + ex.Message, stream);
                }


            }
        }
        public void XtraRp_BienBanGiaoNhanMaster_NotPrice(XtraReport report, GetDataFromSql classReport, DataTable dtsource, MemoryStream stream)
        {
            using (XtraRp_BienBanGiaoNhanMaster_NotPrice xtraRp_PhieuNhapKho = (XtraRp_BienBanGiaoNhanMaster_NotPrice)report)
            {
                try
                {

                    string json = "";
                    if (dtsource.Rows.Count > 0)
                    {
                        json = dtsource.Rows[0]["JsonDescription"].ToString();
                    }
                    if (!string.IsNullOrEmpty(json))
                    {

                        ModelBienBan modelBienBan = JsonConvert.DeserializeObject<ModelBienBan>(json);
                        XRSubreport xrqtitembengiao = xtraRp_PhieuNhapKho.FindControl("xrSubreport1", true) as XRSubreport;


                        XtraRp_BienBanGiaoNhan_BenGiao rp_BienBanGiaoNhan_BenGiao = (XtraRp_BienBanGiaoNhan_BenGiao)xrqtitembengiao.ReportSource;
                        rp_BienBanGiaoNhan_BenGiao.DataSource = modelBienBan.ListBenGiao;

                        XRSubreport xrqtitembennhan = xtraRp_PhieuNhapKho.FindControl("xrSubreport2", true) as XRSubreport;


                        XtraRp_BienBanGiaoNhan_BenNhan rp_BienBanGiaoNhan_BenNhan = (XtraRp_BienBanGiaoNhan_BenNhan)xrqtitembennhan.ReportSource;
                        rp_BienBanGiaoNhan_BenNhan.DataSource = modelBienBan.ListBenNhan;


                        XRSubreport xrqtitemthongtinchung = xtraRp_PhieuNhapKho.FindControl("xrSubreport3", true) as XRSubreport;
                        XtraRp_NvlNhapXuat_ChiPhiKhac rp_NvlNhapXuat_ChiPhiKhac = (XtraRp_NvlNhapXuat_ChiPhiKhac)xrqtitemthongtinchung.ReportSource;
                        List<NvlNhapXuat_PhatSinhKhacShow> lstchiphikhac = new List<NvlNhapXuat_PhatSinhKhacShow>();
                        lstchiphikhac.AddRange(modelBienBan.LstChiPhiKhac);
                        var querysource = lstchiphikhac
                   .Where(x => x.TypeName != "ThanhTienHoaDon");

                        if (querysource != null)
                        {
                            rp_NvlNhapXuat_ChiPhiKhac.DataSource = querysource;
                        }
                        else
                            rp_NvlNhapXuat_ChiPhiKhac.DataSource = lstchiphikhac;
                        var querythanhtien = lstchiphikhac.Where(p => p.TypeName == "ThanhTienHoaDon").FirstOrDefault();

                        decimal? thanhtien = 0;
                        string bangchu = "";
                        if (querythanhtien != null)
                        {
                            if (querythanhtien.ThanhTien != null)
                            {
                                thanhtien = querythanhtien.ThanhTien;
                                bangchu = string.Format("{0} đồng", prs.docsothapphan(Convert.ToDouble(thanhtien)));
                            }

                        }
                        //if (dtnoidung.Rows.Count > 0)
                        //{
                        //    var querytotal = dtnoidung.Compute("sum(ThanhTien)", string.Empty);

                        //    if (querytotal != null)
                        //    {
                        //        double d = double.Parse(querytotal.ToString());
                        //        bangchu = string.Format("{0} đồng", prs.docsothapphan(d));
                        //    }
                        //}

                        rp_NvlNhapXuat_ChiPhiKhac.setSoTien(thanhtien, bangchu);

                        // rp_NvlNhapXuat_ChiPhiKhac.DataSource = lstchiphikhac;

                        // XtraRp_BienBanGiaoNhan_BenNhan rp_BienBanGiaoNhan_BenNhan = (XtraRp_BienBanGiaoNhan_BenNhan)xrqtitembennhan.ReportSource;

                        if (!string.IsNullOrEmpty(classReport.dtparameter))
                        {
                            DataTable dttmp = JsonConvert.DeserializeObject<DataTable>(classReport.dtparameter);

                            if (dttmp.Rows.Count > 0)
                            {
                                string JsonFooter = "";
                                string JsonHeader = "";
                                if (dttmp.Columns.Contains("JsonFooter"))
                                {
                                    if (dttmp.Rows[0]["JsonFooter"] != DBNull.Value && dttmp.Rows[0]["JsonFooter"] != null)
                                    {
                                        JsonFooter = dttmp.Rows[0]["JsonFooter"].ToString();
                                    }
                                }
                                if (dttmp.Columns.Contains("JsonHeader"))
                                {
                                    if (dttmp.Rows[0]["JsonHeader"] != DBNull.Value && dttmp.Rows[0]["JsonHeader"] != null)
                                    {
                                        JsonHeader = dttmp.Rows[0]["JsonHeader"].ToString();
                                    }
                                }
                                xtraRp_PhieuNhapKho.setGhiChu(modelBienBan.ThongTinChung.GhiChu, modelBienBan.ThongTinChung.NgayGioBienBan);


                                xtraRp_PhieuNhapKho.XuLyHeader(JsonHeader);
                                if (modelBienBan.ListNguoiKy != null)
                                {
                                    if (modelBienBan.ListNguoiKy.Count > 0)
                                    {
                                        List<JsonFooter> lst = modelBienBan.ListNguoiKy.Select(p => new JsonFooter { HoVaTen = p.HoTen, TenBoPhan = p.TieuDe }).ToList();
                                        JsonFooter = JsonConvert.SerializeObject(lst);
                                    }
                                }
                                xtraRp_PhieuNhapKho.XuLyFooter(JsonFooter);
                            }
                        }

                        //dtsource.Columns.Add("BangChu", typeof(string));
                        //dtsource.Rows[dtsource.Rows.Count - 1]["BangChu"] = bangchu;
                        xtraRp_PhieuNhapKho.DataSource = modelBienBan.ListHangHoa;
                        xtraRp_PhieuNhapKho.ExportToPdf(stream);
                        dtsource.Dispose();

                    }
                    else
                    {
                        Err("Không có dữ liệu để in", stream);
                    }
                }
                catch (Exception ex)
                {
                    Err("Lỗi:" + ex.Message, stream);
                }


            }
        }
        public void Err(string Err, MemoryStream stream)
        {
            using (XtraRp_Err xtraRp_Bao = new XtraRp_Err())
            {
                xtraRp_Bao.setErr(Err);
                xtraRp_Bao.ExportToPdf(stream);
                //dtsource.Dispose();
            }
        }
    }
}
