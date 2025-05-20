using BusinessLayer.DTO.User;
using BusinessLayer.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _uploadPath;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ProfilePictures");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = DateTime.Now
            };
        }

        public async Task<bool> UpdateProfileImageAsync(string userId, IFormFile profileImage)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                if (profileImage != null && profileImage.Length > 0)
                {
                    // Generate unique filename
                    var fileName = $"{userId}_{DateTime.Now.Ticks}{Path.GetExtension(profileImage.FileName)}";
                    var filePath = Path.Combine(_uploadPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }

                    // Update user's profile image URL
                    var imageUrl = $"/Images/ProfilePictures/{fileName}";
                    user.ProfileImageUrl = imageUrl;
                    await _userManager.UpdateAsync(user);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 