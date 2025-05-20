using BusinessLayer.DTO.Account;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Manager
{
    public class RegisterManager : IRegistrationManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterManager(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ICustomerRepository customerRepository, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepository = customerRepository;
            _roleManager = roleManager;
        }

        public async Task SaveUserAndCustomer(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                throw new ArgumentNullException(nameof(registerDTO));

            // Check if user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }

            // 1. Create the new user
            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FirstName = registerDTO.Name,
                LastName = string.Empty,
                ProfileImageUrl = "/images/default-profile.png", // Set a default profile image
                EmailConfirmed = true // Auto-confirm email for simplicity
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User creation failed: {errors}");
            }

            try
            {
                // Create customer record
                var customer = new Customer
                {
                    Name = registerDTO.Name,
                    Address = registerDTO.Email, // Using email as address for now
                    UserId = user.Id,
                    User = user // Set the navigation property
                };

                await _customerRepository.Add(customer);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            catch (Exception ex)
            {
                // If customer creation fails, delete the user
                await _userManager.DeleteAsync(user);
                throw new Exception($"Failed to create customer record: {ex.Message}");
            }
        }

        public async Task SaveUserAndCustomerAsAdmin(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                throw new ArgumentNullException(nameof(registerDTO));

            // Check if user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }

            // 1. Create the new user
            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FirstName = registerDTO.Name,
                LastName = string.Empty,
                ProfileImageUrl = "/images/default-profile.png", // Set a default profile image
                EmailConfirmed = true // Auto-confirm email for simplicity
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User creation failed: {errors}");
            }

            try
            {
                // 2. Create the customer entity
                var customer = new Customer
                {
                    Name = registerDTO.Name,
                    Address = registerDTO.Email, // Using email as address for now
                    UserId = user.Id,
                    User = user // Set the navigation property
                };

                await _customerRepository.Add(customer);

                // 3. Assign the "Admin" role to the user
                var roleAssignResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleAssignResult.Succeeded)
                {
                    throw new Exception("Failed to assign role: " + string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));
                }

                // 4. Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            catch (Exception ex)
            {
                // If customer creation fails, delete the user
                await _userManager.DeleteAsync(user);
                throw new Exception($"Failed to create customer record: {ex.Message}");
            }
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
