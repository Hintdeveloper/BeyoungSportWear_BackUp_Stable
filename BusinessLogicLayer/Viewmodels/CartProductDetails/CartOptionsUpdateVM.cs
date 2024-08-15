using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.CartProductDetails
{
    public class CartOptionsUpdateVM
    {
        public string ModifiedBy { get; set; }
        public Guid IDOptions { get; set; }
        public Guid IDCart { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int Status { get; set; }
    }
}
