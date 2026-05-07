using Microsoft.AspNetCore.Mvc;

namespace webthibanglai.Controllers
{
    public class ThongBaoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
