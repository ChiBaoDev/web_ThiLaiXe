using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webthibanglai.Models;
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
            model.CourseRegistrationStatusMessage = TempData["CourseRegistrationStatusMessage"]?.ToString();
            model.CourseRegistrationStatusState = TempData["CourseRegistrationStatusState"]?.ToString();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudentProfile(StudentProfileRegistrationModel registration, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["LoginSuccess"] = "Vui lòng đăng nhập để đăng ký học viên.";
                return RedirectToAction("Index", "Login");
            }

            var model = await _studentDashboardApiService.GetDashboardAsync(accessToken, cancellationToken);
            model.Registration = registration;

            if (string.IsNullOrWhiteSpace(registration.HoTen))
            {
                model.RegistrationErrorMessage = "Họ và tên không được để trống.";
                return View("Index", model);
            }

            if (!string.IsNullOrWhiteSpace(registration.NgaySinh) && !DateOnly.TryParse(registration.NgaySinh, out _))
            {
                model.RegistrationErrorMessage = "Ngày sinh không hợp lệ.";
                return View("Index", model);
            }

            var result = await _studentDashboardApiService.RegisterStudentProfileAsync(accessToken, registration, cancellationToken);
            if (!result.IsSuccess || result.Dashboard is null)
            {
                model.RegistrationErrorMessage = result.ErrorMessage ?? "Đăng ký học viên thất bại.";
                return View("Index", model);
            }

            result.Dashboard.RegistrationSuccessMessage = "Đăng ký học viên thành công.";
            return View("Index", result.Dashboard);
        }
    }
}
