
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Viewmodels.CartOptions
{
    public class CartOptionsVM
    {
        public Guid IDOptions { get; set; }
        public DateTime CreateDate { get; set; }
        public string IDCart { get; set; }
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public string SizeName { get; set; }
        public string ColorName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
