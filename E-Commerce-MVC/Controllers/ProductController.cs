using BusinessLayer.DTO;
using BusinessLayer.DTO.Product;
using BusinessLayer.Manager;
using BusinessLayer.Manager.IManager;
using BusinessLayer.Services;
using BusinessLayer.Services.IServices;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using E_Commerce_MVC.ActionRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce_MVC.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IFileServices _fileServices;
        private readonly IProductManager _productManager;
        private readonly ICategoryWithProductManager _categoryManager;

        public ProductController(IFileServices fileServices, IProductManager productManager, ICategoryWithProductManager categoryManager)
        {
            _fileServices = fileServices;
            _productManager = productManager;
            _categoryManager = categoryManager;
        }

        public async Task<IActionResult> Index(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                var products = await _productManager.GetAllProductByCategoryName(category);
                return View("ShowProducts", products);
            }
            return View();
        }

        // GET: Product/AddProduct
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var categories = await _productManager.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }


        // POST: Product/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductActionRequest ProductFromReq)
        {
            if (ProductFromReq.ImagePath == null)
            {
                ModelState.AddModelError("ImagePath", "Please upload an image.");
            }

            if (!ModelState.IsValid)
            {
                // Log or inspect ModelState errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewBag.Categories = await _productManager.GetAllCategories();
                TempData["Danger"] = "Failed to add product. Please check the form.";
                return View(ProductFromReq);
            }
            if (ModelState.IsValid)
            {
                var UniqueFileName = _fileServices.UploadFile(ProductFromReq.ImagePath, "Product/");

                // Mapping From ProductFromRequest to Product DTO
                var ProductDTO = new CreateProductDTO
                {
                    Name = ProductFromReq.Name,
                    Description = ProductFromReq.Description,
                    Price = ProductFromReq.Price,
                    ImagePath = UniqueFileName,
                    CategoryId = ProductFromReq.CategoryId
                };

                await _productManager.AddProduct(ProductDTO);
                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("AddProduct");
            }

            ViewBag.Categories = await _productManager.GetAllCategories();
            return View("AddProduct");
        }
     

        [HttpGet]
        [Route("Product/Category/{CategoryName}")]
        public async Task<IActionResult> GetProductByCategoryName(string CategoryName)
        {
            if (CategoryName == null || CategoryName == string.Empty)
            {
                throw new ArgumentNullException(nameof(CategoryName));
            }

            var Products = await _productManager.GetAllProductByCategoryName(CategoryName);
            var categories = await _categoryManager.GetAllCategoryWithProducts();
            ViewBag.AllCategories = categories;
            return View("ShowProducts", Products);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int ProductId)
        {   
            await _productManager.Delete(ProductId);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productManager.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var model = new EditProductActionRequest
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                // ImagePath is not set here (handled separately)
            };
            ViewBag.Categories = await _productManager.GetAllCategories();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductActionRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _productManager.GetAllCategories();
                TempData["Danger"] = "Failed to update product. Please check the form.";
                return View(model);
            }
            await _productManager.UpdateProduct(model);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productManager.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Get similar products from the same category using CategoryId
            var similarProducts = await _productManager.GetAllProductByCategoryId(product.CategoryId);
            // Exclude the current product and take up to 4 similar products
            similarProducts = similarProducts.Where(p => p.Id != id).Take(4).ToList();
            
            // Add categories for the dropdown
            var categories = await _categoryManager.GetAllCategoryWithProducts();
            ViewBag.AllCategories = categories;

            // Add cart count
            var cartCount = await _productManager.GetCartCount(User.Identity.Name);
            ViewBag.CartCount = cartCount;
            
            ViewBag.SimilarProducts = similarProducts;
            return View(product);
        }
    }
}
