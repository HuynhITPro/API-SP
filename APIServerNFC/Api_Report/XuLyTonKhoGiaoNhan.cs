using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace APIServerNFC.Api_Report
{
    public class XuLyTonKhoGiaoNhan
    {
        public List<MaSPMaMau> querymau=new List<MaSPMaMau>(); 
        public  void XuLyBang(DataSet ds)
        {

            //Xử lý tạo cột mã màu, xác định sản phẩm nào có nhiều màu nhất
           
            DataTable dtsource = ds.Tables[0];
            querymau.Clear();
            if (ds.Tables.Count < 2)
                return;
            DataTable dtmau = ds.Tables[1];
            var querycheckmau = dtmau.AsEnumerable()
                 .GroupBy(p => new {
                     MaSP = p.Field<string>("MaSP"),
                     MaMau = p.Field<string>("MaMau"),
                     TenMau = p.Field<string>("TenMau"),
                     

                 })
                 .Select(g => new
                 {
                     MaSP = g.Key.MaSP,
                     MaMau = g.Key.MaMau,
                     TenMau = g.Key.TenMau,
                     SoDongTon = g.Count(n =>
                        Convert.ToDouble(n["SLTon"] == DBNull.Value ? 0 : n["SLTon"]) != 0),//Lấy những dòng có phát sinh số liệu

                     SLCheck = g.Sum(n =>
                        Math.Abs(Convert.ToDouble(n["SLNhap"] == DBNull.Value ? 0 : n["SLNhap"]))
                       + Math.Abs(Convert.ToDouble(n["SLXuat"] == DBNull.Value ? 0 : n["SLXuat"]))
                       + Math.Abs(Convert.ToDouble(n["SLTonDau"] == DBNull.Value ? 0 : n["SLTonDau"])))
                 })
                 .Where(p=>p.SoDongTon>0)
                 // Nhóm lại theo MaSP, mỗi SP lấy top 6 màu có SLCheck cao nhất
                 .GroupBy(x => x.MaSP)
                 .SelectMany(g => g.OrderByDescending(x => x.SLCheck).Take(6)).ToList();

            querymau = querycheckmau
             .OrderBy(p => p.MaSP).ThenBy(p => p.MaMau)
             .GroupBy(p => p.MaSP)
             .SelectMany(g => g.Select((item, index) => new MaSPMaMau
             {
                 MaSP = item.MaSP,
                 MaMau = item.MaMau,
                 TenMau = item.TenMau,
                 Index = index
             }))
             .ToList();//Đánh index về 0 mỗi khi bắt đầu 1 sản phẩm
            //Do form in chỉ hiển thị chiều ngang được tối đa là 6 cột nên giới hạn lại số màu hiển thị

            //Danh sách thứ tự này rất quan trọng để tạo cột mã màu đúng với sản phẩm
            if (!querymau.Any())//Không có màu nào phát sinh
                return;
            var maxmau = querymau.Max(p => p.Index);//Lấy số lượng màu của sản phẩm có nhiều màu nhất để tạo cột
          
            if (maxmau == 0)//chỉ có 1 màu, không cần xử lý màu làm gì
                return;

            //Thêm cột vào bảng
            for (int i = 0; i <= maxmau; i++)
            {
                dtsource.Columns.Add("MaMau_" + i, typeof(double));
            }
            if (dtmau.Rows.Count > 0)
            {

                // Chuyển DataTable → Dictionary
                Dictionary<string, List<ChiTietTon>> dicTonKhoMau = dtmau.AsEnumerable()
                    .GroupBy(row => row.Field<string>("MaCT"))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(row => new ChiTietTon
                        {
                            MaMau = row.Field<string>("MaMau"),
                            SLTon = Convert.ToDouble(row["SLTon"])
                        }).ToList()
                    );

                //Chuyển dtsource về dictionary luôn duyệt cho nhanh, lưu ý, datarow này vẫn tham chiếu đến dtsource như thường
                Dictionary<string, List<DataRow>> dicSP = dtsource.AsEnumerable()
                .GroupBy(row => row.Field<string>("MaSP") ?? "")
                .ToDictionary(g => g.Key, g => g.ToList());
                //Duyệt theo sản phẩm

                //Bắt đầu duyệt
                foreach (var kvp in dicSP)
                {
                    string maSP = kvp.Key;
                    List<DataRow> rows = kvp.Value;
                    var queryspmau = querymau.Where(p=>p.MaSP==maSP).ToList();
                    foreach (var row in rows)
                    {
                        string mact = row.Field<string>("MaChiTiet");
                        if (dicTonKhoMau.TryGetValue(mact, out var listmauton))
                        {
                            //Lấy ra index
                            foreach (var itmauton in listmauton)
                            {
                                foreach (var itmau in queryspmau)
                                {
                                    if (itmauton.MaMau == itmau.MaMau)
                                    {
                                        row["MaMau_" + itmau.Index] = itmauton.SLTon;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }
                dicTonKhoMau.Clear();
            }

            // Code xử lý tồn kho giao nhận
            // Ví dụ: Lấy dữ liệu từ cơ sở dữ liệu, tính toán tồn kho, và trả về kết quả
        }
        // Định nghĩa class
        public class ChiTietTon
        {

            public string MaMau { get; set; }
            public double SLTon { get; set; }
        }
        public class MaSPMaMau
        {
            public string MaSP { get; set; }
            public string MaMau { get; set; }
            public string TenMau { get; set; }
            public int Index { get; set; }
        }
    }
}
