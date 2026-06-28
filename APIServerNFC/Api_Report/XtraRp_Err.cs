using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_Err : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_Err()
        {
            InitializeComponent();
        }
        public void setErr(string err)
        {
            lbErr.Text = err;
        }
    }
}
