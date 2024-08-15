namespace BusinessLogicLayer.Viewmodels.CartProductDetails
{
    public class ProductDetailUser
    {
        public Guid ID { get; set; }
        public string IDOptions { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<string> Size { get; set; }
        public List<string> Color { get; set; }
        public string UrlImg { get; set; }
    }
}
