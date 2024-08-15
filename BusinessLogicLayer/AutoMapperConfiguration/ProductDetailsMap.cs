using AutoMapper;
using BusinessLogicLayer.Viewmodels.ProductDetails;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class ProductDetailsMap : Profile
    {
        public ProductDetailsMap()
        {
            CreateMap<ProductDetails, ProductDetailsVM>()
           .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.Name))
           .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
           .ForMember(dest => dest.ManufacturersName, opt => opt.MapFrom(src => src.Manufacturers.Name))
           .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.Name));

           //.ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.Options.Sum(opt => opt.StockQuantity)))
           //.ForMember(dest => dest.SmallestPrice, opt => opt.MapFrom(src => src.Options.Any() ? src.Options.Min(opt => opt.RetailPrice) : 0))
           //.ForMember(dest => dest.BiggestPrice, opt => opt.MapFrom(src => src.Options.Any() ? src.Options.Max(opt => opt.RetailPrice) : 0))
           //.ForMember(dest => dest.ImagePaths, opt => opt.MapFrom(src => src.Images.Where(i => i.Status == 1).Select(i => i.Path).ToList()));

        }
    }
}
