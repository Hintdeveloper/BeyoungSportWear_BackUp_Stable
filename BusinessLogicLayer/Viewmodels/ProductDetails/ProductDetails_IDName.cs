using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ProductDetails
{
    public class ProductDetails_IDName
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<string> Sizes { get; set; }  // Danh sách các kích thước
        public List<string> Colors { get; set; } // Danh sách các màu sắc
    }
}
