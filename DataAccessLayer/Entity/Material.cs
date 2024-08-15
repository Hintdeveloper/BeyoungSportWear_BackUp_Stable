using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Material : EntityBase
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<ProductDetails> ProductDetails { get; set; } = null!;

    }
}
