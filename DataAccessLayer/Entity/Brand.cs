using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Brand : EntityBase
    {
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public string? Address { get; set; } 
        public string? Phone { get; set; } 
        public string? Gmail { get; set; } 
        public string? Website { get; set; } 
        public virtual ICollection<ProductDetails>? ProductDetails { get; set; } 

    }
}
