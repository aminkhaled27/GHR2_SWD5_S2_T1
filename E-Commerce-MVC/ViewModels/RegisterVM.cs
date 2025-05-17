using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
