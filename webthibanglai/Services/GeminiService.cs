using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace webthibanglai.Services
{
    public class GeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            _apiUrl = configuration["Gemini:ApiUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        }

        public async Task<string> GetReplyAsync(string message, string context = "general")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    _logger.LogWarning("Gemini API key not configured");
                    return GetFallbackResponse(message, context);
                }

                var prompt = $"{GetSystemPrompt(context)}\n\nCâu hỏi của học viên: {message}";
                var requestBody = new GeminiRequest
                {
                    Contents =
                    [
                        new GeminiContent
                        {
                            Parts =
                            [
                                new GeminiPart { Text = prompt }
                            ]
                        }
                    ],
                    GenerationConfig = new GeminiGenerationConfig
                    {
                        MaxOutputTokens = 500,
                        Temperature = 0.7
                    }
                };

                var requestUrl = $"{_apiUrl}?key={Uri.EscapeDataString(_apiKey)}";
                var jsonContent = JsonSerializer.Serialize(requestBody);
                using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                    var reply = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                    if (!string.IsNullOrWhiteSpace(reply))
                    {
                        return reply.Trim();
                    }
                }
                else
                {
                    _logger.LogError("Gemini API error: {StatusCode}. Response: {ResponseContent}", response.StatusCode, responseContent);
                }

                return GetFallbackResponse(message, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                return GetFallbackResponse(message, context);
            }
        }

        private string GetSystemPrompt(string context)
        {
            var basePrompt = @"Bạn là trợ lý AI thông minh của hệ thống thi bằng lái xe tại Việt Nam.
Nhiệm vụ của bạn là hỗ trợ học viên với thông tin chính xác, hữu ích về:
- Các loại bằng lái xe (A1, A2, B1, B2, C, D, E, F)
- Quy trình đăng ký và thi bằng lái
- Mẹo học tập và ôn thi hiệu quả
- Luật giao thông Việt Nam
- Kỹ năng lái xe an toàn

Hãy trả lời ngắn gọn, rõ ràng, thân thiện và sử dụng tiếng Việt.";

            return context switch
            {
                "exam" => basePrompt + "\n\nHiện tại học viên đang ở trang thi thử. Hãy tập trung vào cách làm bài thi, mẹo ghi nhớ câu hỏi và chiến lược làm bài hiệu quả.",
                "course" => basePrompt + "\n\nHọc viên đang xem thông tin khóa học. Hãy tư vấn về các loại khóa học, thời gian học, học phí và lợi ích của từng khóa.",
                "schedule" => basePrompt + "\n\nHọc viên đang xem lịch học. Hãy giải đáp về lịch học, lịch thi và cách sắp xếp thời gian học tập hiệu quả.",
                "login" => basePrompt + "\n\nHọc viên đang ở trang đăng nhập. Hãy hỗ trợ về tài khoản, đăng ký và quên mật khẩu.",
                _ => basePrompt
            };
        }

        private string GetFallbackResponse(string message, string context)
        {
            var lowerMessage = message.ToLower();

            if (lowerMessage.Contains("bằng") && (lowerMessage.Contains("loại") || lowerMessage.Contains("nào")))
            {
                return @"Hiện tại có các loại bằng lái xe chính:

🏍️ **Bằng A**: Xe mô tô 2 bánh
- A1: Xe dưới 175cc
- A2: Xe trên 175cc

🚗 **Bằng B**: Xe ô tô
- B1: Xe số tự động, dưới 9 chỗ
- B2: Xe số sàn, dưới 9 chỗ

🚚 **Bằng C, D, E**: Xe tải, xe khách

Bạn muốn tìm hiểu loại bằng nào?";
            }

            if (lowerMessage.Contains("thi") || lowerMessage.Contains("ôn"))
            {
                return @"**Mẹo ôn thi hiệu quả:**

✅ Làm bài thi thử thường xuyên
✅ Tập trung vào câu hỏi điểm liệt
✅ Học theo chủ đề (biển báo, sa hình, kỹ thuật...)
✅ Ghi nhớ bằng hình ảnh
✅ Ôn lại các câu sai

Bạn cần hỗ trợ gì thêm về thi cử?";
            }

            if (lowerMessage.Contains("khóa học") || lowerMessage.Contains("học phí") || lowerMessage.Contains("đăng ký"))
            {
                return @"**Thông tin khóa học:**

📚 Chúng tôi có nhiều khóa học phù hợp:
- Khóa học lý thuyết online
- Khóa học thực hành
- Khóa học tổng hợp (lý thuyết + thực hành)

💰 Học phí linh hoạt theo từng khóa
⏰ Lịch học linh hoạt, phù hợp mọi đối tượng

Bạn muốn đăng ký khóa học nào?";
            }

            if (lowerMessage.Contains("lịch") || lowerMessage.Contains("thời gian"))
            {
                return @"**Về lịch học và lịch thi:**

📅 Lịch học được cập nhật thường xuyên
🕐 Có nhiều khung giờ linh hoạt
📍 Học tại trung tâm hoặc online

Bạn có thể xem lịch chi tiết tại trang Lịch Học.
Cần hỗ trợ gì thêm?";
            }

            return @"Xin chào! Tôi có thể giúp bạn:

💬 Tư vấn chọn loại bằng lái phù hợp
📖 Hướng dẫn ôn thi hiệu quả
📚 Thông tin về khóa học
📅 Lịch học và lịch thi
❓ Giải đáp thắc mắc về luật giao thông

Bạn cần hỗ trợ gì?";
        }

        private class GeminiRequest
        {
            [JsonPropertyName("contents")]
            public GeminiContent[] Contents { get; set; } = [];

            [JsonPropertyName("generationConfig")]
            public GeminiGenerationConfig GenerationConfig { get; set; } = new();
        }

        private class GeminiContent
        {
            [JsonPropertyName("parts")]
            public GeminiPart[] Parts { get; set; } = [];
        }

        private class GeminiPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }

        private class GeminiGenerationConfig
        {
            [JsonPropertyName("maxOutputTokens")]
            public int MaxOutputTokens { get; set; }

            [JsonPropertyName("temperature")]
            public double Temperature { get; set; }
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public GeminiCandidate[]? Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent? Content { get; set; }
        }
    }
}
