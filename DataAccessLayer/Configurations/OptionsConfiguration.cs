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
    public partial class OptionsConfiguration : IEntityTypeConfiguration<Options>
    {
        public void Configure(EntityTypeBuilder<Options> builder)
        {
            builder.HasKey(c => c.ID);
            builder.Property(c => c.CreateDate).IsRequired();
            builder.Property(c => c.CreateBy).IsRequired();
            builder.Property(c => c.ModifiedBy).IsRequired(false);
            builder.Property(c => c.ModifiedDate).IsRequired(false);
            builder.Property(c => c.DeleteBy).IsRequired(false);
            builder.Property(c => c.DeleteDate).IsRequired(false);
            builder.Property(c => c.Status).IsRequired();
            builder.HasOne<ProductDetails>(c => c.ProductDetails)
                .WithMany(c => c.Options)
                .HasForeignKey(c => c.IDProductDetails)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Colors>(c => c.Colors)
                .WithMany(c => c.Options)
                .HasForeignKey(c => c.IDColor)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Sizes>(c => c.Sizes)
                .WithMany(c => c.Options)
                .HasForeignKey(c => c.IDSize)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
