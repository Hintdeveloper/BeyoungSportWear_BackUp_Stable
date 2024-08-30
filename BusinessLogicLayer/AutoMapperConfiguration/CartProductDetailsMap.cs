using AutoMapper;
using BusinessLogicLayer.Viewmodels.CartOptions;
using DataAccessLayer.Entity;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class CartProductDetailsMap : Profile
    {
        public CartProductDetailsMap()
        {
            CreateMap<CartOptions, CartOptionsVM>()
                     .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Options.ProductDetails.Products.Name))
                     .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Options.ImageURL))
                     .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Options.Sizes.Name))
                     .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Options.Colors.Name))
                     .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }

}

