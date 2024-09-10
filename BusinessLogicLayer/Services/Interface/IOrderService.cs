using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IOrderService
    {
        public Task<List<OrderVM>> GetAllAsync();
        public Task<List<OrderVM>> GetAllActiveAsync();
        public Task<OrderVM> GetByIDAsync(Guid ID);
        public Task<OrderVM> GetByHexCodeAsync(string HexCode);
        public Task<Result> CreateAsync(OrderCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        public Task<bool> UpdateAsync(Guid ID, OrderUpdateVM request, string IDUserUpdate);
        public Task<List<OrderVM>> GetByCustomerIDAsync(string IDUser);
        public Task<Result> UpdateOrderStatusAsync(Guid IDOrder, int status, string IDUserUpdate, string BillOfLadingCode);
        public Task<List<OrderVM>> GetByStatusAsync(OrderStatus OrderStatus);
        public Task<List<OrderVM>> GetByOrderTypeAsync(OrderType OrderType);
    }
}
