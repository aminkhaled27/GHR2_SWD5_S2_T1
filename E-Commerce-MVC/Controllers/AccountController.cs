using BusinessLayer.DTO.Account;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Entities;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IRegistrationManager _registrationManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IRegistrationManager registrationManager,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _registrationManager = registrationManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SaveRegister(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var RegisterDTO = new RegisterDTO
                {
                    Name = registerVM.UserName,
                    Password = registerVM.Password,
                    Email = registerVM.Email,
                };

                try
                {
                    await _registrationManager.SaveUserAndCustomer(RegisterDTO);
                    TempData["Success"] = "Registration successful!";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    TempData["Danger"] = "Registration failed: " + ex.Message;
                }
            }

            return View("Register", registerVM);
        }

        public async Task<IActionResult> Logout()
        {
            // Clear the cart from session
            HttpContext.Session.Remove("Cart");
            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> SaveLogin(LoginVM loginUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginUserViewModel.Email);
                if (user != null)
                {
                    bool isPasswordMatched = await _userManager
                           .CheckPasswordAsync(user, loginUserViewModel.Password);

                    if (isPasswordMatched == true)
                    {
                        await _signInManager.SignInAsync(user, loginUserViewModel.RememberMe);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email or Password wrong");
                    }
                }

                ModelState.AddModelError("", "Email or Password wrong");
            }

            return View("Login", loginUserViewModel);
        }
    }
}
