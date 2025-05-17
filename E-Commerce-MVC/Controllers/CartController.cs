using BusinessLayer.DTO.Product;
using BusinessLayer.Manager.IManager;
using E_Commerce_MVC.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.DTO.Cart;

namespace E_Commerce_MVC.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductManager _productManager;
        private readonly ICategoryWithProductManager _categoryManager;



        public CartController(IProductManager productManager, ICategoryWithProductManager categoryManager)
        {
            _productManager = productManager;
            _categoryManager = categoryManager;
        }

        public async Task<IActionResult> ShowCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.AllCategories = await _categoryManager.GetAllCategoryWithProducts();
            return View("CartItems", cart);
        }
        [Authorize]
        [HttpGet]
        // Action to add a product to the cart
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _productManager.GetProductByIdAsync(productId);

            if (product == null)
            {
                TempData["Danger"] = "Product not found.";
                return NotFound();  
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(x => x.Product.Id == product.Id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = 1 });
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            TempData["Success"] = "Product added to cart!";
            return RedirectToAction("Index", "Home");
            //return Json(new { success = true, message = "Product added to cart successfully." });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("ShowCart");
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)
            {
                item.Quantity++;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("ShowCart");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            else if (item != null && item.Quantity == 1)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("ShowCart");
        }
    }
}

