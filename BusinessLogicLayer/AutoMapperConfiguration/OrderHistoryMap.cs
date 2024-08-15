using AutoMapper;
using BusinessLogicLayer.Viewmodels.OrderHistory;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class OrderHistoryMap : Profile
    {
        public OrderHistoryMap()
        {
            CreateMap<OrderHistory, OrderHistoryVM>().ReverseMap();
        }
    }
}
