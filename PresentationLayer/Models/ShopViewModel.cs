using BusinessLogicLayer.Viewmodels.Category;
using BusinessLogicLayer.Viewmodels.ProductDetails;

namespace PresentationLayer.Models
{
    public class ShopViewModel
    {
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public List<ProductDetailsVM> Products { get; set; } = new List<ProductDetailsVM>();
        public Guid? SelectedCategoryID { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
