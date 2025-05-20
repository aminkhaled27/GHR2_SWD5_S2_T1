using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.IRepository
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetProductsAsync();
        public Task<List<Product>> GetAllProductsAsync();
        public Task DeleteProductAsync(Product product);
        public Task<Product> GetProductByIdAsync(int Id);
        public Task UpdateProductAsync(Product product);
        public Task CreateProductAsync(Product product);
        public Task<List<Product>> GetProductByCategoryNameAsync(string categoryName);
        public Task DeleteProductAsync(int Id);
        Task<List<Product>> GetProductByCategoryIdAsync(int categoryId);
        Task<List<Product>> GetAllAsync();
    }
}
