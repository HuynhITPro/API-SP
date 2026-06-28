using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_BienBanGiaoNhanMaster_NotPrice : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_BienBanGiaoNhanMaster_NotPrice()
        {
            InitializeComponent();
           
            this.xrSubreport2.ReportSource = new Api_Report.XtraRp_BienBanGiaoNhan_BenNhan();
            this.xrSubreport1.ReportSource = new Api_Report.XtraRp_BienBanGiaoNhan_BenGiao();
            this.xrSubreport3.ReportSource = new Api_Report.XtraRp_NvlNhapXuat_ChiPhiKhac();
        }
        public void XuLyHeader(string? JsonHeader)
        {
            if (JsonHeader == null)
                return;
            var Listheader = JsonConvert.DeserializeObject<JsonHeader>(JsonHeader);
            if (Listheader == null)
            {
                return;
            }

            if (Listheader == null)
                return;
            xrTenCty.Text = Listheader.TenNCC;
            xrTenChiNhanh.Text = Listheader.DiaChi;
            //if (!string.IsNullOrEmpty(Listheader.MauSo))
            //{
            //    xrMauSo.Text = string.Format("Mẫu số: {0}", Listheader.MauSo);
            //}
            //if (!string.IsNullOrEmpty(Listheader.LanSoatXet))
            //{
            //    xrLanBanHanh.Text = string.Format("Lần soát xét: {0}", Listheader.LanSoatXet);
            //}
            //if (Listheader.NgayHieuLuc != null)
            //{
            //    xrNgayHieuLuc.Text = string.Format("Ngày HL: {0}", Listheader.NgayHieuLuc.Value.ToString("dd/MM/yy"));
            //}
        }
        public void XuLyFooter(string? JsonFooter)
        {
            
            if (JsonFooter == null)
                return;
            var Listfooter = JsonConvert.DeserializeObject<List<JsonFooter>>(JsonFooter);
            if (Listfooter == null)
            {
                return;
            }
            //Xử lý chiều rộng cột của 1 cell
            int n = Listfooter.Count;
            float widthtable = 1880F;
            if (n == 0)
                return;
            float d_one = widthtable / n;

            DevExpress.XtraReports.UI.XRTable xrTable_Footer = new DevExpress.XtraReports.UI.XRTable();

            ((System.ComponentModel.ISupportInitialize)(xrTable_Footer)).BeginInit();
            DevExpress.XtraReports.UI.XRTableRow xrRowName = new XRTableRow();
            DevExpress.XtraReports.UI.XRTableRow xrRowTitle = new XRTableRow();
            DevExpress.XtraReports.UI.XRTableRow xrTableRow_HoTen = new XRTableRow();

            xrTable_Footer.SizeF = new System.Drawing.SizeF(widthtable, 330F);

            xrRowName.Dpi = 254F;
            xrRowName.Name = "xrRowName";
            xrRowName.Weight = 1D;

            xrRowTitle.Dpi = 254F;
            xrRowTitle.Name = "xrRowTitle";
            xrRowTitle.Weight = 1D;

            xrTableRow_HoTen.Dpi = 254F;
            xrTableRow_HoTen.Name = "xrTableRow_hoten";
            xrTableRow_HoTen.Weight = 3.7592590812623032D;



            xrTable_Footer.Dpi = 254F;
            xrTable_Footer.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F);
            xrTable_Footer.LocationFloat = new DevExpress.Utils.PointFloat(54.20632F, 0F);
            xrTable_Footer.Name = "xrTable_Footer";
            xrTable_Footer.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            int i = 0;
            foreach (var item in Listfooter)
            {
                DevExpress.XtraReports.UI.XRTableCell xrcell = new DevExpress.XtraReports.UI.XRTableCell();
                DevExpress.XtraReports.UI.XRTableCell xrcelltitle = new DevExpress.XtraReports.UI.XRTableCell();
                DevExpress.XtraReports.UI.XRTableCell xrcellhoten = new DevExpress.XtraReports.UI.XRTableCell();
                xrcell.Dpi = 254F;
                xrcell.Multiline = true;
                xrcell.Name = string.Format("TenBoPhan_{0}", i);
                xrcell.Text = item.TenBoPhan;
                xrcell.Weight = 1D;
                xrRowName.Cells.Add(xrcell);
                xrcelltitle.Dpi = 254F;
                xrcelltitle.Multiline = true;
                xrcelltitle.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F, DevExpress.Drawing.DXFontStyle.Italic);
                xrcelltitle.Name = string.Format("Title_{0}", i);
                xrcelltitle.Text = "(Ký và ghi rõ họ tên)";
                xrcelltitle.Weight = 1D;
                xrRowTitle.Cells.Add(xrcelltitle);

                xrcellhoten.Dpi = 254F;
                xrcellhoten.Multiline = true;
                xrcellhoten.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F, DevExpress.Drawing.DXFontStyle.Italic);
                xrcellhoten.Name = string.Format("Title_{0}", i);
                if (item.TenBoPhan.Contains("Lập phiếu"))
                {
                    xrcellhoten.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                    new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TenUser]")});
                }
                else
                    xrcellhoten.Text = item.HoVaTen;
                xrcellhoten.Weight = 1D;
                xrcellhoten.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
                xrTableRow_HoTen.Cells.Add(xrcellhoten);

                i++;
            }

            xrTable_Footer.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            xrRowName,xrRowTitle,xrTableRow_HoTen
            });

            xrTable_Footer.StylePriority.UseFont = false;
            xrTable_Footer.StylePriority.UseTextAlignment = false;
            xrTable_Footer.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;


            SubBand_Footer.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {

            xrTable_Footer});
            SubBand_Footer.Dpi = 254F;
            SubBand_Footer.HeightF = 317.9537F;
            SubBand_Footer.Name = "SubBand_Footer";
            ((System.ComponentModel.ISupportInitialize)(xrTable_Footer)).EndInit();
        }

        private void xrRichTextGhiChu_BeforePrint(object sender, CancelEventArgs e)
        {
            //           string text = $@"<div style=""""font-family: Times New Roman; font-size:12pt; line-height:1.7;"""">
            //                <p style=""""text-align:left;"""">Duy trì dịch vụ</p>
            //                 <p style=""""text-align:left;"""">✓ Phát hành hóa đơn điện tử từng lần.</p>    
            //<p style=""""text-align:left;"""">✓ Lưu trữ dữ liệu ghi nhận log bơm và phát hành hóa đơn từng lần theo quy định pháp luật.</p>                
            //<p style=""""text-align:left;"""">✓ Quản lý cửa hàng xăng dầu</p>                
            //               </div>";
            //xrRichTextGhiChu.Html = text;
        }
        public string setGhiChu(string GhiChu, string NgayGioBienBan)
        {
            var ghiChuHtml = (GhiChu ?? "")
                .Replace("\r\n", "<br>")   // Windows line ending
                .Replace("\r", "<br>")     // Mac line ending cũ  
                .Replace("\n", "<br>");    // Unix line ending
            string text = $@"
        <div style='
            font-family: Times New Roman;
            font-size: 12pt;
            line-height: 1.6;
           '><p>{ghiChuHtml}</p></div>";
            xrRichTextGhiChu.Html = text;
            lbNgayGioBienBan.Text = NgayGioBienBan;
            return text;
        }

        int rowcount = 0;
        public bool checkHideHeader = true;
        private void PageHeader_BeforePrint(object sender, CancelEventArgs e)
        {
            if ((rowcount >= this.RowCount && this.RowCount > 1))
            {
                e.Cancel = checkHideHeader;

            }
        }

        private void Detail_BeforePrint(object sender, CancelEventArgs e)
        {
          
        }

        private void Detail_AfterPrint(object sender, EventArgs e)
        {
            rowcount++;
        }
    }
}
