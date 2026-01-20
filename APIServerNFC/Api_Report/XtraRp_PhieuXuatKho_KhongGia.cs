using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_PhieuXuatKho_KhongGia : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_PhieuXuatKho_KhongGia()
        {
            InitializeComponent();
        }

        private void xrLabel1_BeforePrint(object sender, CancelEventArgs e)
        {
            var cell = sender as XRLabel;
            //Console.WriteLine(cell.Text);
            if (cell != null)
            {
                cell.Text = cell.Text.Replace("zzzz.", "");
            }
            if(cell.Text.Contains("Cấn trừ nợ cũ"))
            {
                cell.Text = "Nợ cũ được cấn trừ";
            }
        }

      
    }
}
