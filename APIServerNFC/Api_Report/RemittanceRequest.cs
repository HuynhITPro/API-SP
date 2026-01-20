using System.Collections.Generic;
using System;

namespace APIServerNFC.Api_Report
{
    public class RemittanceRequest
    {
        public void Initialize()
        {

            OrganizationInfo = new OrganizationCustomer
            {
                Name = "CÔNG TY TNHH SCANSIA PACIFIC",
                RegisteredAddress = "Lô 18 đường Song Hành, KCN Tân Tạo, Phường Tân Tạo, TP.HCM",
                LegalRepresentative = "ĐINH DIỄM THÚY",
                Position = "Tổng giám đốc",
                Phone = "028.3750.7208",
                Fax = "028.3750.7209",
                BusinessRegistrationNumber = "0302307213",
                RegistrationDetails = "Cấp lần đầu ngày 18/04/2001, đăng ký thay đổi lần 8 ngày 13/06/2025"
            };
            Amount = new AmountDetails { Currency = "USD" };


            PaymentSource = new PaymentSource
            {
                FromForeignCurrencyAccount = true,
                ForeignCurrencyDescription = "007 137 0599 524"
            };
            SelectedDocumnet = new List<NameDocument> {new NameDocument
                 {
                     Name = "Tờ khai hải quan nhập khẩu / Import Customs Declaration",
                     Description = "ImportCustomsDeclaration"
                 }
            };

            Charges = new Charges
            {
                ChangeTypeListDetailDecription = "007 137 0599 524, 007 100 0599 514"
            };
            BeneficiaryBank = new BeneficiaryBank
            {
                //Name = "Standard Chartered Bank Shanghai branch",
                //BankCode = "SCBLCNSXSHA"
            };
            Beneficiary = new Beneficiary
            {
                //Name = "Inter Testing & Consulting Services (Shanghai) Co., Ltd.",
                //AccountNumber = "501511371152",
                //Address = "No.399 Gang Wen Road, Feng Xian District, Shanghai 201413, China"
            };
        }
        public string? MaUNC { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;


        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Remittance Method
        public RemittanceMethod Method { get; set; } = RemittanceMethod.SWIFT;

        // Customer Information

        public CustomerType CustomerType { get; set; } = CustomerType.Organization;
        public bool Swift { get { return (Method == RemittanceMethod.SWIFT); } }
        public bool Sec { get { return (Method == RemittanceMethod.BankDraft); } }
        public IndividualCustomer? IndividualInfo { get; set; }

        public OrganizationCustomer? OrganizationInfo { get; set; }

        // Amount Information

        public AmountDetails Amount { get; set; } = new AmountDetails();

        // Payment Source

        public PaymentSource PaymentSource { get; set; }

        // Bank Information
        public IntermediaryBank? IntermediaryBank { get; set; }


        public BeneficiaryBank BeneficiaryBank { get; set; }


        public Beneficiary Beneficiary { get; set; }

        // Payment Details

        public string PaymentDetails { get; set; } = string.Empty;

        // Charges

        public Charges Charges { get; set; } = new Charges();

        //Change Method
        public TranferMethod TranferMethod { get; set; } = TranferMethod.SWIFT;

        // Other Details
        public string? OtherDetails { get; set; }


        public IEnumerable<NameDocument> SelectedDocumnet { get; set; }
        public List<NameDocument> DocumentList = new(){
                new NameDocument
                {
                    Description   = "Hóa đơn đòi tiền theo hợp đồng mua hàng/Invoice",
                    Name = "InvoiceMoney"
                },
                new NameDocument
                {
                    Description = "Hóa đơn đòi tiền theo hợp đồng tái xuất hàng hóa/Invoice",
                    Name = "InvoiceClaimMoney"
                },
                new NameDocument
                {
                    Description = "Vận đơn hoặc chứng từ vận tải khác/Bill of lading",
                    Name = "Billoflading"
                },
                new NameDocument
                {
                    Description = "Tờ khai hải quan nhập khẩu/Import Customs Declaration",
                    Name = "ImportCustomsDeclaration"
                },
                new NameDocument
                {
                    Description = "Tờ khai hải quan tái xuất hàng hóa hoặc thay thế/Re-export Declaration",
                    Name = "ReexportDeclaration"
                },
                new NameDocument
                {
                    Description = "Báo có tiền hàng theo hợp đồng tái xuất/Credit advice",
                    Name = "CreditAdvice"
                },
                new NameDocument
                {
                    Description = "Hợp đồng bán hàng hóa kinh doanh chuyển khẩu/Sales contract",
                    Name = "SalesContract"
                },
                new NameDocument
                {
                    Description = "Hóa đơn theo hợp đồng bán hàng hóa kinh doanh chuyển khẩu/Invoice",
                    Name = "InvoiceExport"
                },
                new NameDocument
                {
                    Description = "Tờ khai hải quan quá cảnh hoặc tương đương/Transit Declaration",
                    Name = "TransitDeclaration"
                },
                new NameDocument
                {
                    Description = "Giấy chứng nhận đăng ký đầu tư/Investment Registration Certificate",
                    Name = "InvestmentRegistrationCertificate"
                },
                new NameDocument
                {
                    Description = "Giấy chứng nhận đăng ký doanh nghiệp/Enterprise Registration Certificate",
                    Name = "EnterpriseRegistrationCertificate"
                },
                new NameDocument
                {
                    Description = "Chứng minh hoàn thành nghĩa vụ thuế/Tax documents",
                    Name = "TaxDocuments"
                },
                new NameDocument
                {
                    Description = "Chứng từ khác/Other documents",
                    Name = "OtherDocuments"
                }
            };


        public DateTime? DateLine { get; set; }

    }
    public class NameDocument
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public enum RemittanceMethod
    {
        SWIFT,
        BankDraft
    }

    public enum CustomerType
    {
        Individual,
        Organization
    }

    /// <summary>
    /// Thông tin khách hàng cá nhân
    /// </summary>
    public class IndividualCustomer
    {

        public string Name { get; set; } = string.Empty;


        public string Address { get; set; } = string.Empty;


        public string Phone { get; set; } = string.Empty;


        public string IdNumber { get; set; } = string.Empty;


        public DateTime DateOfIssue { get; set; }


        public string PlaceOfIssue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Thông tin khách hàng tổ chức
    /// </summary>
    public class OrganizationCustomer
    {

        public string Name { get; set; } = string.Empty;


        public string RegisteredAddress { get; set; } = string.Empty;


        public string LegalRepresentative { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;


        public string Phone { get; set; } = string.Empty;

        public string? Fax { get; set; }


        public string BusinessRegistrationNumber { get; set; } = string.Empty;


        public string RegistrationDetails { get; set; } = string.Empty;
    }

    /// <summary>
    /// Chi tiết số tiền
    /// </summary>
    public class AmountDetails
    {

        public decimal? AmountInFigures { get; set; }


        public string Currency { get; set; } = "USD";


        public string AmountInWords { get; set; } = string.Empty;
    }

    /// <summary>
    /// Nguồn tiền thanh toán
    /// </summary>
    public class PaymentSource
    {


        // For FX Transaction
        public string? CurrencyPair { get; set; }
        public DateTime? SettlementDate { get; set; }
        public decimal? AmountInForeignCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? AmountInVND { get; set; }

        // Source Options
        public bool? FromForeignCurrencyAccount { get; set; } = false;
        public string? ForeignCurrencyDescription { get; set; }
        public decimal? ForeignCurrencyAmount { get; set; }

        public bool? FromAccount { get; set; } = false;
        public string? AccountDescription { get; set; }
        public decimal? AccountAmount { get; set; }

        public bool? FromForeignCashAccount { get; set; } = false;
        public string? ForeignCashAccountDescription { get; set; }
        public decimal? ForeignCashAccountAmount { get; set; }


        public bool? FromCash { get; set; } = false;
        public decimal? CashAmount { get; set; }
        public string? CashDescription { get; set; }
        public bool? FromOtherSource { get; set; } = false;
        public decimal? OtherSourceAmount { get; set; }
        public string? OtherSourceDescription { get; set; }

        public bool? VCBSellsForeignCurrency { get; set; } = false;
        public decimal? VCBSellsAmount { get; set; }
        public string? VCBSellsDescription { get; set; }
    }

    /// <summary>
    /// Ngân hàng trung gian
    /// </summary>
    public class IntermediaryBank
    {
        public string? Name { get; set; }
        public string? BankCode { get; set; }
        public string? Address { get; set; }
    }

    /// <summary>
    /// Ngân hàng người hưởng
    /// </summary>
    public class BeneficiaryBank
    {

        public string Name { get; set; } = string.Empty;


        public string BankCode { get; set; } = string.Empty;

        public string? Address { get; set; }
    }

    /// <summary>
    /// Người hưởng
    /// </summary>
    public class Beneficiary
    {

        public string Name { get; set; } = string.Empty;


        public string AccountNumber { get; set; } = string.Empty;


        public string Address { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    /// <summary>
    /// Chi tiết phí
    /// </summary>
    public class Charges
    {

        //public ChargeType ChargeType { get; set; } = ChargeType.OUR;

        // For OUR charge type
        public string[] ChargeTypeList = new string[] { "OUR - Phí do người chuyển tiền chịu", " BEN - Phí do người hưởng chịu", "SHA - Phí chia sẻ cho hai bên", "NODEDUCT - Phí chỉ áp dụng cho USD" };
        public string ChargeTypeSelected { get; set; } = "OUR - Phí do người chuyển tiền chịu";

        public string? DebitAccountNumbers { get; set; }

        public string[] ChangeTypeListDetial = new string[] { "Phí trích từ TK số (Debit our account number)", "Phí nộp bằng tiền mặt (Cash)" };
        public string ChangeTypeListDetailSelected { get; set; } = "Phí trích từ TK số (Debit our account number)";
        public string? ChangeTypeListDetailDecription { get; set; } = "007 137 0599 524, 007 100 0599 514";
    }


    public enum TranferMethod
    {
        SWIFT,
        SEC
    }
    /// <summary>
    /// Cam kết bổ sung chứng từ
    /// </summary>
    public class DocumentCommitment
    {
        public List<DocumentType> RequiredDocuments { get; set; } = new List<DocumentType>();

        public string? OtherDocuments { get; set; }

        public DateTime? SupplementDeadline { get; set; }
    }
    public class DocumentItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }


    public enum DocumentType
    {
        Invoice,
        ReExportInvoice,
        BillOfLading,
        ImportCustomsDeclaration,
        ReExportCustomsDeclaration,
        InvestmentRegistrationCertificate,
        EnterpriseRegistrationCertificate,
        TaxObligationProof,
        Others
    }

}
