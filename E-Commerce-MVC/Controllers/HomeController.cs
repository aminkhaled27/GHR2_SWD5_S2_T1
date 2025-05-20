using System.Diagnostics;
using BusinessLayer.Manager.IManager;
using BusinessLayer.DTO.Category;
using BusinessLayer.DTO;
using E_Commerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.Controllers
{

    public class HomeController : BaseController
    {
        private readonly IProductManager _productManager;
        private readonly ICategoryWithProductManager _categoryManager;

        public HomeController(IProductManager productManager, ICategoryWithProductManager categoryManager)
        {
            _productManager = productManager;
            _categoryManager = categoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productManager.GetFeaturedProducts();
            ViewBag.FeaturedProducts = featuredProducts;

            var categories = await _categoryManager.GetAllCategoryWithProducts();
            ViewBag.AllCategories = categories;

            return View(categories);
        }

        [HttpGet]
        public IActionResult ShowAllProducts()
        {

            return View("ShowProducts");
        }

        [HttpGet]
        public IActionResult TestSuccessMessage()
        {
            TempData["Success"] = "Test message! If you see this, TempData is working.";
            return RedirectToAction("Index");
        }
    }

}
