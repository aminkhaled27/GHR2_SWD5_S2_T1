using Microsoft.AspNetCore.Mvc;
using BusinessLayer.DTO.Product;
using System.Collections.Generic;
using E_Commerce_MVC.Extension;

namespace E_Commerce_MVC.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<ProductCardDTO>>("Cart") ?? new List<ProductCardDTO>();
            ViewBag.CartCount = cart.Count;
            base.OnActionExecuting(context);
        }
    }
} 