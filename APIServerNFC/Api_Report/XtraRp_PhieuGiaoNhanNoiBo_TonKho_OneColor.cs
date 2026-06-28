using DevExpress.Export.Xl;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using static APIServerNFC.Api_Report.XuLyTonKhoGiaoNhan;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_PhieuGiaoNhanNoiBo_TonKho_OneColor : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_PhieuGiaoNhanNoiBo_TonKho_OneColor()
        {
            InitializeComponent();
        }
        public void setGhiChu(string ghichu)
        {
            this.lbGhiChu.Text = ghichu;
        }

        // Class model
        //double colWeight = 0.66402827916538432D; // Weight mặc định cho cột màu, có thể điều chỉnh nếu cần
        float colWidth = 60f;
        // Fields trong report
        List<MaSPMaMau> querymau;
        private XRTableCell[] _groupColorCells;
        private XRTableCell[] _detailColorCells;
 
        private int _maxColors = 0;
        public void InitDynamicColors(List<MaSPMaMau> querymau_)
        {
            // Build dictionary từ querymau
                querymau = querymau_;
           
              var maxmau = querymau.Max(p => p.Index);//Lấy số lượng màu của sản phẩm có nhiều màu nhất để tạo cột
            if (maxmau == 0)
                return; // Nếu chỉ có 1 màu thì không cần tạo cột động
            List<string> lstcolumn=new List<string>();
            //Thêm cột vào bảng
            for (int i = 0; i <= maxmau; i++)
            {
                lstcolumn.Add("MaMau_" + i);
                //dtsource.Columns.Add("MaMau_" + i, typeof(string));
            }
            _maxColors = maxmau;
            // Thêm cột động vào report
            AddDynamicColorColumns(
                xrTableRowHeader1,      // header row 1
                xrTableRowHeader2,      // header row 2
                xrTableRowGroup,  // group header row
                xrTableRowDetail, // detail row
                lstcolumn,
                maxmau
            );

            // Cache cells để không phải FindControl mỗi lần
            CacheColorCells();

            // Đăng ký sự kiện
            GroupHeader1.BeforePrint += GroupHeader1_BeforePrint;
            //Detail.BeforePrint += Detail_BeforePrint;
        }

        private void CacheColorCells()
        {
            _groupColorCells = new XRTableCell[_maxColors+1];
            _detailColorCells = new XRTableCell[_maxColors+1];

            for (int i = 0; i <= _maxColors; i++)
            {
                _groupColorCells[i] = xrTableRowGroup.Table?
                    .FindControl($"cellGroup_MaMau_{i}", true) as XRTableCell;
                _detailColorCells[i] = xrTableRowDetail.Table?
                    .FindControl($"cellDetail_MaMau_{i}", true) as XRTableCell;
            }
        }

        private void GroupHeader1_BeforePrint(object sender, CancelEventArgs e)
        {
            var maSP = GetCurrentColumnValue("MaSP")?.ToString();
           
            if (string.IsNullOrEmpty(maSP)) return;
            var querysp=querymau.Where(p=>p.MaSP==maSP).ToList();
           // querymau.TryGetValue(maSP, out var colors);
            int colorCount = querysp?.Count ?? 0;
           
            for (int i = 0; i <= _maxColors; i++)
            {
                var cell = _groupColorCells[i];
                if (cell == null) continue;

                if (i < colorCount)
                {
                    var item = querysp.FirstOrDefault(p=>p.Index == i);
                    cell.Text = item.TenMau;
                  
                  
                    //if (maSP == "C10190010")
                    //    System.Diagnostics.Debug.WriteLine(string.Format("CellTextIN:{0},CellWidthIn: {1}", cell.Text, cell.WidthF));

                }
                else
                {
                    cell.Text = "";
                    //cell.WidthF = 0;
                    //cell.Borders = DevExpress.XtraPrinting.BorderSide.None;
                }
                //if (maSP == "C10190010")
                //    System.Diagnostics.Debug.WriteLine(string.Format("CellText:{0},Width: {1}",cell.Text,cell.WidthF));
            }
        }

        private void Detail_BeforePrint(object sender, CancelEventArgs e)
        {
            //var maSP = GetCurrentColumnValue("MaSP")?.ToString();
            //if (string.IsNullOrEmpty(maSP)) return;
            //// Cùng sản phẩm thì skip
            ////if (maSP == _lastProductName) return;
            ////_lastProductName = maSP;
            //var querysp = querymau.Where(p => p.MaSP == maSP).ToList();
            //_colorsByProduct.TryGetValue(maSP, out var colors);
        //    int colorCount = colors?.Count ?? 0;

        //    for (int i = 0; i <= _maxColors; i++)
        //    {
        //        var cell = _detailColorCells[i];
        //        if (cell == null) continue;

        //        if (i < colorCount)
        //        {
        //            cell.WidthF =colWidth ;
        //            cell.Borders = DevExpress.XtraPrinting.BorderSide.Left
        //                         | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            if(i==c)
        //        }
        //        //else
        //        //{
        //        //    cell.Text = "";
        //        //    cell.WidthF = 0;
        //        //    cell.Borders = DevExpress.XtraPrinting.BorderSide.None;
        //        //}
        //    }
        }
        public void InitColumn(List<string> lstcolumn)
        {
            AddDynamicColorColumns(xrTableRowHeader1, xrTableRowHeader2, xrTableRowGroup, xrTableRowDetail, lstcolumn, lstcolumn.Count);
           


        }
        private void AddDynamicColorColumns(
    XRTableRow xrTableHeader1,    // Header cố định: "Mã màu 1", "Mã màu 2"...
    XRTableRow xrTableHeader2,//Do header có 2 dòng, nó mearge 2 dòng nên phải thêm cột vào cả 2 dòng header
    XRTableRow xrGroupHeader1,   // Group header: tên màu thực tế theo sản phẩm
    XRTableRow xrTableDetail,    // Detail: giá trị số lượng
    List<string> lstcolumn,      // ["Color1", "Color2", ..., "ColorN"]
    int maxColors)
        {
            

            for (int i = 0; i < lstcolumn.Count; i++)
            {
                string fieldName = lstcolumn[i]; // "Color1", "Color2"...
                XRTableCell headerCell = new XRTableCell();
                //  1. TABLE HEADER (tiêu đề cột cố định)
                headerCell.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
           )));
                headerCell.Multiline = true;
                headerCell.Text = $"Mã màu";
                headerCell.Name = $"cellHeader_{fieldName}";
                headerCell.StylePriority.UseBorders = false;
                headerCell.StylePriority.UseTextAlignment = false;
               
                headerCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                headerCell.WidthF = colWidth;
                xrTableHeader1.Cells.Add(headerCell);


                XRTableCell headerCell2 = new XRTableCell();

                headerCell2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Left)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
                headerCell2.ForeColor = System.Drawing.Color.Black;
                headerCell2.Multiline = true;
               
                headerCell2.Name = $"cellHeader_{fieldName}";
                headerCell2.StylePriority.UseBorders = false;
                headerCell2.StylePriority.UseForeColor = false;
                headerCell2.StylePriority.UseTextAlignment = false;
                headerCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                headerCell2.WidthF = colWidth;
                if(i==lstcolumn.Count-1)
                {
                    headerCell.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right))));
                    headerCell2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.All))));
                }
                   



                xrTableHeader2.Cells.Add(headerCell2);


                // === 2. GROUP HEADER (tên màu thực tế, thay đổi theo sản phẩm) ===
                XRTableCell groupCell = new XRTableCell();
                groupCell.Name = $"cellGroup_MaMau_{i}";
                groupCell.Text = ""; // Sẽ set trong BeforePrint
                groupCell.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom)));

                groupCell.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
                groupCell.Multiline = true;
                groupCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                groupCell.StylePriority.UseBorders = false;
                groupCell.StylePriority.UseFont = false;
                groupCell.StylePriority.UseTextAlignment = false;
                groupCell.WidthF = colWidth;
               
                //groupCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;



                // Binding tên màu từ field ColorName1, ColorName2...
                //        groupCell.ExpressionBindings.AddRange(new ExpressionBinding[] {
                //    new ExpressionBinding("BeforePrint", "Text", $"[ColorName{i + 1}]")
                //});
                xrGroupHeader1.Cells.Add(groupCell);

                // === 3. DETAIL (giá trị số lượng) ===
                XRTableCell detailCell = new XRTableCell();
                detailCell.Name = fieldName;
                detailCell.Borders = (DevExpress.XtraPrinting.BorderSide.Left
                                    | DevExpress.XtraPrinting.BorderSide.Bottom);
                detailCell.StylePriority.UseBorders = false;
                detailCell.StylePriority.UseTextAlignment = false;
                detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                detailCell.TextFormatString = "{0:#,#}";
                detailCell.Multiline = true;
                detailCell.Name= $"cellDetail_MaMau_{i}";
                detailCell.WidthF = colWidth;
                detailCell.ExpressionBindings.AddRange(new ExpressionBinding[] {
            new ExpressionBinding("BeforePrint", "Text", $"[{fieldName}]")
        });
                xrTableDetail.Cells.Add(detailCell);

                if (i == lstcolumn.Count - 1)
                {
                    detailCell.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right))));
                    groupCell.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right))));
                    //headerCell2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.All))));
                }

            }

            // QUAN TRỌNG: Tăng width cho cả 4 table
         
            // Tăng width cho tất cả table
            float totalAdded = colWidth * lstcolumn.Count;

            var tables = new HashSet<XRTable>();
            if (xrTableHeader1.Table != null) tables.Add(xrTableHeader1.Table);
            //if (xrTableHeader2.Table != null) tables.Add(xrTableHeader2.Table);
            if (xrTableRowGroup.Table != null) tables.Add(xrTableRowGroup.Table);
            if (xrTableRowDetail.Table != null) tables.Add(xrTableRowDetail.Table);

            foreach (var table in tables)
            {
                table.WidthF += totalAdded;
            }
        }
    }
}
