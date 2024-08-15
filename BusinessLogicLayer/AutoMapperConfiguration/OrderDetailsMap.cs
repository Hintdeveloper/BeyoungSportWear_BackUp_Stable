using AutoMapper;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class OrderDetailsMap : Profile
    {
        public OrderDetailsMap()
        {
            CreateMap<OrderDetails, OrderDetailsVM>()
                .ReverseMap();

        }
    }
}
