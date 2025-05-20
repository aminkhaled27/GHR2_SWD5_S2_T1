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

        [Authorize]
        public async Task<IActionResult> ShowCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.AllCategories = await _categoryManager.GetAllCategoryWithProducts();
            return View("CartItems", cart);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Check if it's an AJAX request
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Check authentication first
            if (!User.Identity.IsAuthenticated)
            {
                if (isAjax)
                {
                    Response.StatusCode = 401;
                    return Json(new { 
                        success = false, 
                        message = "Please log in to add items to your cart.", 
                        requiresLogin = true 
                    });
                }
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }

            try
            {
                if (productId <= 0)
                {
                    if (isAjax)
                    {
                        Response.StatusCode = 400;
                        return Json(new { 
                            success = false, 
                            message = "Invalid product ID." 
                        });
                    }
                    return BadRequest("Invalid product ID.");
                }

                var product = await _productManager.GetProductByIdAsync(productId);

                if (product == null)
                {
                    if (isAjax)
                    {
                        Response.StatusCode = 404;
                        return Json(new { 
                            success = false, 
                            message = "Product not found." 
                        });
                    }
                    return NotFound("Product not found.");
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

                // Calculate total items in cart
                var totalItems = cart.Sum(item => item.Quantity);
                ViewBag.CartCount = totalItems;

                if (isAjax)
                {
                    Response.StatusCode = 200;
                    return Json(new { 
                        success = true, 
                        message = "Product added to cart!", 
                        cartCount = totalItems,
                        cartItems = cart.Count
                    });
                }
                return RedirectToAction("ShowCart");
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    Response.StatusCode = 500;
                    return Json(new { 
                        success = false, 
                        message = "An error occurred while adding the product to cart." 
                    });
                }
                TempData["Danger"] = "An error occurred while adding the product to cart.";
                return RedirectToAction("ShowCart");
            }
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

