using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Product
{
    public class ProductUpdateVM
    {
        public string? ModifiedBy { get; set; }
        public string Name { get; set; } = null!;
        public Guid IDCategory { get; set; }
        public string CategoryName { get; set; }

        public string? Description { get; set; }
        public int Status { get; set; }
    }
}
