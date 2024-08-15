using AutoMapper;
using BusinessLogicLayer.Viewmodels.Colors;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class ColorsMap : Profile
    {
        public ColorsMap()
        {
            CreateMap<Colors, ColorVM>().ReverseMap();
        }
    }
}
