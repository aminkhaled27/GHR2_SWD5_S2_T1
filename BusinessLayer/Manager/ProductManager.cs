using BusinessLayer.DTO.Category;
using BusinessLayer.DTO.Product;
using BusinessLayer.DTO;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using BusinessLayer.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace BusinessLayer.Manager
{
    public class ProductManager : IProductManager
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileServices _fileServices;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;

        public ProductManager(IProductRepository productRepository, ICategoryRepository categoryRepository, IFileServices fileServices, IMapper mapper, ICartRepository cartRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _fileServices = fileServices;
            _mapper = mapper;
            _cartRepository = cartRepository;
        }

        public async Task AddProduct(CreateProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                ImagePath = productDTO.ImagePath,
                CategoryId = productDTO.CategoryId
            };

            await _productRepository.CreateProductAsync(product);

        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetCategoriesAsync(); 
            var categoriesDTO = new List<CategoryDTO>();  

            foreach (var category in categories)
            {
                var products = await _productRepository.GetProductByCategoryIdAsync(category.Id);
                var productsDTO = products.Select(p => new ProductCardDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImagePath = p.ImagePath,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? "Uncategorized"
                }).ToList();

                categoriesDTO.Add(new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Products = productsDTO
                });
            }

            return categoriesDTO;
        }

        public async Task<List<ProductCardDTO>> GetAllProductByCategoryName(string CategoryName)
        {
            var productsFromRepo = await _productRepository.GetProductByCategoryNameAsync(CategoryName);
            var products = new List<ProductCardDTO>();

            foreach (var product in productsFromRepo)
            {
                var productDTO = new ProductCardDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImagePath = product.ImagePath,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name ?? "Uncategorized"
                };

                products.Add(productDTO);
            }

            return products;
        }

        public async Task<ProductCardDTO> GetProductByIdAsync(int Id)
        {
            var product = await _productRepository.GetProductByIdAsync(Id);

            var productDTO = new ProductCardDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImagePath = product.ImagePath,
                CategoryId = product.CategoryId
            };
            return productDTO;
        }

        public async Task Delete(int Id)
        {
          await  _productRepository.DeleteProductAsync(Id);
        }

        public async Task UpdateProduct(EditProductActionRequest model)
        {
            var product = await _productRepository.GetProductByIdAsync(model.Id);
            if (product == null) throw new Exception("Product not found.");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            // Handle image update if a new image is uploaded
            if (model.ImagePath != null)
            {
                var uniqueFileName = _fileServices.UploadFile(model.ImagePath, "Product/");
                product.ImagePath = uniqueFileName;
            }

            await _productRepository.UpdateProductAsync(product);
        }

        public async Task<List<FeaturedProductDTO>> GetFeaturedProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            
            // Get 3 random products to feature
            var random = new Random();
            var featuredProducts = products
                .OrderBy(x => random.Next())
                .Take(3)
                .Select(p => new FeaturedProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImagePath = p.ImagePath,
                    IsFeatured = true
                })
                .ToList();

            return featuredProducts;
        }

        public async Task<List<ProductCardDTO>> GetAllProductByCategoryId(int categoryId)
        {
            var productsFromRepo = await _productRepository.GetProductByCategoryIdAsync(categoryId);
            var products = new List<ProductCardDTO>();

            foreach (var product in productsFromRepo)
            {
                var productDTO = new ProductCardDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImagePath = product.ImagePath,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name ?? "Uncategorized"
                };

                products.Add(productDTO);
            }

            return products;
        }

        public async Task<List<ProductCardDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<List<ProductCardDTO>>(products);
        }

        public async Task<int> GetCartCount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            var cart = await _cartRepository.GetCartByUserId(userId);
            if (cart == null)
                return 0;

            return cart.CartItems?.Count ?? 0;
        }
    }
}
