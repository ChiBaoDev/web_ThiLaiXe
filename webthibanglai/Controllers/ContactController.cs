using Microsoft.AspNetCore.Mvc;

namespace webthibanglai.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
