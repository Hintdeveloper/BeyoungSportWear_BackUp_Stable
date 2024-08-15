using AutoMapper;
using BusinessLogicLayer.Viewmodels.CartProductDetails;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
