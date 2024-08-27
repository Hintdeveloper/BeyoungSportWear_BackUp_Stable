using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class ProductDetails : EntityBase
    {
        public Guid IDProduct { get; set; }
        public Guid IDManufacturers { get; set; }
        public Guid IDMaterial { get; set; }
        public Guid IDBrand { get; set; }
        public Guid IDCategory { get; set; }
        public string KeyCode { get; set; }
        public string Description { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public bool IsActive { get; set; }
        //public string BarCode { get; set; }

        public virtual Product Products { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
        public virtual Brand Brand { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Images> Images { get; set; } = null!;
        public virtual ICollection<Options> Options { get; set; } = null!;
        public virtual Manufacturer Manufacturers { get; set; } = null!;
    }
}
