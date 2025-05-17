using BusinessLayer.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Manager.IManager
{
    public interface IRegistrationManager
    {
        public Task SaveUserAndCustomer(RegisterDTO registerDTO);
        public Task SaveUserAndCustomerAsAdmin(RegisterDTO registerDTO);
        public Task Logout();
    }
}
