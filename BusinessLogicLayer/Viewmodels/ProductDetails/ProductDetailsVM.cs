using BusinessLogicLayer.Viewmodels.Image;
using BusinessLogicLayer.Viewmodels.Options;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ProductDetails
{
    public class ProductDetailsVM
    {
        public string KeyCode { get; set; }
        public Guid ID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? IDProduct { get; set; }
        public string? ProductName { get; set; }
        public Guid? IDCategory { get; set; }
        public string? CategoryName { get; set; }
        public string? ManufacturersName { get; set; }
        public string? MaterialName { get; set; }
        public string? BrandName { get; set; }
        public decimal SmallestPrice { get; set; }
        public decimal BiggestPrice { get; set; }
        public int TotalQuantity { get; set; }
        public string Description { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public List<string> ImagePaths { get; set; }
        public int Status { get; set; }
        public List<OptionsVM> Options { get; set; }
        public List<ImageVM> ImageVM { get; set; }
        public bool IsActive { get; set; }
        //public string? Barcode { get; set; }
    }
}
