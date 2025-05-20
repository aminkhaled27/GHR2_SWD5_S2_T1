using BusinessLayer.DTO.Order;
using BusinessLayer.Services.IServices;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDTO>> GetUserOrdersAsync(string userId)
        {
            var customer = await _context.Customers
                .Include(c => c.orders)
                .ThenInclude(o => o.ProductOrders)
                .ThenInclude(po => po.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                return new List<OrderDTO>();

            return customer.orders.Select(o => new OrderDTO
            {
                OrderId = o.Id,
                UserId = userId,
                OrderDate = o.CreatedDate,
                TotalAmount = o.ProductOrders.Sum(po => po.Product.Price),
                Status = "Completed",
                OrderItems = o.ProductOrders.Select(po => new OrderItemDTO
                {
                    ProductId = po.ProductId,
                    ProductName = po.Product.Name,
                    Quantity = 1,
                    UnitPrice = po.Product.Price
                }).ToList()
            }).ToList();
        }

        public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                CreatedDate = orderDto.OrderDate,
                ProductOrders = orderDto.OrderItems.Select(item => new ProductOrder
                {
                    ProductId = item.ProductId
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return new OrderDTO
            {
                OrderId = order.Id,
                UserId = orderDto.CustomerId.ToString(),
                OrderDate = order.CreatedDate,
                TotalAmount = orderDto.TotalAmount,
                Status = orderDto.Status,
                OrderItems = orderDto.OrderItems
            };
        }
    }
} 