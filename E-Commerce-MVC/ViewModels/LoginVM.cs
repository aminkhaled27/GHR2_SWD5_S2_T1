using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ViewModels
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]       
        public string Password {set; get; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

    }
}
