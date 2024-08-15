using DataAccessLayer.Entity.Base;

namespace DataAccessLayer.Entity
{
    public partial class OrderDetails : EntityBase
    {
        public Guid IDOrder { get; set; }
        public Guid IDOptions { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }

        public virtual Order Order { get; set; } 
        public virtual Options Options { get; set; } 
    }
}
