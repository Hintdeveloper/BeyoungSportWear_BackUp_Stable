using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Image
{
    public class ImageVM
    {
        public string Hash { get; set; }
        public string CreateBy { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public Guid ID { get; set; }
        public Guid? IDProductDetails { get; set; }
        public string Path { get; set; } = null!;
        public int Status { get; set; }
    }
}
