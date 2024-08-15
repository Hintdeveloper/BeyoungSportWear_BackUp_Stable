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
    public partial class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            //BASE
            builder.HasKey(c => c.ID);
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne<ApplicationUser>(c=>c.ApplicationUsers)
                .WithOne(c=>c.Cart)
                .HasForeignKey<Cart>(c=>c.IDUser)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
