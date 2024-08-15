using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entity
{
    public partial class ApplicationUser : IdentityUser
    {
        public string FirstAndLastName { get; set; } = null!;
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Images { get; set; }
        public DateTime JoinDate{ get; set; }
        public int Status { get; set; }
        public virtual ICollection<Address>? Addresss { get; set; }
        public virtual ICollection<VoucherUser> VoucherUser { get; set; } = null!;
        public virtual Cart Cart { get; set; }
    }
}
