using System.Text;
using System.Text.Json;

namespace webthibanglai.Services
{
    public class OpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            // Lấy API key từ configuration
            _apiKey = _configuration["OpenAI:ApiKey"] ?? "";
            _apiUrl = _configuration["OpenAI:ApiUrl"] ?? "https://api.openai.com/v1/chat/completions";
        }

        public async Task<string> GetReplyAsync(string message, string context = "general")
        {
            try
            {
                // Kiểm tra API key
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogWarning("OpenAI API key not configured");
                    return GetFallbackResponse(message, context);
                }

                // Tạo system prompt dựa trên context
                var systemPrompt = GetSystemPrompt(context);

                // Tạo request body
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = message }
                    },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Thêm Authorization header
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                // Gọi OpenAI API
                var response = await _httpClient.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
                    
                    if (result?.Choices != null && result.Choices.Length > 0)
                    {
                        return result.Choices[0].Message.Content;
                    }
                }
                else
                {
                    _logger.LogError($"OpenAI API error: {response.StatusCode}");
                }

                // Fallback nếu API thất bại
                return GetFallbackResponse(message, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
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

Hãy trả lời ngắn gọn, rõ ràng và thân thiện. Sử dụng tiếng Việt.";

            return context switch
            {
                "exam" => basePrompt + "\n\nHiện tại học viên đang ở trang thi thử. Hãy tập trung vào việc hướng dẫn cách làm bài thi, mẹo ghi nhớ câu hỏi, và chiến lược làm bài hiệu quả.",
                "course" => basePrompt + "\n\nHọc viên đang xem thông tin khóa học. Hãy tư vấn về các loại khóa học, thời gian học, học phí và lợi ích của từng khóa.",
                "schedule" => basePrompt + "\n\nHọc viên đang xem lịch học. Hãy giải đáp về lịch học, lịch thi, cách sắp xếp thời gian học tập hiệu quả.",
                "login" => basePrompt + "\n\nHọc viên đang ở trang đăng nhập. Hãy hỗ trợ về tài khoản, đăng ký, quên mật khẩu.",
                _ => basePrompt
            };
        }

        private string GetFallbackResponse(string message, string context)
        {
            // Phản hồi mặc định khi không có API hoặc API lỗi
            var lowerMessage = message.ToLower();

            // Câu hỏi về loại bằng lái
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

            // Câu hỏi về thi
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

            // Câu hỏi về khóa học
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

            // Câu hỏi về lịch
            if (lowerMessage.Contains("lịch") || lowerMessage.Contains("thời gian"))
            {
                return @"**Về lịch học và lịch thi:**

📅 Lịch học được cập nhật thường xuyên
🕐 Có nhiều khung giờ linh hoạt
📍 Học tại trung tâm hoặc online

Bạn có thể xem lịch chi tiết tại trang Lịch Học.
Cần hỗ trợ gì thêm?";
            }

            // Câu hỏi chung
            return @"Xin chào! Tôi có thể giúp bạn:

💬 Tư vấn chọn loại bằng lái phù hợp
📖 Hướng dẫn ôn thi hiệu quả
📚 Thông tin về khóa học
📅 Lịch học và lịch thi
❓ Giải đáp thắc mắc về luật giao thông

Bạn cần hỗ trợ gì?";
        }

        // Models cho OpenAI Response
        private class OpenAIResponse
        {
            public Choice[]? Choices { get; set; }
        }

        private class Choice
        {
            public Message Message { get; set; } = new Message();
        }

        private class Message
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
