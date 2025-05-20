using BusinessLayer.DTO.User;
using BusinessLayer.DTO.Order;
using System.Collections.Generic;

namespace E_Commerce_MVC.ViewModels
{
    public class ProfileViewModel
    {
        public UserDTO User { get; set; }
        public List<OrderDTO> Orders { get; set; }
    }
} 