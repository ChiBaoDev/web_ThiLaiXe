using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webthibanglai.Models;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseApiService _courseApiService;

        public HomeController(ILogger<HomeController> logger, ICourseApiService courseApiService)
        {
            _logger = logger;
            _courseApiService = courseApiService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCoursesAsync(cancellationToken);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
