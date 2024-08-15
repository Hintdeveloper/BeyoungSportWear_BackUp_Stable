using DataAccessLayer.Entity.Base;
namespace DataAccessLayer.Entity
{
    public partial class Images : EntityBase
    {
        public Guid IDProductDetails { get; set; }
        public string Path { get; set; } = null!;
        public virtual ProductDetails? ProductDetails { get; set; }

    }
}
