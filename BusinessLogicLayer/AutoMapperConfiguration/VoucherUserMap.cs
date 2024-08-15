using AutoMapper;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class VoucherUserMap : Profile
    {
        public VoucherUserMap()
        {
            CreateMap<VoucherUser, VoucherUserVM>().ReverseMap();
        }
    }
}
