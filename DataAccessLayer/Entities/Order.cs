using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public Customer Customer { get; set; }

        public List<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    }
}
