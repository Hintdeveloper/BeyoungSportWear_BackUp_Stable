using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Brand
{
    public class BrandCreateVM
    {
        public string CreateBy { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 chữ số.")]
        public string Phone { get; set; } = null!;
        public string Gmail { get; set; } = null!;
        public string Website { get; set; } = null!;
        public int Status { get; set; }
    }
}
