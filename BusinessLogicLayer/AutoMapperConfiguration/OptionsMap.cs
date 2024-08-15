using AutoMapper;
using BusinessLogicLayer.Viewmodels.Options;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class OptionsMap : Profile
    {
        public OptionsMap()
        {
            CreateMap<Options, OptionsVM>()
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(erc => erc.Colors.Name))
                .ForMember(dest => dest.SizesName, opt => opt.MapFrom(erc => erc.Sizes.Name))
                .ReverseMap();
        }
    }
}
