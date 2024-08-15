using AutoMapper;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using BusinessLogicLayer.Viewmodels.Order;
using DataAccessLayer.Entity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderVM>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<ApplicationUser, UserDataVM>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstAndLastName, opt => opt.MapFrom(src => src.FirstAndLastName))
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.JoinDate))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}
