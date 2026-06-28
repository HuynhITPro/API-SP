
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIServerNFC.Controllers
{
    public class DatabaseService
    {
      

       
        ClassProcess prs = new ClassProcess();
        // ============================================================
        // CHẠY SQL QUERY VÀ TRẢ VỀ KẾT QUẢ
        // ============================================================
        public async Task<QueryResult> ExecuteQuery(string sql)
        {
            var result = new QueryResult();

            using SqlConnection conn = prs.Connect() ;
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 30; // Timeout 30 giây

            using var reader = await cmd.ExecuteReaderAsync();

            // Đọc tên cột
            for (int i = 0; i < reader.FieldCount; i++)
            {
                result.Columns.Add(reader.GetName(i));
            }

            // Đọc dữ liệu từng dòng
            while (await reader.ReadAsync())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? "(null)" : reader[i]?.ToString() ?? "";

                    // Format đẹp cho số và ngày
                    if (reader[i] is decimal d)
                        value = d.ToString("N0"); // 1,234,567
                    else if (reader[i] is DateTime dt)
                        value = dt.ToString("dd/MM/yyyy HH:mm");

                    row.Add(value);
                }
                result.Rows.Add(row);
            }

            return result;
        }

        // ============================================================
        // LẤY SCHEMA THỰC TẾ TỪ DATABASE (tự động)
        // ============================================================
        // Gọi hàm này thay vì viết schema tay
        //
        public async Task<string> GetDatabaseSchema()
        {
            var schema = new System.Text.StringBuilder();
            using var conn = prs.Connect();
            await conn.OpenAsync();

            // Lấy danh sách bảng + cột + kiểu dữ liệu
            var sql = @" use NVLDB
            SELECT 
                t.TABLE_NAME,
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.IS_NULLABLE,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PK' ELSE '' END AS IS_PK
            FROM INFORMATION_SCHEMA.TABLES t
            JOIN INFORMATION_SCHEMA.COLUMNS c 
                ON t.TABLE_NAME = c.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
            LEFT JOIN (
                SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku 
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE t.TABLE_TYPE = 'BASE TABLE'
           and t.TABLE_NAME in ('NvlDonDatHangItem','NvlNhapXuat','NvlNhapXuatItem','NvlDonHang')
            ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            string currentTable = "";
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                var colName = reader.GetString(1);
                var dataType = reader.GetString(2);
                var maxLen = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                var nullable = reader.GetString(4);
                var isPk = reader.GetString(5);

                if (tableName != currentTable)
                {
                    if (currentTable != "") schema.AppendLine(");");
                    schema.AppendLine();
                    schema.AppendLine($"CREATE TABLE {tableName} (");
                    currentTable = tableName;
                    if(tableName.Contains("NvlDonDatHang"))
                    {
                        schema.AppendLine("-- Báo cáo lịch đi hàng sẽ được lấy trong bảng này");
                    }
                }
                else
                {
                    // Thêm dấu phẩy cho cột trước
                    //schema.Length -= Environment.NewLine.Length;
                    schema.AppendLine(",");
                }
                if(colName.Contains("NgayInsert"))
                    schema.AppendLine("--Đây là cột ngày sẽ được dùng trong điều kiện tìm kiếm theo ngày");

                var typeStr = dataType.ToUpper();
                if (maxLen.HasValue && maxLen > 0)
                    typeStr += $"({maxLen})";

                var pkStr = isPk == "PK" ? " PRIMARY KEY" : "";
                var nullStr = nullable == "NO" ? " NOT NULL" : "";

                schema.Append($"    {colName} {typeStr}{pkStr}{nullStr}");
            }

            if (currentTable != "")
                schema.AppendLine("\n);");

            // Thêm mẫu dữ liệu (lấy 3 dòng đầu mỗi bảng)
            schema.AppendLine();
            schema.AppendLine("-- DỮ LIỆU MẪU (3 dòng đầu mỗi bảng) --");

            var tables = await GetTableNames(conn,cmd);
            foreach (var table in tables.Take(10)) // Max 10 bảng
            {
                schema.AppendLine($"\n-- Mẫu từ bảng {table}:");
                try
                {
                    var sampleSql = $"SELECT TOP 3 * FROM [{table}]";
                    using var sampleCmd = new SqlCommand(sampleSql, conn);
                    using var sampleReader = await sampleCmd.ExecuteReaderAsync();

                    var cols = new List<string>();
                    for (int i = 0; i < sampleReader.FieldCount; i++)
                        cols.Add(sampleReader.GetName(i));
                    schema.AppendLine($"-- Cột: {string.Join(", ", cols)}");

                    while (await sampleReader.ReadAsync())
                    {
                        var vals = new List<string>();
                        for (int i = 0; i < sampleReader.FieldCount; i++)
                        {
                            var val = sampleReader.IsDBNull(i) ? "NULL" : sampleReader[i]?.ToString() ?? "";
                            if (val.Length > 50) val = val[..50] + "...";
                            vals.Add(val);
                        }
                        schema.AppendLine($"-- {string.Join(" | ", vals)}");
                    }
                }
                catch { /* Bỏ qua bảng lỗi */ }
            }

            return schema.ToString();
        }

        private async Task<List<string>> GetTableNames(SqlConnection conn,SqlCommand sqlCommand)
        {
            var tables = new List<string>();
            var sql = @"use NVLDB SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE='BASE TABLE' and TABLE_NAME in ('NvlNhapXuat','NvlNhapXuatItem','NvlDonDatHang','NvlDonDatHangItem')";
            try
            {
                sqlCommand.CommandText = sql;
                using var reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    tables.Add(reader.GetString(0));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Lỗi:AAA "+ex.Message);
            }
            return tables;
        }
    }
    public class QueryResult
    {
        public List<string> Columns { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
    }
}
