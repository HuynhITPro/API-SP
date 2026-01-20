using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class Xtra_TheKhoHoaChatTheoMaHang : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_TheKhoHoaChatTheoMaHang()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghichu)
        {
            lbghichu.Text = ghichu;
        }
        public void setTonKho(double slton, double? thanhtienton)
        {
            lbTonCuoi.Text = slton.ToString("#,#.##");
            if (thanhtienton != null)
            {
                lbThanhTien.Text = thanhtienton.Value.ToString("#,#");
            }

        }

        private void xrThanhTienContent_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell xRTableCell = (XRTableCell)sender;
            string s = xRTableCell.Text;
            if (s.Length > 4)
            {
                xRTableCell.Text = s.Substring(0, 4);
            }
        }
    }
}
