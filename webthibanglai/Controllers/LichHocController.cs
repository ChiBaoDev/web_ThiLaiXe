using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class LichHocController : Controller
    {
        private const string AccessTokenSessionKey = "AccessToken";
        private readonly IStudentDashboardApiService _studentDashboardApiService;

        public LichHocController(IStudentDashboardApiService studentDashboardApiService)
        {
            _studentDashboardApiService = studentDashboardApiService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["LoginSuccess"] = "Vui lòng đăng nhập để xem trang học viên.";
                return RedirectToAction("Index", "Login");
            }

            var model = await _studentDashboardApiService.GetDashboardAsync(accessToken, cancellationToken);
            return View(model);
        }
    }
}
