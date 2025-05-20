using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTO.Account
{
    public class RegisterDTO
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
