using Microsoft.AspNetCore.Mvc;
using webthibanglai.Models;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class KhoaHocController : Controller
    {
        private const string AccessTokenSessionKey = "AccessToken";
        private readonly ICourseApiService _courseApiService;

        public KhoaHocController(ICourseApiService courseApiService)
        {
            _courseApiService = courseApiService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCoursesAsync(cancellationToken);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCourseDetailAsync(id, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int courseId, string? ghiChu, CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCourseDetailAsync(courseId, cancellationToken);

            if (model.Course is null)
            {
                model.ErrorMessage ??= "Không tìm thấy khóa học để đăng ký.";
                return View("Detail", model);
            }

            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            var result = await _courseApiService.RegisterCourseAsync(accessToken, courseId, ghiChu, cancellationToken);

            if (result.RequiresLogin)
            {
                TempData["LoginSuccess"] = result.Message;
                return RedirectToAction("Index", "Login");
            }

            if (result.RequiresStudentProfile)
            {
                TempData["StudentProfileNotice"] = result.Message;
                return RedirectToAction("Index", "LichHoc");
            }

            if (result.IsSuccess)
            {
                model.RegistrationMessage = result.Message;
            }
            else
            {
                model.RegistrationErrorMessage = result.Message;
            }

            return View("Detail", model);
        }
    }
}
