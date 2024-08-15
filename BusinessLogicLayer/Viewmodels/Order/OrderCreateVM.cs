using BusinessLogicLayer.Viewmodels.OrderDetails;
using DataAccessLayer.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Viewmodels.Order
{
    public class OrderCreateVM
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string? HexCode { get; set; }
        public string CreateBy { get; set; }
        public string? IDUser { get; set; }
        public string CustomerName { get; set; } = null!; // Tên khách hàng
        public string CustomerPhone { get; set; } = null!; // Số điện thoại khách hàng
        public string CustomerEmail { get; set; } = null!;// Địa chỉ email khách hàng
        public string ShippingAddress { get; set; } = null!;
        public string? ShippingAddressLine2 { get; set; }
        public DateTime ShipDate { get; set; } = DateTime.UtcNow.AddDays(3);
        public decimal? TotalAmount { get; set; }
        public decimal? Cotsts { get; set; }
        public string? VoucherCode { get; set; }
        public string? Notes { get; set; }
        public bool TrackingCheck { get; set; } 
        public PaymentMethod PaymentMethods { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingMethod ShippingMethods { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int Status { get; set; } = 1; 
        public List<OrderDetailsCreateVM> OrderDetailsCreateVM { get; set; }
        public OrderType OrderType { get; set; }

        //public string GenerateHexCode()
        //{
        //    using (var context = new ApplicationDBContext())
        //    {
        //        string hexString;
        //        int randomPart;

        //        do
        //        {
        //            var now = DateTime.Now;
        //            var dateString = now.ToString("yyyyMMddHHmmss");

        //            var random = new Random();
        //            randomPart = random.Next(1000, 9999);

        //            hexString = dateString + randomPart.ToString();
        //        } while (context.Order.Any(o => o.HexCode == hexString));

        //        return hexString;
        //    }
        //}

    }
}
