using Microsoft.AspNetCore.Mvc;

namespace webthibanglai.Controllers
{
    public class OnboardingController : Controller
    {
        private readonly ILogger<OnboardingController> _logger;

        public OnboardingController(ILogger<OnboardingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra xem user đã đăng nhập chưa
            var username = TempData.Peek("AuthUsername")?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Complete()
        {
            // Không cần lưu vào database, chỉ redirect về Home
            // JavaScript sẽ xử lý lưu vào localStorage
            TempData["OnboardingCompleted"] = "Cảm ơn bạn đã hoàn thành khảo sát! Chúng tôi đã gợi ý các khóa học phù hợp cho bạn.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Skip()
        {
            // Cho phép bỏ qua onboarding
            _logger.LogInformation("User skipped onboarding");
            return RedirectToAction("Index", "Home");
        }
    }
}
