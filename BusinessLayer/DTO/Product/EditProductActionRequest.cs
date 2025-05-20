using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.DTO.Product;

namespace BusinessLayer.DTO.Product
{
    public class EditProductActionRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Product Name can only contain letters, numbers, spaces, and hyphens")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Description is Required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Product Description must be between 10 and 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product Price is Required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImagePath { get; set; }

        [Required(ErrorMessage = "Category is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
} 