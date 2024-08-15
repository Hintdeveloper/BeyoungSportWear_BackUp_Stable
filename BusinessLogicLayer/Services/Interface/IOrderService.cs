using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IOrderService
    {
        public Task<List<OrderVM>> GetAllAsync();
        public Task<List<OrderVM>> GetAllActiveAsync();
        public Task<OrderVM> GetByIDAsync(Guid ID);
        public Task<OrderVM> GetByHexCodeAsync(string HexCode);
        public Task<List<OrderDetailsVM>> GetOrderDetailsVMByIDAsync(Guid IDOrder);
        public Task<bool> CreateAsync(OrderCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OrderUpdateVM request, string IDUserUpdate);
        public Task<List<OrderVM>> GetByCustomerIDAsync(string IDUser);
        public Task<bool> MarkAsCancelledAsync(Guid IDOrder, string IDUserUpdate);
        public Task<bool> MarkAsTrackingCheckAsync(Guid IDOrder, string IDUserUpdate);
        public Task<bool> UpdateOrderStatusAsync(Guid IDOrder, string status, string IDUserUpdate);
        public Task<List<OrderVM>> GetByStatusAsync(OrderStatus OrderStatus);
        public Task<List<OrderVM>> GetByOrderTypeAsync(OrderType OrderType);
    }
}
