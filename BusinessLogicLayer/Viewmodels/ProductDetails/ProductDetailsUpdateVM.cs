using BusinessLogicLayer.Viewmodels.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ProductDetails
{
    public class ProductDetailsUpdateVM
    {
        public string? ModifiedBy { get; set; }
        public Guid? IDProduct { get; set; }
        public string? ProductName { get; set; }
        public Guid? IDCategory { get; set; }
        public string? CategoryName { get; set; }
        public Guid? IDManufacturers { get; set; }
        public string? ManufacturersName { get; set; }
        public Guid? IDMaterial { get; set; }
        public string? MaterialName { get; set; }
        public Guid? IDBrand { get; set; }
        public string? BrandName { get; set; }
        public string Description { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string Origin { get; set; } = null!; 
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public List<string> ImagePaths { get; set; }
        public List<OptionsCreateVM> NewOptions { get; set; }
    }
}
