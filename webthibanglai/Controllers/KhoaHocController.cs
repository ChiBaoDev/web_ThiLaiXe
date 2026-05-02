using Microsoft.AspNetCore.Mvc;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class KhoaHocController : Controller
    {
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
    }
}
