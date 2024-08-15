using AutoMapper;
using BusinessLogicLayer.Viewmodels.Voucher;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class VoucherMap : Profile
    {
        public VoucherMap()
        {
            CreateMap<Voucher, VoucherVM>()
                 .ForMember(dest => dest.IDUser, opt => opt.MapFrom(src => src.VoucherUser.Select(c => c.IDUser).ToList()))
                .ReverseMap();
        }
    }
}
