using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ProductDetails
{
    public class ProductOptionVM
    {
        public Guid ID { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal RetailPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
