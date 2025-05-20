using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.DTO.User;
using BusinessLayer.DTO.Order;
using BusinessLayer.Services.IServices;
using E_Commerce_MVC.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Entities;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace E_Commerce_MVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryWithProductManager _categoryManager;
        private readonly ICustomerRepository _customerRepository;

        public ProfileController(
            IUserService userService, 
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            ICategoryWithProductManager categoryManager,
            ICustomerRepository customerRepository)
        {
            _userService = userService;
            _orderService = orderService;
            _userManager = userManager;
            _categoryManager = categoryManager;
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);
            var orders = await _orderService.GetUserOrdersAsync(userId);

            // Get categories for the navbar
            ViewBag.AllCategories = await _categoryManager.GetAllCategoriesWithProductsAsync();

            // Get cart count
            var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
            if (customer != null)
            {
                ViewBag.CartCount = await _customerRepository.GetCartItemCountAsync(customer.Id);
            }

            var viewModel = new ProfileViewModel
            {
                User = user,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(IFormFile profileImage)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userService.UpdateProfileImageAsync(userId, profileImage);
                
                if (result)
                {
                    TempData["Success"] = "Profile image updated successfully!";
                }
                else
                {
                    TempData["Danger"] = "Failed to update profile image.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserInfo(string fullName, string email, string phoneNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Check if email is being changed and if it's already in use
                if (email != user.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        TempData["Danger"] = "Email is already in use by another account.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Split full name into first and last name
                var nameParts = fullName.Split(new[] { ' ' }, 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                user.Email = email;
                user.UserName = email; // Update username to match email
                user.PhoneNumber = phoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update the customer record if it exists
                    var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
                    if (customer != null)
                    {
                        customer.Name = fullName;
                        customer.Address = email;
                        await _customerRepository.Update(customer);
                    }

                    TempData["Success"] = "Profile information updated successfully!";
                }
                else
                {
                    TempData["Danger"] = "Failed to update profile information: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["Danger"] = "All password fields are required.";
                return RedirectToAction(nameof(Index));
            }

            if (newPassword != confirmPassword)
            {
                TempData["Danger"] = "New password and confirmation password do not match.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result.Succeeded)
                {
                    // Sign out the user and sign them back in with the new password
                    await _userManager.UpdateSecurityStampAsync(user);
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    await _userManager.UpdateAsync(user);
                    
                    TempData["Success"] = "Password changed successfully! Please log in with your new password.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["Danger"] = "Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 