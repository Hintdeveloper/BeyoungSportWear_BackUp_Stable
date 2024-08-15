using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Cart
{
    public class CartUpdateVM
    {
        public string? Description { get; set; }
        public string? IDUser { get; set; }
        public int Status { get; set; }
    }
}
