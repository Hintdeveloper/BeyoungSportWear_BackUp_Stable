using AutoMapper;
using BusinessLogicLayer.Viewmodels.Material;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class MaterialMap : Profile
    {
        public MaterialMap()
        {
            CreateMap<Material, MaterialVM>().ReverseMap();

        }
    }
}
