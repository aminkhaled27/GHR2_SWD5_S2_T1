using Microsoft.AspNetCore.Mvc;

using Stripe.Checkout;

namespace E_Commerce_MVC.Controllers
{
    public class PaymentController : Controller
    {
        public class DamyProduct
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public int CategoryId { get; set; }
            public int Quantity { get; set; }
        }





        public IActionResult Checkout(string id)
        {
            //Claim IdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            //var order = _cardServices.GetCartItems(IdClaims.Value.ToString());
            var order = new List<DamyProduct>
            {
                new DamyProduct
                {
                    Id=1,
                    Name="IPhone13",
                    CategoryId=1,
                    ImageUrl="1.jpg",
                    Price=190000,
                    Description="This is Iphone13",
                    Quantity= 1
                }
            };

            var domain = "https://localhost:7216/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Payment/OrderConfiermation",
                CancelUrl = domain + $"Account/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };
            foreach (var item in order)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * item.Quantity),
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name
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
    }
}
