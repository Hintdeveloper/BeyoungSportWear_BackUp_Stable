using AutoMapper;
using BusinessLogicLayer.Viewmodels.Brand;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class BrandMap : Profile
    {
        public BrandMap()
        {
            CreateMap<Brand, BrandVM>().ReverseMap();
        }
    }
}
