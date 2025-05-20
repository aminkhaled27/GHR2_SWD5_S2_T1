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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                 .ValueGeneratedOnAdd();

            builder.Property(c => c.Name).IsRequired();

            builder.Property(c => c.Name).HasMaxLength(100);

            builder.Property(c => c.Address).IsRequired();

            builder.Property(c => c.Address).HasMaxLength(150);

            builder.HasOne(a => a.User).WithOne(b => b.Customer).HasForeignKey<Customer>(c => c.UserId);


        }
    }
}
