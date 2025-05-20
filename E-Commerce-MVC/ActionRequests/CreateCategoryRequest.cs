using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ActionRequests
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
} 