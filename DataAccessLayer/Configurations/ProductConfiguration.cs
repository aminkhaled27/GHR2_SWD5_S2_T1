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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                 .ValueGeneratedOnAdd();

            builder.Property(p => p.Name).IsRequired();

            builder.Property(p => p.Name).HasMaxLength(100);

            builder.Property(p => p.Description).HasMaxLength(500);

            builder.Property(p => p.Price).HasPrecision(18, 2);

            builder.HasOne(p => p.Category).WithMany(c=>c.Products).HasForeignKey(p=>p.CategoryId);




        }
    }
}
