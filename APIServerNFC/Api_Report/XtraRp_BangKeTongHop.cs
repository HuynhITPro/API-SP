using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_BangKeTongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_BangKeTongHop()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghiChu)
        {
            lbGhiChu.Text = ghiChu;
        }
        string previousValue = string.Empty;
        bool visiblemearge = true;
        private void xrTableCell10_BeforePrint(object sender, CancelEventArgs e)
        {

            var cell = sender as XRTableCell;
            //Console.WriteLine(cell.Text);
            if (cell != null)
            {
                if (cell.Text == previousValue)
                {
                    cell.Visible = false; // Dòng này có thể gây ẩn nhầm
                    visiblemearge = false;
                }
                else
                {
                    cell.Visible = true;
                    visiblemearge = true;

                }

            }

            //Console.WriteLine(string.Format("Cũ {0}, mới {1}", previousValue, cell.Text));
            previousValue = cell.Text;

        }

        private void xrTableCell11_BeforePrint(object sender, CancelEventArgs e)
        {
            var cell = sender as XRTableCell;
            //Console.WriteLine(cell.Text);
            cell.Visible = visiblemearge;

            //Console.WriteLine(string.Format("Cũ {0}, mới {1}", previousValue, cell.Text));
            //previousValue = cell.Text;
        }
    }
}
