using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.VNPay
{
    public class RefundRequestModel
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
    }

}
