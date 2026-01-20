using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace APIServerNFC.Api_Report
{
    public partial class XRp_UNCVietComBank : DevExpress.XtraReports.UI.XtraReport
    {
        public XRp_UNCVietComBank()
        {
            InitializeComponent();
        }

        private void xrCheckBox1_BeforePrint(object sender, CancelEventArgs e)
        {
            object ob = xrCheckBox1.Value;
            if ((bool)(xrCheckBox1.Value) == true)
            {
                xrCheckBox1.Text = "";
                xrCheckBox1.Checked = true;
            }
        }

        private void xrTableCell14_BeforePrint(object sender, CancelEventArgs e)
        {
            var cell = sender as XRTableCell;
            if (cell != null && decimal.TryParse(cell.Text, out decimal value))
            {
                var viVN = new CultureInfo("vi-VN");
                cell.Text = value.ToString("#,0.##", viVN); // "1.234.567,89"
            }
        }
    }
}
