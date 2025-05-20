using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTO.Product;

namespace BusinessLayer.DTO.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductCardDTO> Products { get; set; } = new List<ProductCardDTO>();
    }

    //public class ProductCardDTO
    //{
    //    public string Name { get; set; }

    //    public string Description { get; set; }

    //    public decimal Price { get; set; }

    //    public string ImagePath { get; set; }
    //}
}
