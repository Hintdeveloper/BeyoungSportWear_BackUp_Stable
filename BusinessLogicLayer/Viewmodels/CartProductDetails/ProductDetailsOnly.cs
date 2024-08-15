
namespace BusinessLogicLayer.Viewmodels.CartProductDetails
{
	public class ProductDetailsOnly
	{
		public Guid ID { get; set; }
		public DateTime CreateDate { get; set; }
		public string KeyCode { get; set; }
		public string? ProductName { get; set; }
		public string? CategoryName { get; set; }
		public string? ManufacturersName { get; set; }
		public string? MaterialName { get; set; }
		public string? BrandName { get; set; }
		public decimal SmallestPrice { get; set; }
		public decimal BiggestPrice { get; set; }
		public int TotalQuantity { get; set; }
		public decimal RetailPrice_Only { get; set; }
		public decimal Quantity_Only { get; set; }
		public string Description { get; set; } = null!;
		public string Style { get; set; } = null!;
		public string Origin { get; set; } = null!;
		public List<string> ProductDetails_ImagePaths { get; set; }
		public bool IsActive { get; set; }
		//=========================================//
		public string IDOptions { get; set; }
		public List<string> Size { get; set; }
		public List<string> Color { get; set; }
		public string ImageURL_Only { get; set; }
		public int Status { get; set; }

	}
}
