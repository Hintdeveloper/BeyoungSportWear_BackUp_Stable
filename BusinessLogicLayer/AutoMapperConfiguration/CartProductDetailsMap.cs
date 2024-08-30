using AutoMapper;
using BusinessLogicLayer.Viewmodels.CartOptions;
using DataAccessLayer.Entity;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class CartProductDetailsMap : Profile
    {
        public CartProductDetailsMap()
        {
            CreateMap<CartOptions, CartOptionsVM>().ReverseMap();
        }
    }
}