using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_UNCNgoaiTe : DevExpress.XtraReports.UI.XtraReport
    {
        Dictionary<string, XRCheckBox> map;
        public XtraRp_UNCNgoaiTe()
        {
            InitializeComponent();
            map = new()
                {
                    { "InvoiceMoney", InvoiceMoney },
                    { "InvoiceClaimMoney", InvoiceClaimMoney },
                    { "Billoflading", Billoflading },
                    { "ImportCustomsDeclaration", ImportCustomsDeclaration },
                    { "ReexportDeclaration", ReexportDeclaration },
                    { "CreditAdvice", CreditAdvice },
                    { "SalesContract", SalesContract },
                    { "InvoiceExport", InvoiceExport },
                    { "TransitDeclaration", TransitDeclaration },
                    { "InvestmentRegistrationCertificate", InvestmentRegistrationCertificate },
                    { "EnterpriseRegistrationCertificate", EnterpriseRegistrationCertificate },
                    { "TaxDocuments", TaxDocuments },
                     { "OtherDocuments", OtherDocuments }
                };
        }
        public IEnumerable<NameDocument> SelectedDocumnet { get; set; }

        private void XtraRp_UNCNgoaiTe_BeforePrint(object sender, CancelEventArgs e)
        {
           
        }

        private void xrTableCell73_BeforePrint(object sender, CancelEventArgs e)
        {
            var cell = sender as XRTableCell;
            if (cell != null && decimal.TryParse(cell.Text, out decimal value))
            {
                var viVN = new CultureInfo("vi-VN");
                cell.Text = value.ToString("#,0.##", viVN); // "1.234.567,89"
            }
        }

        

        private void XtraRp_UNCNgoaiTe_BeforePrint_1(object sender, CancelEventArgs e)
        {
            if (SelectedDocumnet != null)
            {
                foreach (var doc in SelectedDocumnet)
                {
                    foreach (var kv in map)
                    {
                        if (doc.Name == kv.Key)
                        {
                            kv.Value.Checked = true;
                            break;
                        }

                    }
                }
            }
            if (this.DataSource is List<RemittanceRequest>)
            {
                List<RemittanceRequest> lst = (List<RemittanceRequest>)(this.DataSource);
                RemittanceRequest remittanceRequest = lst.First();
                Sec.Checked = remittanceRequest.Sec;
                Swift.Checked = remittanceRequest.Swift;
                FromAccount.Checked = remittanceRequest.PaymentSource.FromAccount ?? false;
                FromCash.Checked = remittanceRequest.PaymentSource.FromCash ?? false;
                FromForeignCashAccount.Checked = remittanceRequest.PaymentSource.FromForeignCashAccount ?? false;
                FromForeignCurrencyAccount.Checked = remittanceRequest.PaymentSource.FromForeignCurrencyAccount ?? false;
                FromOtherSource.Checked = remittanceRequest.PaymentSource.FromOtherSource ?? false;
                VCBSellsForeignCurrency.Checked = remittanceRequest.PaymentSource.VCBSellsForeignCurrency ?? false;
                OUR.Checked = remittanceRequest.Charges.ChargeTypeSelected.Contains("OUR - ");
                SHA.Checked = remittanceRequest.Charges.ChargeTypeSelected.Contains("SHA - ");
                BEN.Checked = remittanceRequest.Charges.ChargeTypeSelected.Contains("BEN - ");
                NODEDUCT.Checked = remittanceRequest.Charges.ChargeTypeSelected.Contains("NODEDUCT - ");
                TK.Checked = remittanceRequest.Charges.ChangeTypeListDetailSelected.Contains("TK số");
                Cash.Checked = remittanceRequest.Charges.ChangeTypeListDetailSelected.Contains("Cash");
                //IIF(Contains([Charges.ChangeTypeListDetailSelected], 'TK số'), TRUE, false)

            }
        }
    }
}
