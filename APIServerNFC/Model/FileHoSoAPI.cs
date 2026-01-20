using System;

namespace APIServerNFC.Model
{
    public class FileHoSoAPI
    {
        public int Serial { get; set; }
        public string TableName { get; set; }
        public int? SerialLink { get; set; }
        public string TenFile { get; set; }
        public string UrlFile { get; set; }
        public string DienGiai { get; set; }
        public double? DungLuong { get; set; }
        public string Dvt { get; set; }
        public string UserInsert { get; set; }

        public int? SerialRoot { get; set; }
        public string? TableNameRoot { get; set; }
        public string? NoiDungRoot { get; set; }
        public DateTime? NgayInsert { get; set; }
    }
}
