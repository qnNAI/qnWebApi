using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations {

    public class ProductConfiguration : IEntityTypeConfiguration<Product> {

        public void Configure(EntityTypeBuilder<Product> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(128);
        }
    }
}
