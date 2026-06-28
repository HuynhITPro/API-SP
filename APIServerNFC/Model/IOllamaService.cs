using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static APIServerNFC.Model.ChatAI;

namespace APIServerNFC.Model
{
    public interface IOllamaService
    {
        Task<string> AskAsync(string question, string? dbData = null,bool Think=false);
    }

    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ollamaBaseUrl = "http://localhost:11434/api";
         private string _model = "gemma4:e4b";
       //private string _model = "qwen3.5:4b";

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(3); // timeout 3 phút
        }

        public async Task<string> AskAsync(string question, string? dbData = null,bool _Think=false)
        {
            // Tạo prompt, nếu có DB data thì ghép vào
            string prompt = BuildPrompt(question, dbData);

            var request = new OllamaRequest
            {
                Model = _model,
                Prompt = prompt,
                Stream = false,
                Think=true
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);

            return result?.Response ?? "Không có phản hồi";
        }
       
        private string BuildPrompt(string question, string? dbData)
        {
            if (string.IsNullOrEmpty(dbData))
            {
                return question;
            }

            // Prompt khi có dữ liệu từ DB
            return $"""
                Bạn là trợ lý AI. Dựa vào dữ liệu sau để trả lời câu hỏi.
                Chỉ trả lời dựa trên dữ liệu được cung cấp, không bịa thêm.
                Trả lời bằng tiếng Việt.

                DỮ LIỆU:
                {dbData}

                CÂU HỎI: {question}

                TRẢ LỜI:
                """;
        }

        public async Task<string> GenerateAnswer(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                prompt = prompt,
                stream = false,
                options = new
                {
                    temperature = 0.1,
                    num_predict = 512    // giảm từ 2048 xuống 512
                    
                }
            };
            try
            {
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/generate", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("response").GetString() ?? "";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return  "bố khỉ lỗi";
        }

    }
}
