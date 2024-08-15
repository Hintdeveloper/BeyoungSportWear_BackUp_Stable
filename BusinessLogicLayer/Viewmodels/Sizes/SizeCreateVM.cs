using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Sizes
{
    public class SizeCreateVM
    {
        public string CreateBy { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Status { get; set; }
    }
}
