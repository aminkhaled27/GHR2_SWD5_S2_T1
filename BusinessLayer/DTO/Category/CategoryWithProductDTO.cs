using BusinessLayer.DTO.Product;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTO.Category
{
    public class CategoryWithProductDTO
    {
        public string CategoryName { get; set; }

        public List<ProductCardDTO> Products { get; set; }
    }

   
}
