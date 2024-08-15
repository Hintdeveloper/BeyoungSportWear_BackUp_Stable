using BusinessLogicLayer.Viewmodels.Address;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using BusinessLogicLayer.Viewmodels.Brand;
using BusinessLogicLayer.Viewmodels.Manufacturer;
using BusinessLogicLayer.Viewmodels.Material;
using BusinessLogicLayer.Viewmodels.Product;
using BusinessLogicLayer.Viewmodels.ProductDetails;

namespace PresentationLayer.Areas.Admin.Models
{
    public class CompositeViewModel
    {
        public List<AddressVM>? Locations { get; set; }
        public RegisterUser? RegisterUser {  get; set; }
        public SearchModel? SearchModel { get; set; }
        public List<UserDataVM>? UserDataVM { get; set; } 
        public UserLoginModel? UserLogin { get; set; } 
        public UserUpdateVM? UserUpdateVM { get; set; } 
        public ProductVM? ProductVM { get; set; } 
        public ManufacturerVM? ManufacturerVM { get; set; } 
        public MaterialVM? MaterialVM { get; set; } 
        public BrandVM? BrandVM { get; set; } 

        public ProductDetailsCreateVM? ProductDetailsCreateVM { get; set; }
    }
}
