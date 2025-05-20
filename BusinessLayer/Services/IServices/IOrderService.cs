using BusinessLayer.DTO.Order;

namespace BusinessLayer.Services.IServices
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetUserOrdersAsync(string userId);
        Task<OrderDTO> CreateOrderAsync(CreateOrderDTO orderDto);
    }
} 