using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.IRepository
{
    public interface ICustomerRepository
    {
        public Task Add(Customer customer);
        public Task<Customer> GetCustomerByUserIdAsync(string userId);
        public Task<int> GetCartItemCountAsync(int customerId);
        public Task Update(Customer customer);
    }
}
