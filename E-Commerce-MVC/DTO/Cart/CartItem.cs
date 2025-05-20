using BusinessLayer.DTO.Product;

namespace BusinessLayer.DTO.Cart
{
    public class CartItem
    {
        public ProductCardDTO Product { get; set; }
        public int Quantity { get; set; }
    }
} 