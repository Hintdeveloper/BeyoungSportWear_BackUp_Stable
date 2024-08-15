using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Manufacturer : EntityBase
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gmail { get; set; } = null!;
        public string Website { get; set; } = null!;

        public virtual ICollection<ProductDetails> ProductDetails { get; set; }
    }
}
