using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ActionRequests
{
    public class CreateProductActionRequest
    {
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

        [Required(ErrorMessage = "Product Image is Required")]
       // [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Only image files (jpg, jpeg, png, gif) are allowed")]
        [Display(Name = "Image")]
        public IFormFile ImagePath { get; set; }

        [Required(ErrorMessage = "Category is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
