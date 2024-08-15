using DataAccessLayer.Entity.Base;
namespace DataAccessLayer.Entity
{
    public partial class CartOptions : NoIDEntityBase
    {
        public Guid IDOptions { get; set; }
        public string IDCart { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
        public virtual Options Options { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
    }
}
