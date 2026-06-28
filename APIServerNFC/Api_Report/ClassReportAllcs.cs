using System;
using System.Collections.Generic;

namespace APIServerNFC.Api_Report
{
    public class ClassReportAllcs
    {
        public class ModelBienBan
        {
            public List<NguoiDaiDien> ListBenGiao { get; set; } = new();
            public List<NguoiDaiDien> ListBenNhan { get; set; } = new();
            public List<NguoiKy> ListNguoiKy { get; set; } = new();
            public ThongTinChungModel ThongTinChung { get; set; } = new();
            public List<NvlNhapXuatItemShow> ListHangHoa { get; set; } = new();
            public List<NvlNhapXuat_PhatSinhKhacShow> LstChiPhiKhac { get; set; } = new();


        }
        public class NguoiDaiDien
        {
            public string HoTen { get; set; } = "";

            public string ChucVu { get; set; } = "";
            public string SoDT { get; set; }
            public string CongTy { get; set; }
            public string DiaChi { get; set; }
        }
        public class NguoiKy
        {
            public string TieuDe { get; set; } = "";
            public string HoTen { get; set; } = "";
        }

        public class ThongTinChungModel
        {

            public string NoiGiao { get; set; } = "";
            public string NoiNhan { get; set; } = "";
            public string DiaChiNoiGiao { get; set; }
            public string DiaChiNoiNhan { get; set; }
            public string SDTNoiGiao { get; set; }
            public string SDTNoiNhan { get; set; }
            public string TaiKho { get; set; } = "";
            public string LyDo { get; set; } = "";
            public string GhiChu { get; set; } = "- Biên bản được lập thành 2 bản có giá trị như nhau./.";
            public DateTime? NgayGioIn { get; set; } = DateTime.Now;
            public string NgayGioBienBan { get; set; } = string.Format("Ngày {0}, tháng {1}, năm {2}", DateTime.Now.Day.ToString("D2"), DateTime.Now.Month.ToString("D2"), DateTime.Now.Year);
        }
        public class NvlNhapXuatItemShow
        {
            public int Serial { get; set; }
            public int? SerialCT { get; set; }
            public int? SerialLink { get; set; }
            public bool chk { get; set; } = false;
            public int? SerialKHDH { get; set; }
            public Nullable<int> SerialDN { get; set; }
            public string NguoiDN { get; set; }
            public string MaPDoc { get; set; }
            public string MaCT { get; set; }
            public string TenLienKet { get; set; }
            public string SelectedKHItem { get; set; }
            public string TableName { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaNhom { get; set; }
            public string TenNhom { get; set; }
            public string DVT { get; set; }
            public Nullable<DateTime> NgaySanXuat { get; set; }
            private decimal? _soLuong { get; set; }
            public decimal? SoLuong
            {
                get => _soLuong.HasValue ? Math.Truncate(_soLuong.Value) == _soLuong.Value
                               ? Math.Truncate(_soLuong.Value)  // số nguyên → trim hết .000000
                               : _soLuong.Value                 // có lẻ → giữ nguyên
                             : null;
                set => _soLuong = value;
            }

            public decimal? SLNhap { get; set; } = 0;//Nên khởi tạo sẵn và 2 biến này không cho phép null
            public decimal? SLXuat { get; set; } = 0;//Nên khởi tạo sẵn và 2 biến này không cho phép null
            public decimal? SLTon { get; set; } = 0;
            public decimal? SLNo { get; set; } = 0;
            public Nullable<decimal> DonGiaDN { get; set; }
            private decimal? _dongia;
            public decimal? DonGia
            {
                get => _dongia.HasValue ? Math.Truncate(_dongia.Value) == _dongia.Value
                               ? Math.Truncate(_dongia.Value)  // số nguyên → trim hết .000000
                               : _dongia.Value                 // có lẻ → giữ nguyên
                             : null;
                set => _dongia = value;
            }

            public decimal? SLNhapTT { get; set; }

            public decimal? ThanhTien
            {
                get { return DonGia * (SLNhap + SLXuat); }
            }
            public decimal? SLXuatTT { get; set; }
            public string DVTTT { get; set; }
            public double? TyLeQuyDoi { get; set; }
            public string KhachHang_XuatXu { get; set; }
            public Nullable<DateTime> NgayHetHan { get; set; }
            public string MaKien { get; set; }
            public string SoLo { get; set; }

            public string SoXe { get; set; }
            public string GhiChu { get; set; }
            public string Barcode { get; set; }
            public string MaSP { get; set; }
            public string ArticleNumber { get; set; }
            public string UserInsert { get; set; }
            public DateTime NgayInsert { get; set; }
            public DateTime Ngay { get; set; }
            public string LyDo { get; set; }
            public string TenSP { get; set; }
            public string MaGN { get; set; }
            public string TenGN { get; set; }
            public string ChatLuong { get; set; }
            public decimal? SLConLai { get; set; }
            public string NhaMay { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public Nullable<int> flag { get; set; }
            public string MaDatHang { get; set; }
            private string _tinhTrang { get; set; }
            public string PathImgTinhTrang { get; set; }
            public string GhiChuDeNghi { get; set; }
            public string GhiChuKeHoach { get; set; }
           
            public string DauTuan { get; set; }
            public string ViTri { get; set; }
            public string DienGiai { get; set; }
            public string NguoiThanhToan { get; set; }
            public DateTime? NgayThanhToan { get; set; }
            public string TinhTrangSuDung { get; set; }
            public string MaKeHoach { get; set; }
            public string Err { get; set; }
            public string? JsonDescription { get; set; }
        }
        public class ExtraCost
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
        }
        public class NvlNhapXuat_PhatSinhKhacShow
        {
            public int Serial { get; set; } = 0;

            public int? SerialCT { get; set; }

            public string? NoiDung { get; set; }

            public decimal? ThanhTien { get; set; }
            public string? TypeName { get; set; }
            public int? sign_type { get; set; }

            public string? TableName { get; set; }
            public int? GhiNo { get; set; }

            public DateTime? NgayInsert { get; set; }

            public string? UserInsert { get; set; }
        }
        public class NvlNhapXuat_PhatSinhMain
        {
            public decimal? TotalQuantity { get; set; } = 0;
            public decimal? TotalAmount { get; set; } = 0;


            public decimal? Discount { get; set; } = 0;

            public decimal? VatPercent { get; set; } = 0;
            public decimal? VatTotal { get; set; } = 0;
            public decimal? VatNote { get; set; } = 0;
            public decimal? Payment { get; set; } = 0;
            public decimal? PaymentAccounts { get; set; } = 0;
            public bool IsDebt { get; set; } = false;
            public bool IsVAT { get; set; } = true;
            public decimal? FinalTotal { get; set; } = 0;
            public decimal? Remaining { get; set; } = 0;
            public decimal? DiscountPercent { get; set; } = 0;
            public bool DiscountIsPercent { get; set; } = false;
            public List<ExtraCost> ExtraCosts { get; set; } = new();
            public int extraCostIdCounter { get; set; } = 0;
        }
    }
}
