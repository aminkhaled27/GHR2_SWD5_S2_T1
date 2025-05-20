using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Product
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

       
        public string ImagePath { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public List<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
    }


}
