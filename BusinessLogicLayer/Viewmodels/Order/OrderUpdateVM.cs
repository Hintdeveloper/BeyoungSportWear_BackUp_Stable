using BusinessLogicLayer.Viewmodels.OrderDetails;

namespace BusinessLogicLayer.Viewmodels.Order
{
    public class OrderUpdateVM
    {
        public string? ModifieBy { get; set; }
        public string? IDUser { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? ShippingAddress { get; set; }
        public string? ShippingAddressLine2 { get; set; }
        public DateTime ShipDate { get; set; } = DateTime.UtcNow.AddDays(3);
        public string? Notes { get; set; }
        public decimal Cotsts { get; set; }
        public List<OrderDetailsUpdateVM> OrderDetails { get; set; } = new();
    }
}
