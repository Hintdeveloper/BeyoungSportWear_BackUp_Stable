using BusinessLogicLayer.Viewmodels.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IColorService
    {
        public Task<List<ColorVM>> GetAllAsync();
        public Task<List<ColorVM>> GetAllActiveAsync();
        public Task<ColorVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(ColorCreateVM request);
        public Task<bool> SetStatus(Guid ID);
        public Task<bool> UpdateAsync(Guid ID, ColorUpdateVM request);
    }
}
