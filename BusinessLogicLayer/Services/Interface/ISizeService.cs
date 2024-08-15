using BusinessLogicLayer.Viewmodels.Sizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface ISizeService
    {
        public Task<List<SizeVM>> GetAllAsync();
        public Task<List<SizeVM>> GetAllActiveAsync();
        public Task<SizeVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(SizeCreateVM request);
        public Task<bool> SetStatus(Guid ID);
        public Task<bool> UpdateAsync(Guid ID, SizeUpdateVM request);
    }
}
