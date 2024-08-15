using AutoMapper;
using BusinessLogicLayer.Viewmodels.Address;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class AddressMap : Profile
    {
        public AddressMap()
        {
            CreateMap<Address, AddressVM>().ReverseMap();
        }
    }
}
