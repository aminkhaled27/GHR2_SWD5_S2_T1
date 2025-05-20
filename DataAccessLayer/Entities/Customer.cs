using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Customer
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string UserId { get; set; }

        public ApplicationUser? User {  get; set; }  

       public List<Order>? orders { get; set; } = new List<Order>();


    }
}
