using BusinessLayer.DTO.Category;
using BusinessLayer.DTO.Product;
using BusinessLayer.DTO;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Manager.IManager
{
    public interface IProductManager
    {
        public Task AddProduct(CreateProductDTO productDTO);
        public Task<List<CategoryDTO>> GetAllCategories();
        public Task<List<ProductCardDTO>> GetAllProductByCategoryName(string CategoryName);
        public Task<ProductCardDTO> GetProductByIdAsync(int Id);
        public Task Delete(int Id);
        Task UpdateProduct(EditProductActionRequest model);
        Task<List<FeaturedProductDTO>> GetFeaturedProducts();
        Task<List<ProductCardDTO>> GetAllProductByCategoryId(int categoryId);
        Task<List<ProductCardDTO>> GetAllProductsAsync();
        Task<int> GetCartCount(string userId);
    }
}
