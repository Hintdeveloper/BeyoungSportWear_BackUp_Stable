using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public partial class ProductDetailsConfiguration : IEntityTypeConfiguration<ProductDetails>
    {
        public void Configure(EntityTypeBuilder<ProductDetails> builder)
        {
            builder.HasKey(c => c.ID);
            builder.Property(c => c.CreateDate).IsRequired();
            builder.Property(c => c.CreateBy).IsRequired();
            builder.Property(c => c.ModifiedBy).IsRequired(false);
            builder.Property(c => c.ModifiedDate).IsRequired(false);
            builder.Property(c => c.DeleteBy).IsRequired(false);
            builder.Property(c => c.DeleteDate).IsRequired(false);
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne<Product>(c=>c.Products).WithMany(c=>c.ProductDetails).HasForeignKey(c=>c.IDProduct).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Manufacturer>(c=>c.Manufacturers).WithMany(c=>c.ProductDetails).HasForeignKey(c=>c.IDManufacturers).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Material>(c=>c.Material).WithMany(c=>c.ProductDetails).HasForeignKey(c=>c.IDMaterial).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Brand>(c=>c.Brand).WithMany(c=>c.ProductDetails).HasForeignKey(c=>c.IDBrand).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Category>(c=>c.Category).WithMany(c=>c.ProductDetails).HasForeignKey(c=>c.IDCategory).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
