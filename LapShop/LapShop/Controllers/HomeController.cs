using Microsoft.AspNetCore.Mvc;
using LapShop.Models;
using Microsoft.EntityFrameworkCore;
namespace LapShop.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            LapShopContext ctx = new LapShopContext();
            var categories= ctx.TbCategories.Where(s=>s.CategoryName.EndsWith("e") || s.CreatedDate<DateTime.Now)
                .OrderBy(a=>a.CategoryName).OrderBy(d=>d.CreatedDate).ToList();


            return View(categories);
        }
    }
}
