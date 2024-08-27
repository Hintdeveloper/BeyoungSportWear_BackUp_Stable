using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Order
{
    public class OrderResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
