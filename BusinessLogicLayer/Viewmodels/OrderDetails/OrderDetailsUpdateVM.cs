using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.OrderDetails
{
    public class OrderDetailsUpdateVM
    {
        public string? ModifiedBy { get; set; }
        public Guid IDOrder { get; set; }
        public Guid IDOptions { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; } = 1;
    }
}
