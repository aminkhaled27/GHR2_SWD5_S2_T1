using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.IRepository
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<Category>> GetCategoriesWithProductsAsync();

        public Task AddAsync(Category category);

        public Task SaveChangesAsync();

        public Task<Category> GetCategoryByIdAsync(int id);

        public Task DeleteAsync(Category category);
    }
}
