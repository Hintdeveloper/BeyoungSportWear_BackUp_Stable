using AutoMapper;
using BusinessLogicLayer.Viewmodels.Manufacturer;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class ManufacturerMap : Profile
    {
        public ManufacturerMap()
        {
            CreateMap<Manufacturer, ManufacturerVM>().ReverseMap();
        }
    }
}

