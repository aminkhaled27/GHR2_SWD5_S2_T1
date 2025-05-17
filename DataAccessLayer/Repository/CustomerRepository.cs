using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public  class CustomerRepository : ICustomerRepository
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
    }
}
