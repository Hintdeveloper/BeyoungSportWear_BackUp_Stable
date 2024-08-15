using BusinessLogicLayer.Viewmodels.OrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IOrderDetailsService
    {
        public Task<List<OrderDetailsVM>> GetAllAsync();
        public Task<List<OrderDetailsVM>> GetAllActiveAsync();
        public Task<OrderDetailsVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(OrderDetailsCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OrderDetailsUpdateVM request);
    }
}
