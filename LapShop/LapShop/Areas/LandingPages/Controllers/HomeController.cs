using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Areas.LandingPages.Controllers
{
    [Area("LandingPages")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
