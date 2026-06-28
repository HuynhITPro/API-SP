using APIServerNFC.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static APIServerNFC.Model.ChatAI;

namespace APIServerNFC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatAIController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;
        private readonly TextToSqlService _textToSql;
        private readonly DatabaseService _db;
        public ChatAIController(IOllamaService ollamaService, TextToSqlService textToSql, DatabaseService db)
        {
            _ollamaService = ollamaService;
            _textToSql = textToSql;
            _db = db;
        }

        // POST api/chat
        [HttpPost]
        [Route("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Question))
                return BadRequest("Question không được để trống");

            try
            {
                var answer = await _ollamaService.AskAsync(request.Question, request.SqlQuery);

                return Ok(new ChatResponse
                {
                    Answer = answer,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ChatResponse
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        // GET api/chat/health - kiểm tra Ollama có đang chạy không
        [HttpGet("test")]
        public async Task<IActionResult> Health()
        {
            try
            {
                var answer = await _ollamaService.AskAsync("Trả lời đúng 1 từ: OK");
                return Ok(new { status = "Ollama đang chạy", response = answer });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { status = "Ollama không phản hồi" });
            }
        }

      

      

        // ============================================================
        // ENDPOINT CHÍNH: Hỏi đáp bằng tiếng Việt
        // ============================================================
        [HttpPost]
        [Route("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { Error = "Vui lòng nhập câu hỏi" });

            var result = await _textToSql.ProcessQuestion(request.Question);

            return Ok(new
            {
                result.Question,
                result.Answer,
                result.GeneratedSql,
                result.Success,
                DataRowCount = result.RawData?.Rows.Count ?? 0
            });
        }

        // ============================================================
        // ENDPOINT: Xem schema database (để debug/config)
        // ============================================================
        [HttpGet("schema")]
        public async Task<IActionResult> GetSchema()
        {
            var schema = await _db.GetDatabaseSchema();
            return Ok(new { Schema = schema });
        }
    }

}
