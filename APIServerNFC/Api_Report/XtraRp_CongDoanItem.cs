using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_CongDoanItem : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_CongDoanItem()
        {
            InitializeComponent();
           // this.FilterString = "[MaCT] = ?MaChiTietInput";
        }

       

        private void xrTableCell1_BeforePrint(object sender, CancelEventArgs e)
        {

        }
        bool visibletext = true;
        private void xrLabel2_BeforePrint(object sender, CancelEventArgs e)
        {
            XRLabel xRTableCell = (XRLabel)sender;
           // string s = xrTableCell4.Text.ToLower();
           //// Console.WriteLine(s);
            
           // if (!s.Contains("đóng gói"))
           // {
               

           //     string text = "TGHT :......giờ......, ....../...../";
           //     if (s.Trim().Contains("ráp"))
           //     {

           //         text += Environment.NewLine + "Chờ khô keo: .....giờ....,..../..../";
           //     }

           //     text += Environment.NewLine + "Chuyển CĐ:..... giờ....., ...../";
           //     xRTableCell.Text = text;
           // }
            

          
        }

        private void xrTableCell4_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell xRTableCell = (XRTableCell)sender;
            string s = xRTableCell.Text.ToLower();
            if (!s.Contains("đóng gói"))
            {
                visibletext = true;
               
            }
            else
            {
                visibletext = false;
            }
            //xRTableCell.Visible = visibletext;
            SubBand1.Visible = visibletext;
        }

    }
}
