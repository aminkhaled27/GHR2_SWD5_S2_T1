using BusinessLayer.DTO.Category;
using BusinessLayer.DTO.Product;
using BusinessLayer.Manager.IManager;
using BusinessLayer.Services.IServices;
using E_Commerce_MVC.ActionRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IProductManager _productManager;
        private readonly IFileServices _fileServices;
        private readonly ICategoryWithProductManager _categoryManager;

        public AdminController(
            IProductManager productManager, 
            IFileServices fileServices,
            ICategoryWithProductManager categoryManager)
        {
            _productManager = productManager;
            _fileServices = fileServices;
            _categoryManager = categoryManager;
        }

        public async Task<IActionResult> Dashboard(int? categoryId = null)
        {
            var products = categoryId.HasValue 
                ? await _productManager.GetAllProductByCategoryId(categoryId.Value)
                : await _productManager.GetAllProductsAsync();
            
            var categories = await _productManager.GetAllCategories();
            
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var categories = await _productManager.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductActionRequest productFromReq)
        {
            if (productFromReq.ImagePath == null)
            {
                ModelState.AddModelError("ImagePath", "Please upload an image.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _productManager.GetAllCategories();
                TempData["Danger"] = "Failed to add product. Please check the form.";
                return View(productFromReq);
            }

            var uniqueFileName = _fileServices.UploadFile(productFromReq.ImagePath, "Product/");

            var productDTO = new CreateProductDTO
            {
                Name = productFromReq.Name,
                Description = productFromReq.Description,
                Price = productFromReq.Price,
                ImagePath = uniqueFileName,
                CategoryId = productFromReq.CategoryId
            };

            await _productManager.AddProduct(productDTO);
            TempData["Success"] = "Product added successfully!";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productManager.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new EditProductRequest
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            ViewBag.Categories = await _productManager.GetAllCategories();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _productManager.GetAllCategories();
                TempData["Danger"] = "Failed to update product. Please check the form.";
                return View(model);
            }

            var updateDto = new BusinessLayer.DTO.Product.EditProductActionRequest
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                ImagePath = model.ImagePath // This will be null if no new image is uploaded
            };

            try
            {
                await _productManager.UpdateProduct(updateDto);
                TempData["Success"] = "Product updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Danger"] = $"Failed to update product: {ex.Message}";
                ViewBag.Categories = await _productManager.GetAllCategories();
                return View(model);
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productManager.Delete(id);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryRequest model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Danger"] = "Failed to add category. Please check the form.";
                return View(model);
            }

            try
            {
                var categoryDTO = new CreateCategoryDTO
                {
                    Name = model.Name
                };

                await _categoryManager.AddCategory(categoryDTO);
                TempData["Success"] = "Category added successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Danger"] = $"Failed to add category: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryManager.DeleteCategory(id);
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Danger"] = $"Failed to delete category: {ex.Message}";
            }
            return RedirectToAction("Dashboard");
        }
    }
} 