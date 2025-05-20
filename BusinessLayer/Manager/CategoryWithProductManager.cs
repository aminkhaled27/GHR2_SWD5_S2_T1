using BusinessLayer.DTO.Product;
using BusinessLayer.DTO.Category;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Repository;
using DataAccessLayer.Repository.IRepository;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Manager
{
    public class CategoryWithProductManager : ICategoryWithProductManager
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryWithProductManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task AddCategory(CreateCategoryDTO categoryDTO)
        {
            var category = new Category
            {
                Name = categoryDTO.Name
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<List<CategoryWithProductDTO>> GetAllCategoryWithProducts()
        {
            return await GetAllCategoriesWithProductsAsync();
        }

        public async Task<List<CategoryWithProductDTO>> GetAllCategoriesWithProductsAsync()
        {
            var categoriesWithProductsFromRepo = await _categoryRepository.GetCategoriesWithProductsAsync();
 
            var categoryWithProductDTOs = new List<CategoryWithProductDTO>();

            foreach (var category in categoriesWithProductsFromRepo)
            {
                var productsDTO = new List<ProductCardDTO>();

                foreach (var product in category.Products)
                {
                    // Map each product to ProductCardDTO
                    var productDto = new ProductCardDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        ImagePath = product.ImagePath
                    };
                    productsDTO.Add(productDto);
                }

                var categoryDTO = new CategoryWithProductDTO
                {
                    CategoryName = category.Name,
                    Products = productsDTO 
                };

                categoryWithProductDTOs.Add(categoryDTO);
            }

            return categoryWithProductDTOs;
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            if (category.Products.Any())
            {
                throw new Exception("Cannot delete category that contains products. Please delete or move the products first.");
            }

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
