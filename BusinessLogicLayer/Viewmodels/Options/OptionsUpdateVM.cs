using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Options
{
    public class OptionsUpdateVM
    {
        public Guid ID { get; set; }
        public string ModifiedBy { get; set; }
        public Guid? IDColor { get; set; }
        public string ColorName { get; set; }
        public Guid? IDSize { get; set; }
        public string SizesName { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal RetailPrice { get; set; }
        public IFormFile? ImageURL { get; set; }
        public bool IsActive { get; set; }
    }
}
