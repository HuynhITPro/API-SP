using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class Xtra_TheKhoHoaChatFIFO : DevExpress.XtraReports.UI.XtraReport
    {
        public Xtra_TheKhoHoaChatFIFO()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghichu, string tenhang, string dvt)
        {
            xrGhiChu.Text = ghichu;
            xrDVT.Text = dvt;
            xrtenhang.Text = tenhang;
        }
    }
}
