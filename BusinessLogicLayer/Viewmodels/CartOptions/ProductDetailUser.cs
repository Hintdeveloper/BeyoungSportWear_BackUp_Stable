using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Viewmodels.CartOptions
{
    public class ProductDetailUser
    {
        public Guid ID { get; set; }
        public string IDOptions { get; set; }
        public string KeyCode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public List<string> Size { get; set; }
        public List<string> Color { get; set; }
        public string UrlImg { get; set; }
    }
}
