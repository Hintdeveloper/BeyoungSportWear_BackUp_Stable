using AutoMapper;
using BusinessLogicLayer.Viewmodels.Category;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.AutoMapperConfiguration
{
    public class CategoryMap : Profile
    {
        public CategoryMap()
        {
            CreateMap<Category, CategoryVM>();
        }
    }
}
