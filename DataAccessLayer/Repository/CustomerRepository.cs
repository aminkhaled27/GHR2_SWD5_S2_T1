using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Customer customer)
        {
            if(customer == null) throw new ArgumentNullException(nameof(customer));

            await _context.Customers.AddAsync(customer);

            await _context.SaveChangesAsync();
        }

        public async Task<Customer> GetCustomerByUserIdAsync(string userId)
        {
            return await _context.Customers
                .Include(c => c.orders)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<int> GetCartItemCountAsync(int customerId)
        {
            // This is a placeholder implementation
            // You should implement the actual cart item count logic based on your cart system
            return 0;
        }

        public async Task Update(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
