using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_GiaoNhanNoiBo_BangKe : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_GiaoNhanNoiBo_BangKe()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghichu)
        {
            this.lbGhiChu.Text = ghichu;
        }
    }
}
