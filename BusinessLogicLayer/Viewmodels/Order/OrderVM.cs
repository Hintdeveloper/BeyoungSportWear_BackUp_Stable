using BusinessLogicLayer.Viewmodels.OrderDetails;
using BusinessLogicLayer.Viewmodels.OrderHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Viewmodels.Order
{
    public class OrderVM
    {
        public Guid ID { get; set; }
        public DateTime CreateDate { get; set; }
        public string? VoucherCode { get; set; }
        public string HexCode { get; set; }
        public string? IDUser { get; set; }
        public string CustomerName { get; set; } = null!; // Tên khách hàng
        public string CustomerPhone { get; set; } = null!; // Số điện thoại khách hàng
        public string CustomerEmail { get; set; } = null!;// Địa chỉ email khách hàng
        public string FullNameUser { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? ShippingAddressLine2 { get; set; }
        public string? BillOfLadingCode { get; set; }
        public DateTime ShipDate { get; set; } = DateTime.UtcNow.AddDays(3);
        public decimal TotalAmount { get; set; }
        public decimal? Cotsts { get; set; }
        public string? Notes { get; set; }
        public bool TrackingCheck { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderType OrderType { get; set; }
        public int Status { get; set; } = 1;
        public List<OrderDetailsVM> OrderDetailsVM { get; set; }
        public List<OrderHistoryVM> OrderHistoryVM { get; set; }
    }
}
