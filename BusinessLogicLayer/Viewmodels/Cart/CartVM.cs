using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Cart
{
    public class CartVM
    {
        public string ID { get; set; }
        public string? Description { get; set; }
        public string? IDUser { get; set; }
        public int Status { get; set; }
    }
}
