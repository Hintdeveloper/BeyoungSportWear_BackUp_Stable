using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class Colors : EntityBase
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<Options>? Options { get; set; }
    }
}
