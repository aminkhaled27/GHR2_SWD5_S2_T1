using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class ProductOrderConfiguration : IEntityTypeConfiguration<ProductOrder>
    {
        public void Configure(EntityTypeBuilder<ProductOrder> builder)
        {

            // Composite Key
            builder.HasKey(p => new
            {
                p.ProductId , p.OrderId
            });

            builder.HasOne(p=>p.Product).WithMany(p=>p.ProductOrders).HasForeignKey(p=>p.ProductId);

            builder.HasOne(p => p.Order).WithMany(p => p.ProductOrders).HasForeignKey(p => p.OrderId);



        }
    }
}
