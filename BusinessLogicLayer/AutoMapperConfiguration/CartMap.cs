using AutoMapper;
using BusinessLogicLayer.Viewmodels.Cart;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class CartMap : Profile
    {
        public CartMap()
        {
            CreateMap<Cart, CartVM>().ReverseMap();
        }
    }
}
