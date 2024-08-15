using BusinessLogicLayer.Viewmodels.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IOptionsService : IActivatable
    {
        public Task<List<OptionsVM>> GetAllAsync();
        public Task<List<OptionsVM>> GetAllActiveAsync();
        public Task<OptionsVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(OptionsCreateSingleVM request);
        //public Task<bool> UpdateIsActiveAsync(Guid IDOptions, bool isActive);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OptionsUpdateVM request);
        public Task<Guid> GetProductDetailsByID(Guid IDOptions);
        public Task<OptionsVM> FindIDOptionsAsync(Guid IDProductDetails, string size, string color);
        public Task<List<OptionsVM>> GetOptionsByProductDetailsIdAsync(Guid IDProductDetails);

    }
}
