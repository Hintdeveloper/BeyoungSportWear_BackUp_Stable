using AutoMapper;
using BusinessLogicLayer.Viewmodels.Sizes;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class SizesMap : Profile
    {
        public SizesMap()
        {
            CreateMap<Sizes, SizeVM>().ReverseMap();
        }
    }
}
