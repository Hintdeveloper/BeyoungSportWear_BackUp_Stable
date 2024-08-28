using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.OrderDetails
{
    public class OrderDetailsVM
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid IDOrder { get; set; }
        public Guid IDOptions { get; set; }
        public string ProductName { get; set; } = null!;
        public string? SizeName { get; set; }
        public string? ColorName { get; set; }
        public string ImageURL { get; set; }
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal UnitPrice { get; set; } // Giá của một đơn vị sản phẩm
        public decimal? Discount { get; set; } // Mức giảm giá (nếu có)
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
    }
}
