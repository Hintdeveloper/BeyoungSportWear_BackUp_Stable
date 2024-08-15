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
    public partial class VoucherUserConfiguration : IEntityTypeConfiguration<VoucherUser>
    {
        public void Configure(EntityTypeBuilder<VoucherUser> builder)
        {
            builder.HasKey(c => new {c.IDUser, c.IDVoucher});
            builder.Property(c => c.CreateDate).IsRequired();
            builder.Property(c => c.CreateBy).IsRequired();
            builder.Property(c => c.ModifiedBy).IsRequired(false);
            builder.Property(c => c.ModifiedDate).IsRequired(false);
            builder.Property(c => c.DeleteBy).IsRequired(false);
            builder.Property(c => c.DeleteDate).IsRequired(false);
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne<ApplicationUser>(c => c.ApplicationUser).WithMany(c => c.VoucherUser).HasForeignKey(c => c.IDUser).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Voucher>(c => c.Voucher).WithMany(c => c.VoucherUser).HasForeignKey(c => c.IDVoucher).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
