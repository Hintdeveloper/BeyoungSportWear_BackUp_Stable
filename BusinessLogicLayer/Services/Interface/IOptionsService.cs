using BusinessLogicLayer.Viewmodels;
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
        public Task<List<OptionsVM>> GetByNameAsync(string Name);
        public Task<bool> CreateAsync(OptionsCreateSingleVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OptionsUpdateVM request);
        public Task<OptionsVM> FindIDOptionsAsync(Guid IDProductDetails, string size, string color);
        public Task<List<OptionsVM>> FindIDOptionsBySize(Guid IDProductDetails, string size);
        public Task<List<OptionsVM>> FindIDOptionsByColor(Guid IDProductDetails, string color);
        public Task<Result> DecreaseQuantityAsync(Guid IDOptions, int quantityToDecrease);
        public Task<Result> IncreaseQuantityAsync(Guid IDOptions, int quantityToIncrease);
    }
}