using BusinessLogicLayer.Viewmodels.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IMaterialService
    {
        public Task<List<MaterialVM>> GetAllAsync();
        public Task<List<MaterialVM>> GetAllActiveAsync();
        public Task<MaterialVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(MaterialCreateVM request);
        public Task<bool> SetStatus(Guid ID);
        public Task<bool> UpdateAsync(Guid ID, MaterialUpdateVM request);
    }
}
