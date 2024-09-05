using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Order
{
    public class UpdateOrderStatusRequest
    {
        public int Status { get; set; }
        public string IDUser { get; set; }
        public string BillOfLadingCode { get; set; }
    }
}
