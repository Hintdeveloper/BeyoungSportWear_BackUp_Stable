using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.OrderHistory;
using DataAccessLayer.Application;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Implements
{
    public class OrderHistoryService : IOrderHistoryService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        public OrderHistoryService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbContext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<List<OrderHistoryVM>> GetByIDOrderAsync(Guid IDOrder)
        {
            var orderHistories = await _dbContext.OrderHistory
                                                 .Where(oh => oh.IDOrder == IDOrder).OrderByDescending(oh => oh.CreateDate)

                                                 .ToListAsync();
            return _mapper.Map<List<OrderHistoryVM>>(orderHistories);
        }
        public Task<bool> CreateAsync(OrderHistoryCreateVM request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderHistoryVM>> GetAllActiveAsync()
        {
            var orderHistories = await _dbContext.OrderHistory
                            .Where(o => o.Status == 1)
                            .OrderByDescending(oh => oh.CreateDate)
                            .ToListAsync();
            return _mapper.Map<List<OrderHistoryVM>>(orderHistories);
        }

        public async Task<List<OrderHistoryVM>> GetAllAsync()
        {
            var orderHistories = await _dbContext.OrderHistory.OrderByDescending(oh => oh.CreateDate)

                            .ToListAsync();
            return _mapper.Map<List<OrderHistoryVM>>(orderHistories);
        }

        public async Task<OrderHistoryVM> GetByIDAsync(Guid ID)
        {
            try
            {
                var orderDetail = await _dbContext.OrderHistory.FindAsync(ID);

                if (orderDetail == null)
                {
                    return null;
                }

                var orderDetailVM = _mapper.Map<OrderHistoryVM>(orderDetail);

                return orderDetailVM;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RemoveAsync(Guid ID, string IDUserdelete)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbContext.OrderHistory.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;

                        _dbContext.OrderHistory.Attach(obj);
                        await _dbContext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public Task<bool> UpdateAsync(Guid ID, OrderHistoryUpdateVM request)
        {
            throw new NotImplementedException();
        }
    }
}
