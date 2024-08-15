using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Cart 
    {
        public string ID { get; set; }
        public string? Description { get; set; }
        public string? IDUser { get; set; }
        public int Status { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; } = null!;
        public virtual ICollection<CartOptions>? CartOptions { get; set; } 
    }
}
