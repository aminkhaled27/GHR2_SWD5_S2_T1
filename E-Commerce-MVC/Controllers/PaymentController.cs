using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using E_Commerce_MVC.Extension;
using BusinessLayer.DTO.Cart;
using System.Security.Claims;
using BusinessLayer.Services.IServices;
using BusinessLayer.DTO.Order;
using DataAccessLayer.Repository.IRepository;

namespace E_Commerce_MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerRepository _customerRepository;

        public PaymentController(IOrderService orderService, ICustomerRepository customerRepository)
        {
            _orderService = orderService;
            _customerRepository = customerRepository;
        }

        public IActionResult Checkout(string id)
        {
            // Get cart items from session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Danger"] = "Your cart is empty.";
                return RedirectToAction("ShowCart", "Cart");
            }

            // Get the current request's scheme and host
            var scheme = Request.Scheme; // This will be "http" or "https"
            var host = Request.Host.Value; // This will include the port if present
            var domain = $"{scheme}://{host}/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Payment/Confirmation",
                CancelUrl = domain + $"Cart/ShowCart",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in cart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100), // Convert to cents for Stripe
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                            Images = new List<string> { domain + "Images/Product/" + item.Product.ImagePath }
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            var services = new SessionService();
            Session session = services.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> Confirmation()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Danger"] = "Please log in to complete your order.";
                    return RedirectToAction("Login", "Account");
                }

                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
                if (cart == null || !cart.Any())
                {
                    TempData["Danger"] = "Your cart is empty.";
                    return RedirectToAction("ShowCart", "Cart");
                }

                // Get customer information
                var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
                if (customer == null)
                {
                    TempData["Danger"] = "Customer information not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Create order
                var orderDto = new CreateOrderDTO
                {
                    CustomerId = customer.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = "Completed",
                    TotalAmount = cart.Sum(item => item.Product.Price * item.Quantity),
                    OrderItems = cart.Select(item => new OrderItemDTO
                    {
                        ProductId = item.Product.Id,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    }).ToList()
                };

                await _orderService.CreateOrderAsync(orderDto);

                // Clear the cart after successful order creation
                HttpContext.Session.Remove("Cart");
                TempData["Success"] = "🎉 Order placed successfully! Thank you for your purchase. Your items will be shipped soon.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Danger"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("ShowCart", "Cart");
            }
        }
    }
}
