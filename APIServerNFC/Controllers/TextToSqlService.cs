using APIServerNFC.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APIServerNFC.Controllers
{
    public class TextToSqlService
    {
        private readonly OllamaService _ollama;
        private readonly DatabaseService _db;
        private  string _dbSchema;

        public TextToSqlService(OllamaService ollama, DatabaseService db, IConfiguration config)
        {
            _ollama = ollama;
            _db = db;

            // Schema mô tả cấu trúc database của bạn
            // ĐÂY LÀ PHẦN QUAN TRỌNG NHẤT - bạn cần sửa cho đúng với DB thực
            //_dbSchema = config["Database:Schema"] ?? GetDefaultSchema();
           // _dbSchema = _db.GetDatabaseSchema();//  GetDefaultSchema();
        }

        // ============================================================
        // HÀM CHÍNH: Xử lý câu hỏi end-to-end
        // ============================================================
        public async Task<ChatResult> ProcessQuestion(string question)
        {
            var result = new ChatResult { Question = question };

            try
            {
                // ═══ BƯỚC 1: Gửi câu hỏi + schema cho Ollama để viết SQL ═══
                var sqlQuery = await GenerateSql(question);
                if (!sqlQuery.Contains("use NVLDB"))
                    sqlQuery = string.Format(" use NVLDB {0}", sqlQuery);
                result.GeneratedSql = sqlQuery;


                // ═══ BƯỚC 2: Validate SQL (bảo mật!) ═══
                var validation = ValidateSql(sqlQuery);
                if (!validation.IsValid)
                {
                    result.Answer = $"Câu hỏi không thể xử lý: {validation.Reason}";
                    result.Success = false;
                    return result;
                }

                // ═══ BƯỚC 3: Chạy SQL trên database ═══
                var queryResult = await _db.ExecuteQuery(sqlQuery);
                result.RawData = queryResult;

                if (queryResult.Rows.Count == 0)
                {
                    result.Answer = "Không tìm thấy dữ liệu phù hợp với câu hỏi của bạn.";
                    result.Success = true;
                    return result;
                }

                // ═══ BƯỚC 4: Gửi kết quả cho Ollama để diễn giải ═══
                var answer = await InterpretResult(question, sqlQuery, queryResult);
                result.Answer = answer;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Answer = "Xin lỗi, đã có lỗi khi xử lý câu hỏi. Vui lòng thử lại.";
                result.Error = ex.Message;
                result.Success = false;
            }

            return result;
        }

        // ============================================================
        // BƯỚC 1: Ollama viết SQL từ câu hỏi
        // ============================================================
        private async Task<string> GenerateSql(string question)
        {
            if (_dbSchema == null)
                _dbSchema =await _db.GetDatabaseSchema();
            var prompt = $@"Bạn là chuyên gia SQL Server. Nhiệm vụ: viết câu SQL query dựa trên câu hỏi của người dùng.

                        === CẤU TRÚC DATABASE ===
                        {_dbSchema}

                        === QUY TẮC BẮT BUỘC ===
                        1. CHỈ viết câu SELECT, TUYỆT ĐỐI KHÔNG dùng INSERT, UPDATE, DELETE, DROP, ALTER, EXEC, TRUNCATE.
                        2. Trả về DUY NHẤT câu SQL, không giải thích, không markdown, không dấu ```.
                        3. Dùng TOP 100 để giới hạn kết quả.
                        4. Dùng alias tiếng Việt cho cột kết quả (VD: AS N'Tên sản phẩm').
                        5. Xử lý ngày tháng đúng format SQL Server.
                        6. Nếu câu hỏi không liên quan đến database, trả về: SELECT N'Câu hỏi không liên quan đến dữ liệu' AS N'Thông báo'

                        === CÂU HỎI ===
                        {question}

                        === SQL QUERY ===";

            var sql = await _ollama.GenerateAnswer(prompt);

            // Dọn dẹp kết quả (loại bỏ markdown, khoảng trắng thừa)
            sql = CleanSqlOutput(sql);

            return sql;
        }

        // ============================================================
        // BƯỚC 2: Validate SQL — QUAN TRỌNG VỀ BẢO MẬT
        // ============================================================
        private SqlValidation ValidateSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return new SqlValidation(false, "SQL rỗng");

            var upperSql = sql.ToUpper().Trim();

            // Chỉ cho phép SELECT
            //if (!upperSql.StartsWith("SELECT"))
            //    return new SqlValidation(false, "Chỉ cho phép câu lệnh SELECT");

            // Danh sách từ khóa nguy hiểm
            var dangerous = new[]
            {
           
            "INTO "     // SELECT INTO
        };

            foreach (var keyword in dangerous)
            {
                if (upperSql.Contains(keyword))
                    return new SqlValidation(false, $"Phát hiện từ khóa không an toàn: {keyword.Trim()}");
            }

            // Kiểm tra có nhiều statement không (ngăn SQL injection qua ;)
            // Cho phép ; ở cuối nhưng không ở giữa
            var trimmed = sql.TrimEnd().TrimEnd(';');
            if (trimmed.Contains(';'))
                return new SqlValidation(false, "Không cho phép nhiều câu lệnh SQL");

            return new SqlValidation(true, "OK");
        }

        // ============================================================
        // BƯỚC 4: Ollama diễn giải kết quả thành câu trả lời
        // ============================================================
        private async Task<string> InterpretResult(
            string question, string sql, QueryResult data)
        {
            // Chuyển data thành dạng text dễ đọc
            var dataText = FormatDataAsText(data);

            var prompt = $@"Bạn là trợ lý AI thông minh. Nhiệm vụ: dựa vào dữ liệu truy vấn được, trả lời câu hỏi của người dùng.

=== CÂU HỎI ===
{question}

=== CÂU SQL ĐÃ CHẠY ===
{sql}

=== KẾT QUẢ TRẢ VỀ ({data.Rows.Count} dòng) ===
{dataText}

=== QUY TẮC TRẢ LỜI ===
1. Trả lời bằng tiếng Việt, tự nhiên, dễ hiểu.
2. Nêu con số cụ thể nếu có.
3. Nếu dữ liệu có nhiều dòng, tóm tắt những điểm quan trọng.
4. Không nhắc đến SQL hay database trong câu trả lời.
5. Trả lời ngắn gọn, đi thẳng vào vấn đề.

=== CÂU TRẢ LỜI ===";

            return await _ollama.GenerateAnswer(prompt);
        }

        // ============================================================
        // HÀM PHỤ
        // ============================================================

        private string CleanSqlOutput(string sql)
        {
            // Loại bỏ markdown code block
            sql = Regex.Replace(sql, @"```sql\s*", "", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"```\s*", "");

            // Loại bỏ giải thích trước/sau SQL
            var lines = sql.Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .ToList();

            // Tìm dòng bắt đầu bằng SELECT
            var selectIndex = lines.FindIndex(l =>
                l.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase));

            if (selectIndex >= 0)
            {
                sql = string.Join("\n", lines.Skip(selectIndex));
            }

            return sql.Trim().TrimEnd(';');
        }

        private string FormatDataAsText(QueryResult data)
        {
            if (data.Rows.Count == 0)
                return "(Không có dữ liệu)";

            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(" | ", data.Columns));
            sb.AppendLine(new string('-', data.Columns.Sum(c => c.Length + 3)));

            // Giới hạn hiển thị 20 dòng đầu (tránh prompt quá dài)
            var rowsToShow = data.Rows.Take(20).ToList();
            foreach (var row in rowsToShow)
            {
                sb.AppendLine(string.Join(" | ", row));
            }

            if (data.Rows.Count > 20)
                sb.AppendLine($"... và {data.Rows.Count - 20} dòng nữa");

            return sb.ToString();
        }

        // Schema mặc định — BẠN CẦN SỬA CHO ĐÚNG VỚI DATABASE THỰC
        private string GetDefaultSchema()
        {
            return @"
-- Bảng sản phẩm
CREATE TABLE Products (
    Id INT PRIMARY KEY,
    ProductName NVARCHAR(200),      -- Tên sản phẩm
    CategoryId INT,                  -- Mã danh mục
    Price DECIMAL(18,2),            -- Giá bán
    Stock INT,                       -- Số lượng tồn kho
    CreatedAt DATETIME2             -- Ngày tạo
);

-- Bảng danh mục
CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    CategoryName NVARCHAR(100)      -- Tên danh mục
);

-- Bảng khách hàng
CREATE TABLE Customers (
    Id INT PRIMARY KEY,
    FullName NVARCHAR(200),         -- Họ tên
    Phone NVARCHAR(20),             -- Số điện thoại
    Email NVARCHAR(200),            -- Email
    Address NVARCHAR(500),          -- Địa chỉ
    CreatedAt DATETIME2             -- Ngày đăng ký
);

-- Bảng đơn hàng
CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    CustomerId INT,                  -- FK -> Customers
    OrderDate DATETIME2,            -- Ngày đặt hàng
    TotalAmount DECIMAL(18,2),      -- Tổng tiền
    Status NVARCHAR(50)             -- Trạng thái: 'Pending','Confirmed','Shipped','Delivered','Cancelled'
);

-- Bảng chi tiết đơn hàng
CREATE TABLE OrderDetails (
    Id INT PRIMARY KEY,
    OrderId INT,                     -- FK -> Orders
    ProductId INT,                   -- FK -> Products
    Quantity INT,                    -- Số lượng
    UnitPrice DECIMAL(18,2)         -- Đơn giá tại thời điểm mua
);
";
        }
    }

    // ============================================================
    // MODELS
    // ============================================================
    public class ChatResult
    {
        public string Question { get; set; } = "";
        public string GeneratedSql { get; set; } = "";
        public string Answer { get; set; } = "";
        public bool Success { get; set; }
        public string? Error { get; set; }
        public QueryResult? RawData { get; set; }
    }

    public record SqlValidation(bool IsValid, string Reason);
}
