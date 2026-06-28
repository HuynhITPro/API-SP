using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_NvlNhapXuat_ChiPhiKhac : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_NvlNhapXuat_ChiPhiKhac()
        {
            InitializeComponent();
        }
        public void setSoTien(decimal? thanhtien, string text)
        {
            if (thanhtien == null)
            {
                return;
            }

            xrThanhTienHoaDon.Text = thanhtien.Value.ToString("#,#.#");

            xrBangChu.Text = text;
        }
    }
}
