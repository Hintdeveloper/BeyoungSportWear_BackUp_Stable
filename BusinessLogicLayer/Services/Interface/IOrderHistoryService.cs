using BusinessLogicLayer.Viewmodels.OrderHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IOrderHistoryService
    {
        public Task<List<OrderHistoryVM>> GetByIDOrderAsync(Guid IDOrder);
        public Task<List<OrderHistoryVM>> GetAllAsync();
        public Task<List<OrderHistoryVM>> GetAllActiveAsync();
        public Task<OrderHistoryVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(OrderHistoryCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OrderHistoryUpdateVM request);
    }
}
