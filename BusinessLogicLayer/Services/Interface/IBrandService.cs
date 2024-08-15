using BusinessLogicLayer.Viewmodels.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IBrandService
    {
        public Task<List<BrandVM>> GetAllAsync();
        public Task<List<BrandVM>> GetAllActiveAsync();
        public Task<BrandVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(BrandCreateVM request);
        public Task<bool> SetStatus(Guid ID);
        public Task<bool> UpdateAsync(Guid ID, BrandUpdateVM request);
    }
}
