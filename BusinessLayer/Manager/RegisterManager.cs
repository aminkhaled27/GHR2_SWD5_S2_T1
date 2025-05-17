using BusinessLayer.DTO.Account;
using BusinessLayer.Manager.IManager;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public RegisterManager(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager
            ,ICustomerRepository customerRepository,RoleManager<IdentityRole> roleManager)
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

            // 1. Create the new user
            var user = new ApplicationUser
            {
                UserName = registerDTO.Name,
                PasswordHash = registerDTO.Password
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return;
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }


            var customer = new Customer
            {
                Name = registerDTO.Name,
                Address = registerDTO.Email,
                UserId = user.Id,
                User = user
            };


            await _customerRepository.Add(customer);

         //   await _userManager.AddToRoleAsync(user, "Admin");

            await _signInManager.SignInAsync(user, isPersistent: false);


        }

        public async Task SaveUserAndCustomerAsAdmin(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                throw new ArgumentNullException(nameof(registerDTO));

            // Ensure the "Admin" role exists
            var roleExists = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Role creation failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            // 1. Create the new user
            var user = new ApplicationUser
            {
                UserName = registerDTO.Name,
                PasswordHash = registerDTO.Password
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // 2. Create the customer entity
            var customer = new Customer
            {
                Name = registerDTO.Name,
                Address = registerDTO.Email,
                UserId = user.Id,
                User = user
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


        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }



    }
}
