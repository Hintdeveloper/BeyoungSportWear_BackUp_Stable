using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Brand
{
    public class BrandVM
    {
        public Guid ID { get; set; } 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gmail { get; set; } = null!;
        public string Website { get; set; } = null!;
        public int Status { get; set; }
    }
}
