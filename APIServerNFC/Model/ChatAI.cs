using System.Text.Json.Serialization;

namespace APIServerNFC.Model
{
    public class ChatAI
    {
        public class ChatRequest
        {
            public string Question { get; set; } = string.Empty;
            public string TableName { get; set; }
            public string SqlQuery { get; set; }
            public bool Think { get; set; } = false;  // 👈 mặc định false luôn
        }

        // Response trả về cho client
        public class ChatResponse
        {
            public string Answer { get; set; } = string.Empty;
            public bool Success { get; set; }
            public string Error { get; set; }
        }

        // Request gửi lên Ollama
        public class OllamaRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = "gemma4:e4b";

            [JsonPropertyName("prompt")]
            public string Prompt { get; set; } = string.Empty;

            [JsonPropertyName("stream")]
            public bool Stream { get; set; } = false;



            [JsonPropertyName("think")]
            public bool Think { get; set; }  // 👈 thêm property này
        }

        // Response nhận từ Ollama
        public class OllamaResponse
        {
            [JsonPropertyName("response")]
            public string Response { get; set; } = string.Empty;

            [JsonPropertyName("done")]
            public bool Done { get; set; }
        }
    }
}
