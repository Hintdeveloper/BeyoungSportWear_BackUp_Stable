using DataAccessLayer.Entity.Base;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace DataAccessLayer.Entity
{
    public partial class Order : EntityBase
    {
        public string HexCode { get; set; }
        public string? IDUser { get; set; }
        public string CustomerName { get; set; } = null!; 
        public string CustomerPhone { get; set; } = null!; 
        public string CustomerEmail { get; set; } = null!;
        public string? ShippingAddress { get; set; } = null!;
        public string? ShippingAddressLine2 { get; set; }
        public DateTime ShipDate { get; set; } = DateTime.UtcNow.AddDays(3);
        public decimal TotalAmount { get; set; }
        public decimal Cotsts { get; set; }
        public string? VoucherCode { get; set; }
        public string? Notes { get; set; }
        public bool TrackingCheck { get; set; } = false; 
        public PaymentMethod PaymentMethods { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingMethod ShippingMethods { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderType OrderType { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } 
        public virtual ICollection<OrderHistory>? OrderHistory { get; set; } 
       
    }
}
