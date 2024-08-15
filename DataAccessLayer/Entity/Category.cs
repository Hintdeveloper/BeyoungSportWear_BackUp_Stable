using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Category : EntityBase
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<ProductDetails> ProductDetails { get; set; }
    }
}
