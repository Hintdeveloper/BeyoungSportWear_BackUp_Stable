using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Cart
{
    public class CartCreateVM
    {
        public string? Description { get; set; }
        public string IDUser { get; set; } = null!;
        public int Status { get; set; }
    }
}
