using BusinessLayer.DTO.User;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Services.IServices
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(string userId);
        Task<bool> UpdateProfileImageAsync(string userId, IFormFile profileImage);
    }
} 