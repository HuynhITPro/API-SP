using System;
using System.Collections.Generic;

namespace APIServerNFC
{
    public class AllClass
    {
    }
    public class ExcelExportDefine
    {
        public GetDataFromSql getDataFromSql { get; set; }
        //public string SqlQuery { get; set; }
        public string FileName { get; set; } = "ExportExcel";
        public string SheetName { get; set; } = "Sheet1";
        public string TieuDe { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public int ColumnBegin { get; set; } = 1;
        public int RowBegin { get; set; } = 3;
        public List<HeaderExcel> lstheader { get; set; }

    }
    public class HeaderExcel
    {
        public string HeaderName { get; set; }
        public string HeaderCaption { get; set; }
        //public int RowSpan { get; set; }
    }
    public class JsonFooter
    {
        public string TenBoPhan { get; set; }
        public string HoVaTen { get; set; }
        //public int RowSpan { get; set; }
    }
    public class JsonHeader
    {
        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string DiaChi { get; set; }
        public string? MauSo { get; set; }
        public string? LanSoatXet { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
    }
}
