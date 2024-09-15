using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Implements
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        public OrderDetailsService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbContext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(OrderDetailsCreateVM request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var order = await _dbContext.Order.FirstOrDefaultAsync(c => c.ID == request.IDOrder);

                if (order == null)
                {
                    return false;
                }

                var orderDetailsList = new List<OrderDetails>();
                decimal totalAmount = 0;

                var options = await _dbContext.Options.FirstOrDefaultAsync(v => v.ID == request.IDOptions);

                if (options != null)
                {
                    var orderDetail = new OrderDetails
                    {
                        ID = Guid.NewGuid(),
                        IDOrder = order.ID,
                        IDOptions = request.IDOptions,
                        Quantity = request.Quantity,
                        UnitPrice = options.RetailPrice,
                        TotalAmount = request.Quantity * options.RetailPrice,
                        Status = 1
                    };

                    orderDetailsList.Add(orderDetail);
                    decimal discountValue = request.Discount ?? 0;
                    totalAmount += (orderDetail.UnitPrice * orderDetail.Quantity) - discountValue;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                bool stockUpdated = await CheckAndReduceStock(request.IDOptions, request.Quantity);
                if (!stockUpdated)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                order.TotalAmount = totalAmount;

                _dbContext.OrderDetails.AddRange(orderDetailsList);

                await _dbContext.SaveChangesAsync();

                await UpdateTotalAmountForOrder(order.ID);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        private async Task<bool> CheckAndReduceStock(Guid IDOptions, int quantity)
        {
            if (IDOptions != Guid.Empty)
            {
                var optionItem = await _dbContext.Options
                                        .FirstOrDefaultAsync(o => o.ID == IDOptions);

                if (optionItem != null && optionItem.StockQuantity >= quantity)
                {
                    optionItem.StockQuantity -= quantity;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        private async Task UpdateTotalAmountForOrder(Guid orderId)
        {
            var orderDetailsList = await _dbContext.OrderDetails
                .Where(od => od.IDOrder == orderId)
                .ToListAsync();

            if (orderDetailsList != null && orderDetailsList.Any())
            {
                decimal totalAmount = orderDetailsList.Sum(od => od.TotalAmount);

                var orderToUpdate = await _dbContext.Order.FirstOrDefaultAsync(o => o.ID == orderId);
                if (orderToUpdate != null)
                {
                    orderToUpdate.TotalAmount = totalAmount;
                    _dbContext.Entry(orderToUpdate).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        public async Task<List<OrderDetailsVM>> GetAllActiveAsync()
        {
            try
            {
                var activeOrderDetails = await _dbContext.OrderDetails
                    .Where(od => od.Status == 1)
                    .ToListAsync();

                var orderDetailVMs = _mapper.Map<List<OrderDetailsVM>>(activeOrderDetails);

                return orderDetailVMs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<OrderDetailsVM>> GetAllAsync()
        {
            try
            {
                var orderDetails = await _dbContext.OrderDetails.ToListAsync();
                var orderDetailVMs = _mapper.Map<List<OrderDetailsVM>>(orderDetails);

                return orderDetailVMs;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OrderDetailsVM> GetByIDAsync(Guid ID)
        {
            try
            {
                var orderDetail = await _dbContext.OrderDetails.FindAsync(ID);

                if (orderDetail == null)
                {
                    return null;
                }

                var orderDetailVM = _mapper.Map<OrderDetailsVM>(orderDetail);

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
                    var obj = await _dbContext.Order.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;

                        _dbContext.Order.Attach(obj);
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

        public Task<bool> UpdateAsync(Guid ID, OrderDetailsUpdateVM request)
        {
            throw new NotImplementedException();
        }
    }
}
