using DataAccessLayer.Entities;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserId(string userId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(int cartId);
    }
} 