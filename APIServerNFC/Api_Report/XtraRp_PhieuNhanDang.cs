using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_PhieuNhanDang : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_PhieuNhanDang()
        {
            InitializeComponent();
           
        }
        string MaCT = "";
        private void xrSubreport1_BeforePrint(object sender, CancelEventArgs e)
        {
            XRSubreport sub = (XRSubreport)sender;

            // Lấy data source của subreport
            var dt = ((XtraReport)sub.ReportSource).DataSource as DataTable;
            if(dt!=null)
            {
                if (dt.Select(string.Format("MaCT='{0}'", MaCT)).FirstOrDefault() != null)
                {
                    sub.Visible = true;
                }
                else
                {
                    sub.Visible = false;
                }
            }
            else
            {
                sub.Visible = false;
            }
           
        }

        private void xrTableCell20_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell xRTableCell = (XRTableCell)sender;
           MaCT= xRTableCell.Text;
        }
    }
}
