using Microsoft.AspNetCore.Mvc;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IAIService _aiService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IAIService aiService, ILogger<ChatbotController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AskAI([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Json(new { success = false, reply = "Vui lòng nhập câu hỏi." });
                }

                // Lấy context từ URL hiện tại (nếu có)
                var referer = Request.Headers["Referer"].ToString();
                var context = GetContextFromUrl(referer);

                // Gọi AI service
                var reply = await _aiService.GetReplyAsync(request.Message, context);

                return Json(new { success = true, reply = reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AskAI");
                return Json(new { success = false, reply = "Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }

        private string GetContextFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "general";

            // Xác định context dựa trên URL
            if (url.Contains("/Exam", StringComparison.OrdinalIgnoreCase))
                return "exam";
            else if (url.Contains("/KhoaHoc", StringComparison.OrdinalIgnoreCase))
                return "course";
            else if (url.Contains("/LichHoc", StringComparison.OrdinalIgnoreCase))
                return "schedule";
            else if (url.Contains("/Login", StringComparison.OrdinalIgnoreCase))
                return "login";
            else
                return "general";
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
