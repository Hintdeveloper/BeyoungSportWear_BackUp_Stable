using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Options
{
    public class DecreaseQuantityRequest
    {
        public Guid IDOptions { get; set; }
        public int QuantityToDecrease { get; set; }
    }
}
