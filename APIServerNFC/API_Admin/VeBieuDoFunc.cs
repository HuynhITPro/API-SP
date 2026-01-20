using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace APIServerNFC.API_Admin
{
    public class VeBieuDoFunc
    {
        public class imgclass
        {
            public string imgBase64 { get; set; }

        }
        bool chk_ChuaLP { get; set; } = false;
        bool chk_TonKTG { get; set; } = true;
        bool chk_TonDV { get; set; } = true;
        bool chk_TonKCR { get; set; } = true;
        bool chk_TonMBLR { get; set; } = true;
        bool chk_TonKTam { get; set; } = true;
        bool chk_TonMBKV4 { get; set; } = true;
        string chk_ChuaLPContent = "Chưa lên phiếu";
        string chk_TonKTGContent { get; set; } = "Tồn kho tinh chế";
        string chk_TonDVContent { get; set; } = "Tồn định vị";
        string chk_TonKCRContent { get; set; } = "Tồn Kho chờ ráp";
        string chk_TonMBLRContent { get; set; } = "Tồn mặt bằng ráp";
        string chk_TonKTamContent { get; set; } = "Tồn kho nhúng";
        string chk_TonMBKV4Content { get; set; } = "Tồn mặt bằng đóng gói";

        ClassProcess prs = new ClassProcess();
        public List<imgclass> lstimg = new List<imgclass>();
        DataTable dtDonHang = new DataTable();
        DataTable dt = new DataTable();
        DataTable dt_TonVe = new DataTable();
        string MaSP1 = "", MaVe1 = "";
        int Height_SP = 0;
        int Height_Ve = 0;
        int Ox_Ve = 0, Ox_CT, Oy_Ve = 0, Oy_CT = 0;
        //double d_Ve = 0, d_CT = 0, d = 0;
        DataTable dt_image = new DataTable();
      
        public async Task<string>Vebieudo(List<string> lstSP, string KhachHang, string NhaMay)
        {

            string string_temp = "";
            //dt_image.Columns.Add("Image", System.Type.GetType("System.Byte[]")); 
            dtDonHang.Clear();
            dt.Clear();
            lstimg.Clear();
            dt_TonVe.Clear();
            string DieuKien = "";
            if (lstSP.Count > 0)
            {
                string dksp = "";
                foreach (var it in lstSP)
                {
                    if (dksp == "")
                        dksp = string.Format("'{0}'", it);
                    else
                        dksp = string.Format(",'{1}'", it);

                }
                DieuKien = string.Format(" and sp.MaSP in ({0})", dksp);
            }
            SqlConnection sqlConnection = prs.Connect();
            sqlConnection.Open();
            dt = await TonKhoAll_ChartAsync(sqlConnection, NhaMay, KhachHang, DieuKien);
            //Xử lý luôn đối với trường hợp các cụm không thuộc kho tạm, cụm đó phân chia ở kho chờ ráp, nhưng không xuất hiện ở kho tạm
            //Sửa mã vế nếu vế này có xuất hiện ở vế level 2, vì level chỉ là khai báo để hiển thị cụm
            //Khu vực này được khai báo trong nhóm KV2LV2


            dt_TonVe = await TonTheoVeDBAsync(sqlConnection, NhaMay, KhachHang, DieuKien);

            dtDonHang = await DonHangPhaiNhapAsync(sqlConnection, NhaMay, "");
            //Chi tiết thuộc nhóm Level 2 (thường xảy ra đối với hàng trong nhà, do có nhiều cụm)

            //Lấy số lượng chi tiết thuộc vế để vẽ
            var query = dt.AsEnumerable().GroupBy(p => new { MaVe = p.Field<string>("MaVe") }).Select(p => new { MaVe = p.Key.MaVe, SLCTofVe = p.Count() });
            dt_TonVe.Columns.Add("SLCT_of_Ve", typeof(int));
            int k = 0;

            foreach (DataRow row in dt_TonVe.Rows)
            {
                string_temp = row["MaChiTiet"].ToString();
                k = 0;
                foreach (var it in query)
                {
                    if (string_temp == it.MaVe)
                    {
                        row["SLCT_of_Ve"] = it.SLCTofVe;
                        k = 1;
                        break;
                    }
                }
                if (k == 0)
                {
                    row["SLCT_of_Ve"] = 1;
                }
            }
            //Gom nhóm sản phẩm
            var query_SP = dt.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP"), TenSP = p.Field<string>("TenSP") }).Select(p => new { MaSP = p.Key.MaSP, TenSP = p.Key.TenSP, SLCT_of_SP = p.Count() });



            int LocationY_pic = 0;
            lstsvg.Clear();
            foreach (var it in query_SP)
            {
                VeSoDoSP_Grid(it.MaSP, it.TenSP, it.SLCT_of_SP, 0, LocationY_pic);
                //VeSoDoSP(it.MaSP, it.TenSP, it.SLCT_of_SP, 0, LocationY_pic);
                //LocationY_pic += 20 + Height_item * it.SLCT_of_SP + it.SLCT_of_SP + 10;

            }

            sqlConnection.Close();
            string jsonoutput = JsonConvert.SerializeObject(lstsvg);
            return jsonoutput;

        }
       
        public class SvgImg
        {
            public string MaSP { get; set; }
            public string Svg { get; set; }
        }
        public string Draw_RetangcleSvg(float Location_X, float Location_Y, float Width_, float Height_, string color, string NoiDung)
        {
            string text = "";
            text = Environment.NewLine + $"<rect x=\"{Location_X}\" y=\"{ Location_Y}\" width=\"{ Width_}\" height=\"{ Height_}\" fill=\"{color}\" />";
            text+= Environment.NewLine+ $"<text x=\"{Location_X+1}\" y=\"{Location_Y+15}\" font-family=\"Arial\" font-size=\"12\" fill=\"white\">{NoiDung}</text>";
            return text;
        }
        public string Draw_RetangcleSvgWithText(float Location_X, float Location_Y, float Width_, float Height_, string color,string colortext, string NoiDung)
        {
            string text = "";
            text = Environment.NewLine + $"<rect x=\"{Location_X}\" y=\"{Location_Y}\" width=\"{Width_}\" height=\"{Height_}\" fill=\"{color}\" />";
            text += Environment.NewLine + $"<text x=\"{Location_X + 1}\" y=\"{Location_Y + 15}\" font-family=\"Arial\" font-size=\"12\" fill=\"{colortext}\">{NoiDung}</text>";
            return text;
        }

        string string_temp = "";
        int i = 0;
        public async Task<DataTable> TonKhoAll_ChartAsync(SqlConnection sqlConnection, string NhaMay, string KhachHang, string DieuKien)
        {

            DataTable dt_ChiTietKhuVuc = new DataTable();
            if (NhaMay != "All")
            {
                string_temp = string.Format(@"
                declare @NhaMay nvarchar(100)=N'{2}'
                declare @tblsp as Table (MaSP nvarchar(100))
                insert into @tblsp(MaSP)
                select MaSP from ChiTietSP where MaChiTiet in (SELECT [MaCT]
                  FROM [KV1_KTG] where NhaMay=@NhaMay
                  group by MaCT) group by MaSP
                        SELECT ctsp.[MaSP],ctsp.TenSP
                        ,[MaCT],ctsp.TenChiTiet,ctsp.SoLuongCT,ChieuDaySC*ChieuRongSC*ChieuDaiSC/1000000000 as SoKhoi
                        ,MaCT_link as ID_link,[SLCT_QuyDoi],[KhuVuc]
                          FROM (select * from [dbo].[ChiTiet_KhuVuc] where KhuVuc in ('KV1X','KV2LR','KV4')) ctkv inner join dbo.Load_CTSP ctsp 
						  on ctsp.MaChiTiet=ctkv.MaCT
						inner join Load_cbSP sp on ctsp.MaSP=sp.MaSP
						where sp.KhachHang=N'{0}' {1} and sp.MaSP in (select MaSP from @tblsp)

						 order by sp.He", KhachHang, DieuKien, NhaMay);

            }
            else
            {
                string_temp = string.Format(@"
                declare @NhaMay nvarchar(100)=N'{2}'
                declare @tblsp as Table (MaSP nvarchar(100))
                insert into @tblsp(MaSP)
                select MaSP from ChiTietSP where MaChiTiet in (SELECT [MaCT]
                  FROM [KV1_KTG]
                  group by MaCT) group by MaSP
                        SELECT ctsp.[MaSP],ctsp.TenSP
                        ,[MaCT],ctsp.TenChiTiet,ctsp.SoLuongCT,ChieuDaySC*ChieuRongSC*ChieuDaiSC/1000000000 as SoKhoi
                        ,MaCT_link as ID_link,[SLCT_QuyDoi],[KhuVuc]
                          FROM (select * from [dbo].[ChiTiet_KhuVuc] where KhuVuc in ('KV1X','KV2LR','KV4')) ctkv inner join dbo.Load_CTSP ctsp 
						  on ctsp.MaChiTiet=ctkv.MaCT
						inner join Load_cbSP sp on ctsp.MaSP=sp.MaSP
						where sp.KhachHang=N'{0}' {1} and sp.MaSP in (select MaSP from @tblsp)

						 order by sp.He", KhachHang, DieuKien, NhaMay);
            }


            dt_ChiTietKhuVuc = prs.dt_Connect(string_temp, sqlConnection);



            //dt_ChiTietKhuVuc = prs.dt_Connect(string_temp, Conn);

            //DataTable dtKV3 = new DataTable();
            //DataTable dtKV2 = new DataTable();
            //DataTable dtKV1 = new DataTable();
            var KV3 = dt_ChiTietKhuVuc.AsEnumerable().Where(p => p.Field<string>("KhuVuc").ToString() == "KV4").Select(p => new { MaSP = p.Field<string>("MaSP").ToString(), TenSP = p.Field<string>("TenSP").ToString(), MaCT = p.Field<string>("MaCT").ToString(), TenCT = p.Field<string>("TenChiTiet").ToString(), SLCT_QuyDoi = p.Field<double>("SLCT_QuyDoi").ToString(), SoKhoi = p.Field<double>("SoKhoi") }).OrderBy(p => p.MaCT);

            var KV2 = dt_ChiTietKhuVuc.AsEnumerable().Where(p => p.Field<string>("KhuVuc").ToString() == "KV2LR" && (!p.Field<string>("MaCT").Contains("CTR"))).Select(p => new { MaSP = p.Field<string>("MaSP").ToString(), TenSP = p.Field<string>("TenSP").ToString(), MaCT = p.Field<string>("MaCT").ToString(), TenCT = p.Field<string>("TenChiTiet").ToString(), SLCT_QuyDoi = p.Field<double>("SLCT_QuyDoi").ToString(), ID_link = p.Field<string>("ID_link").ToString() }).OrderBy(p => p.MaCT);

            var KV1 = dt_ChiTietKhuVuc.AsEnumerable().Where(p => p.Field<string>("KhuVuc").ToString() == "KV1X").Select(p => new { MaSP = p.Field<string>("MaSP").ToString(), TenSP = p.Field<string>("TenSP").ToString(), MaCT = p.Field<string>("MaCT").ToString(), TenCT = p.Field<string>("TenChiTiet").ToString(), SLCT_QuyDoi = p.Field<double>("SLCT_QuyDoi").ToString(), SoKhoi = p.Field<double>("SoKhoi") }).OrderBy(p => p.MaCT);

            var query_groupSP = dt_ChiTietKhuVuc.AsEnumerable().GroupBy(p => new { MaSP = p.Field<string>("MaSP"), TenSP = p.Field<string>("TenSP") }).Select(p => new { MaSP = p.Key.MaSP.ToString(), TenSP = p.Key.TenSP.ToString() });

            DataTable dt_TonKho = new DataTable();

            dt_TonKho.Columns.Add("MaSP", typeof(string));
            dt_TonKho.Columns.Add("TenSP", typeof(string));
            dt_TonKho.Columns.Add("MaCT", typeof(string));
            dt_TonKho.Columns.Add("TenCT", typeof(string));
            dt_TonKho.Columns.Add("MaVe", typeof(string));
            dt_TonKho.Columns.Add("SoLuongCT", typeof(double));
            dt_TonKho.Columns.Add("SoKhoiCT", typeof(double));
            List<string> lst_ColumnLR = new List<string>();
            List<string> lst_ColumnCT = new List<string>();
            double SLCT = 1;
            #region //Tồn kho theo chi tiết
            DataTable dt_TonTheoCT = await TonTheoChiTietAsync(sqlConnection, NhaMay, DieuKien);

            for (i = 1; i < dt_TonTheoCT.Columns.Count; i++)
            {
                string_temp = dt_TonTheoCT.Columns[i].ColumnName;
                dt_TonKho.Columns.Add(string_temp, typeof(double));
                dt_TonKho.Columns[string_temp].DefaultValue = 0;
                lst_ColumnCT.Add(string_temp);
            }
            DataTable dt_TonLR = await TonLapRapAsync(sqlConnection, NhaMay, DieuKien);

            for (i = 1; i < dt_TonLR.Columns.Count; i++)
            {
                string_temp = dt_TonLR.Columns[i].ColumnName;
                dt_TonKho.Columns.Add(string_temp, typeof(double));
                dt_TonKho.Columns[string_temp].DefaultValue = 0;
                lst_ColumnLR.Add(string_temp);
            }

            string MaCTKV2, MaCTKV1;
            foreach (var it in KV1)
            {
                DataRow row = dt_TonKho.NewRow();
                row["MaCT"] = it.MaCT;
                row["MaSP"] = it.MaSP;
                row["TenCT"] = it.TenCT;
                row["TenSP"] = it.TenSP;
                row["SoLuongCT"] = it.SLCT_QuyDoi;
                row["SoKhoiCT"] = it.SoKhoi;

                dt_TonKho.Rows.Add(row);
            }



            foreach (DataRow row in dt_TonKho.Rows)
            {
                MaCTKV1 = row["MaCT"].ToString();
                SLCT = double.Parse(row["SoLuongCT"].ToString());

                foreach (DataRow row_CT in dt_TonTheoCT.Rows)
                {
                    if (MaCTKV1 == row_CT["MaCT"].ToString())
                    {
                        foreach (string it in lst_ColumnCT)
                        {

                            row[it] = Math.Round(double.Parse(row_CT[it].ToString()) / SLCT);
                        }

                        break;
                    }
                }

            }
            dt_TonTheoCT.Clear();
            lst_ColumnCT.Clear();

            #endregion
            #region //Tồn kho theo chi tiết dùng chung



            int k = 0;
            foreach (DataRow row in dt_TonKho.Rows)
            {
                MaCTKV1 = row["MaCT"].ToString();
                SLCT = double.Parse(row["SoLuongCT"].ToString());
                k = 0;
                foreach (DataRow row_CT in dt_TonLR.Rows)
                {
                    if (MaCTKV1 == row_CT["MaCT"].ToString())
                    {
                        foreach (string it in lst_ColumnLR)
                        {
                            row[it] = Math.Round(double.Parse(row_CT[it].ToString()) / SLCT);
                        }
                        k = 1;
                        break;
                    }
                }
            }

            //if (k == 0)
            //{
            //Lưu ý trường hợp chi tiết vừa nằm trong tồn kho lắp ráp, vừa nằm trong dùng chung (vì trước đây nó dùng riêng, sau này lại dùng chung)
            //Chi tiết này ko có trong bảng Tồn kho lắp ráp (Có thể cho tiết này là chi tiết rời hoặc chi tiết dùng chung)

            //Dò lại với bảng dùng chung để xử lý
            //Kiếm trong bảng dùng chung

            //}





            dt_TonLR.Clear();
            lst_ColumnLR.Clear();

            #endregion
            #region //Tồn kho theo vế
            //Xử lý ID_link, gắn vế vào bảng TonKho
            k = 0;
            foreach (DataRow row in dt_TonKho.Rows)
            {
                k = 0;
                MaCTKV1 = row["MaCT"].ToString();
                foreach (var it in KV2)
                {
                    if (MaCTKV1 == it.MaCT)
                    {
                        row["MaVe"] = it.ID_link;
                        k = 1;
                        break;
                    }
                }
                if (k == 0)
                {
                    row["MaVe"] = MaCTKV1;
                }
            }

            #endregion

            dt_TonTheoCT.Dispose();
            dt_ChiTietKhuVuc.Dispose();
            dt_TonLR.Dispose();
            return dt_TonKho;

        }
        public async Task<DataTable> TonTheoChiTietAsync(SqlConnection sqlConnection, string NhaMay, string DieuKien)
        {
            if (NhaMay != "All")
            {


                string_temp = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            
                            declare @tbl Table (ID nvarchar(100))
                            insert into @tbl (ID) select ID from dbo.KeHoachChuaXP()
                            select MaCT,sum(TonKTG) as TonKTG,sum(ChuaLP) as ChuaLP,sum(TonDV) as TonDV 
                            from (
                            SELECT [MaCT],sum([SLNhap]-[SLXuat]) as TonKTG,0 as ChuaLP,0 as TonDV
                             FROM [dbo].[KV1_KTG]  
                            where NhaMay=@NhaMay                 
                            group by MaCT
                             union all
                             select MaCT,0 as TonKTG,0 as ChuaLP,qrytondv.SLTon as TonDV 
                             from (select MaCT,SLTon from dbo.KV2_TonDinhVi(@NhaMay)) as qrytondv
                                                          union all
                                                          SELECT [MaCT],0 as TonKTG,sum([SLTheoDoi]) as ChuaLP,0 as TonDV   
                                              FROM [dbo].KeHoachChiTiet khct
                                              where SLTheoDoi>0  and NhaMay=@NhaMay 
				                              and ID_KeHoach  in  (select ID from @tbl)
                                              group by MaCT) as qry 
                                                 group by MaCT", NhaMay);
            }
            else
            {
                string_temp = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                            
                            declare @tbl Table (ID nvarchar(100))
                            insert into @tbl (ID) select ID from dbo.KeHoachChuaXP()
                            select MaCT,sum(TonKTG) as TonKTG,sum(ChuaLP) as ChuaLP,sum(TonDV) as TonDV 
                            from (
                            SELECT [MaCT],sum([SLNhap]-[SLXuat]) as TonKTG,0 as ChuaLP,0 as TonDV
                             FROM [dbo].[KV1_KTG]  
                                           
                            group by MaCT
                             union all
                             select MaCT,0 as TonKTG,0 as ChuaLP,qrytondv.SLTon as TonDV 
                             from (select MaCT,SLTon from dbo.KV2_TonDinhVi(@NhaMay)) as qrytondv
                                                          union all
                                                          SELECT [MaCT],0 as TonKTG,sum([SLTheoDoi]) as ChuaLP,0 as TonDV   
                                              FROM [dbo].KeHoachChiTiet khct
                                              where SLTheoDoi>0 
				                              and ID_KeHoach  in  (select ID from @tbl)
                                              group by MaCT) as qry 
                                                 group by MaCT", NhaMay);
            }
            //string json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            DataTable dt = prs.dt_Connect(string_temp, sqlConnection);
            return dt;
        }
        public async Task<DataTable> TonLapRapAsync(SqlConnection sqlConnection, string NhaMay, string DieuKien)
        {
            if (NhaMay != "All")
            {
                string_temp = string.Format(@"declare @NhaMay nvarchar(100) = N'{0}'
                                select MaCT, SUM(TonKCR) as TonKCR,SUM(TonMBLR) as TonMBLR from (SELECT[MaCT], sum([SLNhap] -[SLXuat]) as TonKCR,0 as TonMBLR
                              FROM[dbo].KV3_KhoChoRap  where NhaMay = @NhaMay

                              group by MaCT
                              union all
                              SELECT [MaCT],0 as TonKCR,sum([SLNhap] -[SLXuat]) as TonMBLR
                             FROM [KV3_TonMB]  where NhaMay = @NhaMay

                             group by MaCT) as qry
                             group by MaCT", NhaMay);
            }
            else
            {
                string_temp = string.Format(@"declare @NhaMay nvarchar(100) = N'{0}'
                                select MaCT, SUM(TonKCR) as TonKCR,SUM(TonMBLR) as TonMBLR from (SELECT[MaCT], sum([SLNhap] -[SLXuat]) as TonKCR,0 as TonMBLR
                              FROM[dbo].KV3_KhoChoRap  

                              group by MaCT
                              union all
                              SELECT [MaCT],0 as TonKCR,sum([SLNhap] -[SLXuat]) as TonMBLR
                             FROM [KV3_TonMB]  

                             group by MaCT) as qry
                             group by MaCT", NhaMay);
            }
            //string json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            DataTable dt = prs.dt_Connect(string_temp, sqlConnection);
            return dt;
        }
        public async Task<DataTable> DonHangPhaiNhapAsync(SqlConnection sqlConnection, string NhaMay, string DieuKien)
        {

            string_temp = string.Format(@"select qry.MaSP,qry.SoLuong as SLDH,isnull(qrydaxuat.SLDaXuat,0) as SLDaXuat,isnull(qryktp.SLNhap,0) as SLTonKho
                                ,qry.SoLuong-isnull(qrydaxuat.SLDaXuat,0)-isnull(qryktp.SLNhap,0) as SLCanNK
                                from (SELECT   [MaSP],sum([SoLuong]) as SoLuong
                                  FROM [DonHangMua]
                                  group by MaSP) as qry left join
                                 (select art.MaSP,sum(qry.SLDaXuat) as SLDaXuat from (SELECT [ArticleNumber]
                                      ,sum([SLDaXuat]) as SLDaXuat
                                  FROM [KeHoachXuatHang]
                                group by ArticleNumber) as qry
                                inner join dbo.ArticleNumberProduct art on qry.ArticleNumber=art.ArticleNumber
                                group by MaSP) as qrydaxuat on qry.MaSP=qrydaxuat.MaSP
                                left join 
                                (select MaSP,sum(SLNhap) as SLNhap from KhoTP_NK where IDNumber not in (select IDNumber from KhoTP_XK) group by MaSP) as qryktp
                                on qry.MaSP=qryktp.MaSP", NhaMay);

            //string json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            DataTable dt = prs.dt_Connect(string_temp, sqlConnection);
            return dt;
        }
        public async Task<DataTable> TonTheoVeDBAsync(SqlConnection sqlConnection, string NhaMay, string KhachHang, string dieukiensp)
        {
            string string_temp = "";
            if (NhaMay != "All")
            {
                string_temp = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                                   declare @KhachHang nvarchar(100)='{1}'
                           select ctsp.MaSP,ctsp.TenSP,ctsp.MaChiTiet,ctsp.TenChiTiet,ctsp.SoLuongCT,
                        floor(qry.TonKTam/SoLuongCT) as TonKTam,FLOOR(qry.TonMBKV4/SoLuongCT) as TonMBKV4
                         from (select MaCT,SUM(TonKTam)as TonKTam,SUM(TonMBKV4) as TonMBKV4 from (
                                   
							SELECT MaCT,0 as TonKTam,sum([SLNhap]-[SLXuat]) as TonMBKV4
     
                              FROM [dbo].[KV4_TonMB]  where NhaMay=@NhaMay                  
							  group by MaCT
                              union all
                              SELECT [MaCT],sum([SLNhap]-[SLXuat]) as TonKTam,0 as TonMBKV4
                              FROM [dbo].[KV4_KhoNhung]  where NhaMay=@NhaMay                   
							  group by MaCT 
							union all
                              select MaCT,0 as TonKTam,0 as TonMBKV4 from ChiTiet_KhuVuc where ChiTiet_KhuVuc.KhuVuc='KV4') as qry
                              group by MaCT) as qry 
							  inner join Load_CTSP ctsp on ctsp.MaChiTiet=qry.MaCT 
							  inner join  (SELECT [MaCT] FROM [ChiTiet_KhuVuc] where KhuVuc='KV4') as qry_CTKV 
							  on qry_CTKV.MaCT=ctsp.MaChiTiet 
                                inner join Load_cbSP sp on ctsp.MaSP=sp.MaSP
							  where sp.KhachHang=@KhachHang {2}
							  order by MaChiTiet", NhaMay, KhachHang, dieukiensp);
            }
            else
            {
                string_temp = string.Format(@"declare @NhaMay nvarchar(100)=N'{0}'
                                   declare @KhachHang nvarchar(100)='{1}'
                           select ctsp.MaSP,ctsp.TenSP,ctsp.MaChiTiet,ctsp.TenChiTiet,ctsp.SoLuongCT,
                        floor(qry.TonKTam/SoLuongCT) as TonKTam,FLOOR(qry.TonMBKV4/SoLuongCT) as TonMBKV4
                         from (select MaCT,SUM(TonKTam)as TonKTam,SUM(TonMBKV4) as TonMBKV4 from (
                                   
							SELECT MaCT,0 as TonKTam,sum([SLNhap]-[SLXuat]) as TonMBKV4
     
                              FROM [dbo].[KV4_TonMB]               
							  group by MaCT
                              union all
                              SELECT [MaCT],sum([SLNhap]-[SLXuat]) as TonKTam,0 as TonMBKV4
                              FROM [dbo].[KV4_KhoNhung]                  
							  group by MaCT 
							union all
                              select MaCT,0 as TonKTam,0 as TonMBKV4 from ChiTiet_KhuVuc where ChiTiet_KhuVuc.KhuVuc='KV4') as qry
                              group by MaCT) as qry 
							  inner join Load_CTSP ctsp on ctsp.MaChiTiet=qry.MaCT 
							  inner join  (SELECT [MaCT] FROM [ChiTiet_KhuVuc] where KhuVuc='KV4') as qry_CTKV 
							  on qry_CTKV.MaCT=ctsp.MaChiTiet 
                                inner join Load_cbSP sp on ctsp.MaSP=sp.MaSP
							  where sp.KhachHang=@KhachHang {2}
							  order by MaChiTiet", NhaMay, KhachHang, dieukiensp);
            }
            //string json = await callAPI.ExcuteQueryEncryptAsync(string_temp, new List<ParameterDefine>());
            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            DataTable dt = prs.dt_Connect(string_temp, sqlConnection);
            return dt;

        }
        int Height_item = 20;//20 pixel/ 1 chi tiết
        //int Width_item = 10; //10 ĐV/1 pixel
        int Location_BeginX = 230;
        int Location_BeginY = 90;

        string Color_TonKTG = "#0074bd";// Color.MediumAquamarine;

        string Color_ChuaLP = "#7c3a8c";// Color.Yellow;
        string Color_DinhVi = "#2e7063";// Color.Orange;
        string Color_KCR = "#16537e";// prs.ConvertcolorfromhexDrawing("#2e7063");// Color.DarkSeaGreen;
        string Color_MBLR = "#ff66cc";
        string Color_TonKTam = "#FFA500";
        string Color_TonKV4 = "#008000";//
        string Color_text = "#000000";

      
        double widthimg = 1900;
        List<SvgImg>lstsvg=new List<SvgImg>();
        private void VeSoDoSP_Grid(string MaSP, string TenSP, int SLCT_of_SP, int LocationX_pic, int LocationY_pic)
        {
            StringBuilder stringBuilder = new StringBuilder() ;
            double d_Ve = 0, d_CT = 0, d = 0;
            Oy_CT = Location_BeginY;
            Ox_Ve = Location_BeginX;
            Oy_Ve = Location_BeginY;
            MaSP1 = MaSP;
            int int_temp = 0;
            //Thiết kế control để khai báo theo kích thước màn hình
            //double widthimg = 1500;
            dt_image.Clear();
            int drawsmal = 20;//50pixel là tối thiểu
            Height_SP = 30 + Height_item * SLCT_of_SP + SLCT_of_SP + Location_BeginY + 20;
            //PictureBox pic = new PictureBox();
            //pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //pic.Width = 1200;
            //pic.Height = 20 + Height_item * SLCT_of_SP + SLCT_of_SP + 10;//Chiều rộng của sơ đồ bằng Tổng độ rộng từng chi tiết + 50  ,mỗi chi tiết cách ra 1 pixel
            double d_TongCT = 0, d_VeOld = 0;

            //Ảnh phải rộng ra thêm Location_BeginX do phải điền tên
            int wimg = (int)(Math.Round(widthimg, 0)) + Location_BeginX;
            int hght = (int)Height_SP;
            //Bitmap imn = new Bitmap(100, 200);
            stringBuilder.Append($"<svg width=\"{wimg}\" height=\"{hght}\" xmlns=\"http://www.w3.org/2000/svg\">");
           // Bitmap image = new Bitmap(wimg, hght);
            //image.MakeTransparent();
            //Graphics G = Graphics.FromImage(image);
            //Brushes.Transparent không hỗ trợ định dạng saveformat BMP, chỉ hỗ trộ PNG
            //G.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height);
            //Brush br = null;
            DataTable dtveTemp = new DataTable();
            DataTable dtTempCT = new DataTable();
            var querytemp = dt_TonVe.AsEnumerable().Where(p => p.Field<string>("MaSP").Equals(MaSP));
            if (querytemp.Count() > 0)
            {
                dtveTemp = querytemp.CopyToDataTable();
            }
            var querytempct = dt.AsEnumerable().Where(p => p.Field<string>("MaSP").Equals(MaSP));
            if (dtveTemp.Rows.Count > 0)
            {
                double querymaxchitiet = 0;
                double querymaxve = dtveTemp.AsEnumerable().Select(p => new { SLVe = ((chk_TonKTam) ? p.Field<double>("TonKTam") : 0) + ((chk_TonMBKV4) ? p.Field<double>("TonMBKV4") : 0) }).Max(p => p.SLVe);

                if (querytempct.Count() > 0)
                {
                    dtTempCT = querytempct.CopyToDataTable();
                    querymaxchitiet = dtTempCT.AsEnumerable().Select(p => new { SLCT = ((chk_TonKTG) ? p.Field<double>("TonKTG") : 0) + ((chk_ChuaLP) ? p.Field<double>("ChuaLP") : 0) + ((chk_TonDV) ? p.Field<double>("TonDV") : 0) + ((chk_TonKCR) ? p.Field<double>("TonKCR") : 0) + ((chk_TonMBLR) ? p.Field<double>("TonMBLR") : 0) }).Max(p => p.SLCT);
                }



                double max = querymaxve + querymaxchitiet;
                double pixeltopwidth = 0;
                pixeltopwidth = widthimg / max;

                //Width_item
                //Khởi tạo kích thước chart
                foreach (DataRow rowVe in dtveTemp.Rows)
                {
                    Ox_Ve = Location_BeginX; //Gán lại Ox
                    //Oy_CT = 0;
                    //d_Tong = 0;
                    d_VeOld = 0;
                    d_TongCT = 0;
                    if (MaSP1 == rowVe["MaSP"].ToString())
                    {
                        MaVe1 = rowVe["MaChiTiet"].ToString();
                        Height_Ve = Height_item * int.Parse(rowVe["SLCT_of_Ve"].ToString()) + int.Parse(rowVe["SLCT_of_Ve"].ToString()) - 1; //Lấy kích thước của 1 vế
                        //Tăng Oy lên
                        if (chk_TonMBKV4)
                        {
                            d_Ve = (double.TryParse(rowVe["TonMBKV4"].ToString(), out d)) ? d : 0;
                            d_VeOld += (d_Ve > 0) ? d_Ve : 0;
                            //d_Ve = double.Parse(rowVe["TonMBKV4"].ToString());
                            if (d_Ve > 0)
                            {
                                int_temp = (int)Math.Round(d_Ve * pixeltopwidth);
                              
                                    //int_temp = drawsmal;
                              stringBuilder.Append(Draw_RetangcleSvg(Ox_Ve, Oy_Ve, int_temp, Height_Ve, Color_TonKV4,  d_Ve.ToString("#,#")));
                              
                                Ox_Ve += int_temp;//Tăng Ox lên để vẽ khu vực tiếp theo
                            }
                        }
                        if (chk_TonKTam)
                        {
                            d_Ve = (double.TryParse(rowVe["TonKTam"].ToString(), out d)) ? d : 0;
                            d_VeOld += (d_Ve > 0) ? d_Ve : 0;
                            //d_Ve=double.Parse(rowVe["TonKTam"].ToString());
                            if (d_Ve > 0)
                            {
                                int_temp = (int)Math.Round(d_Ve * pixeltopwidth);
                                stringBuilder.Append(Draw_RetangcleSvg(Ox_Ve, Oy_Ve, int_temp, Height_Ve, Color_TonKTam, d_Ve.ToString("#,#")));
                                //if (int_temp < drawsmal)
                                //{
                                //    //int_temp = drawsmal;
                                //    Draw_Retangcle_FontSmall(Ox_Ve, Oy_Ve, int_temp, Height_Ve, Color_TonKTam, Color_text, br, G, d_Ve.ToString("#,#"));
                                //}
                                //else
                                //    Draw_Retangcle(Ox_Ve, Oy_Ve, int_temp, Height_Ve, Color_TonKTam, Color_text, br, G, d_Ve.ToString("#,#"));
                                Ox_Ve += int_temp;//Tăng Ox lên để vẽ khu vực tiếp theo
                            }
                        }
                        //Ox_CT = Ox_Ve;//Vẽ chi tiết nằm trên Vế
                        foreach (DataRow row_CT in dtTempCT.Rows)
                        {
                            Ox_CT = Ox_Ve;
                            if (MaVe1 == row_CT["MaVe"].ToString() || MaVe1 == row_CT["MaCT"].ToString())//Nếu là vế hoặc chi tiết rời
                            {
                                if (chk_TonMBLR)
                                {
                                    d_CT = (double.TryParse(row_CT["TonMBLR"].ToString(), out d)) ? d : 0;
                                    d_TongCT += (d_CT > 0) ? d_CT : 0;
                                    //d_CT = double.Parse(row_CT["TonMBLR"].ToString());
                                    if (d_CT > 0)
                                    {
                                        int_temp = (int)Math.Round(d_CT * pixeltopwidth);
                                        stringBuilder.Append(Draw_RetangcleSvg(Ox_CT, Oy_CT, int_temp, Height_item, Color_MBLR, d_CT.ToString("#,#")));
                                        //if (int_temp < drawsmal)
                                        //{
                                        //    //int_temp = drawsmal;
                                        //    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, int_temp, Height_item, Color_MBLR, Color.Black, br, G, d_CT.ToString("#,#"));
                                        //}
                                        //else
                                        //    Draw_Retangcle(Ox_CT, Oy_CT, int_temp, Height_item, Color_MBLR, Color.Black, br, G, d_CT.ToString("#,#"));

                                        Ox_CT += int_temp;//Tăng Ox_CT lên để vẽ khu vực tiếp theo

                                    }
                                }
                                if (chk_TonKCR)
                                {
                                    d_CT = (double.TryParse(row_CT["TonKCR"].ToString(), out d)) ? d : 0;
                                    d_TongCT += (d_CT > 0) ? d_CT : 0;
                                    //d_CT = double.Parse(row_CT["TonKCR"].ToString());
                                    if (d_CT > 0)
                                    {
                                        int_temp = (int)Math.Round(d_CT * pixeltopwidth);
                                        stringBuilder.Append(Draw_RetangcleSvg(Ox_CT, Oy_CT, int_temp, Height_item, Color_KCR, d_CT.ToString("#,#")));


                                        //if (int_temp < drawsmal)
                                        //{
                                        //    //int_temp = drawsmal;
                                        //    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, int_temp, Height_item, Color_KCR, Color.White, br, G, d_CT.ToString("#,#"));
                                        //}
                                        //else
                                        //    Draw_Retangcle(Ox_CT, Oy_CT, int_temp, Height_item, Color_KCR, Color.White, br, G, d_CT.ToString("#,#"));
                                        Ox_CT += int_temp;//Tăng Ox_CT lên để vẽ khu vực tiếp theo

                                    }
                                }
                                if (chk_TonDV)
                                {
                                    d_CT = (double.TryParse(row_CT["TonDV"].ToString(), out d)) ? d : 0;
                                    d_TongCT += (d_CT > 0) ? d_CT : 0;
                                    // d_CT = double.Parse(row_CT["TonDV"].ToString());
                                    if (d_CT > 0)
                                    {
                                        int_temp = (int)Math.Round(d_CT * pixeltopwidth);
                                        stringBuilder.Append(Draw_RetangcleSvg(Ox_CT, Oy_CT, int_temp, Height_item, Color_DinhVi,d_CT.ToString("#,#")));

                                        //if (int_temp < drawsmal)
                                        //{
                                        //    //int_temp = drawsmal;
                                        //    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, int_temp, Height_item, Color_DinhVi, Color.White, br, G, d_CT.ToString("#,#"));
                                        //}
                                        ////if (d_CT < 300)
                                        ////    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, (int)Math.Round(d_CT / Width_item), Height_item, Color_DinhVi, Color_text, br, G, d_CT.ToString());
                                        //else
                                        //    Draw_Retangcle(Ox_CT, Oy_CT, int_temp, Height_item, Color_DinhVi, Color.White, br, G, d_CT.ToString("#,#"));
                                        Ox_CT += int_temp;//Tăng Ox_CT lên để vẽ khu vực tiếp theo

                                    }
                                }
                                if (chk_ChuaLP)
                                {
                                    d_CT = (double.TryParse(row_CT["ChuaLP"].ToString(), out d)) ? d : 0;
                                    d_TongCT += (d_CT > 0) ? d_CT : 0;
                                    //d_CT = double.Parse(row_CT["ChuaLP"].ToString());
                                    if (d_CT > 0)
                                    {
                                        int_temp = (int)Math.Round(d_CT * pixeltopwidth);
                                        stringBuilder.Append(Draw_RetangcleSvg(Ox_CT, Oy_CT, int_temp, Height_item, Color_ChuaLP, d_CT.ToString("#,#")));
                                        //if (int_temp < drawsmal)
                                        //{
                                        //    //int_temp = drawsmal;
                                        //    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, int_temp, Height_item, Color_ChuaLP, Color.White, br, G, d_CT.ToString("#,#"));
                                        //}
                                        ////if (d_CT < 300)
                                        ////    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, (int)Math.Round(d_CT / Width_item), Height_item, Color_ChuaLP, Color_text, br, G, d_CT.ToString());
                                        //else
                                        //    Draw_Retangcle(Ox_CT, Oy_CT, int_temp, Height_item, Color_ChuaLP, Color.White, br, G, d_CT.ToString("#,#"));
                                        Ox_CT += int_temp;//Tăng Ox_CT lên để vẽ khu vực tiếp theo

                                    }
                                }
                                if (chk_TonKTG)
                                {
                                    d_CT = (double.TryParse(row_CT["TonKTG"].ToString(), out d)) ? d : 0;
                                    d_TongCT += (d_CT > 0) ? d_CT : 0;
                                    // d_CT = double.Parse(row_CT["TonKTG"].ToString());
                                    if (d_CT > 0)
                                    {
                                        int_temp = (int)Math.Round(d_CT * pixeltopwidth);
                                        stringBuilder.Append(Draw_RetangcleSvg(Ox_CT, Oy_CT, int_temp, Height_item, Color_TonKTG, d_CT.ToString("#,#")));

                                        //if (int_temp < drawsmal)
                                        //{
                                        //    //int_temp = drawsmal;
                                        //    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, int_temp, Height_item, Color_TonKTG, Color.White, br, G, d_CT.ToString("#,#"));
                                        //}
                                        ////if (d_CT < 300)
                                        ////    Draw_Retangcle_FontSmall(Ox_CT, Oy_CT, (int)Math.Round(d_CT / Width_item), Height_item, Color_TonKTG, Color_text, br, G, d_CT.ToString());
                                        //else
                                        //    Draw_Retangcle(Ox_CT, Oy_CT, int_temp, Height_item, Color_TonKTG, Color.White, br, G, d_CT.ToString("#,#"));
                                        Ox_CT += int_temp;//Tăng Ox_CT lên để vẽ khu vực tiếp theo

                                    }
                                }
                                //Vẽ tên chi tiết
                                // Draw_RetangcleNoFill
                                stringBuilder.Append(Draw_RetangcleSvgWithText(0, Oy_CT, 220, Height_item, "white", Color_text, row_CT["TenCT"].ToString() + " (" + (d_TongCT + d_VeOld).ToString("#,#") + " )"));
                                //Draw_Retangcle(0, Oy_CT, 220, Height_item, Color.White, Color_text, br, G, row_CT["TenCT"].ToString() + " (" + (d_TongCT + d_VeOld).ToString("#,#") + " )");
                                d_TongCT = 0;
                                Ox_CT += 200;
                                Oy_CT += Height_item + 1; //Tăng tọa độ Oy của chi tiết lên khi qua dòng mới

                            }
                        }
                        //Vẽ xong dòng này rồi thí xuống dòng kế tiếp
                        Oy_Ve += Height_Ve + 1;
                        Oy_CT = Oy_Ve;
                    }
                }
            }
            //Vẽ tên Sản Phẩm
            stringBuilder.Append(Draw_RetangcleSvgWithText(0, 10, 400, Height_item, "white", "Blue", MaSP + " - " + TenSP.ToUpper()));

            //Vẽ đơn hàng còn lại
            var querydonhang = dtDonHang.Select(string.Format("MaSP='{0}'", MaSP)).FirstOrDefault();
            if (querydonhang != null)
            {

                double cannk = double.Parse(querydonhang["SLCanNK"].ToString());
                stringBuilder.Append(Draw_RetangcleSvgWithText(0, 50, 400, Height_item, "white", "Green", string.Format("Tổng Đơn hàng còn thiếu: {0}", ((cannk < 0) ? "0" : cannk.ToString("#,#")))));
            }
            //Vẽ diễn giải trên biểu đồ luôn

            int Ox_SP = 50;
            if (chk_TonMBKV4)
            {
                int_temp = 180;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_TonKV4,chk_TonMBKV4Content));
                Ox_SP += int_temp + 1;
            }
            if (chk_TonKTam)
            {
                int_temp = 130;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_TonKTam, chk_TonKTamContent));
                Ox_SP += int_temp + 1;
            }
            if (chk_TonMBLR)
            {
                int_temp = 150;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_MBLR, chk_TonMBLRContent));
                Ox_SP += int_temp + 1;
            }
            if (chk_TonKCR)
            {
                int_temp = 140;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_KCR,  chk_TonKCRContent));
                Ox_SP += int_temp + 1;
            }
            if (chk_TonDV)
            {
                int_temp = 100;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_DinhVi, chk_TonDVContent));
                Ox_SP += int_temp + 1;
            }
            if (chk_ChuaLP)
            {
                int_temp = 130;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_ChuaLP,  chk_ChuaLPContent));
                Ox_SP += int_temp + 1;
            }
            if (chk_TonKTG)
            {
                int_temp = 120;
                stringBuilder.Append(Draw_RetangcleSvg(Ox_SP, Oy_CT + Height_item, int_temp, Height_item, Color_TonKTG, chk_TonKTGContent));
                Ox_SP += int_temp + 1;
            }
            stringBuilder.Append(Environment.NewLine + "</svg>");
            SvgImg svgImg = new SvgImg();
            svgImg.Svg = stringBuilder.ToString();  
            svgImg.MaSP = MaSP;
            lstsvg.Add(svgImg);
            dtveTemp.Dispose();
            dtTempCT.Dispose();


        }
    }
}
