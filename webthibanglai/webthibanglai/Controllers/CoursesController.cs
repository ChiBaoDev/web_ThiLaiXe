using Microsoft.AspNetCore.Mvc;

namespace webthibanglai.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
