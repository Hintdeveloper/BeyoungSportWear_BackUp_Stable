
namespace BusinessLogicLayer.Viewmodels.CartOptions
{
    public class CartOptionsCreateVM
    {
        public string CreateBy { get; set; }
        public Guid IDOptions { get; set; }
        public string IDCart { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int Status { get; set; }
    }
}
