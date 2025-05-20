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
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            var Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == Id);

            return Product ?? throw new KeyNotFoundException($"Product with ID {Id} not found."); 
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var Products = await _context.Products.ToListAsync();

            return Products;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            if(product == null) throw new ArgumentNullException( nameof(product));

           await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();
        }


        public async Task<List<Product>> GetProductByCategoryNameAsync(string categoryName)
        {
            if(string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or empty", nameof(categoryName));
            }

            var category = await _context.Categories
                .Include(p => p.Products)
                .FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category == null)
            {
                return new List<Product>(); // Return empty list instead of throwing exception
            }

            return await _context.Products
                .Where(p => p.CategoryId == category.Id)
                .ToListAsync();
        }

        public async Task DeleteProductAsync(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
