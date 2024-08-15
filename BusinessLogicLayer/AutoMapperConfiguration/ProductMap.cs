using AutoMapper;
using BusinessLogicLayer.Viewmodels.Product;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class ProductMap : Profile
    {
        public ProductMap()
        {
            CreateMap<Product, ProductVM>().ReverseMap();
        }
    }
}
