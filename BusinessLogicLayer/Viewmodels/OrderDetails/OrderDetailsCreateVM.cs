using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.OrderDetails
{
    public class OrderDetailsCreateVM
    {
        public string CreateBy { get; set; }
        public Guid IDOrder { get; set; }
        public Guid IDOptions { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public int Status { get; set; } = 1;
        public decimal TotalAmount
        {
            get
            {
                decimal discountValue = Discount ?? 0;

                return Quantity * UnitPrice * (1 - discountValue);
            }
        }
    }
}
