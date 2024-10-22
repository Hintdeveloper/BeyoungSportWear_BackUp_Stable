﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static DataAccessLayer.Entity.Base.EnumBase;
using static DataAccessLayer.Entity.Voucher;
using BusinessLogicLayer.Viewmodels.OrderHistory;

namespace BusinessLogicLayer.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailService;
        public OrderService(ApplicationDBContext ApplicationDBContext, IMapper mapper, IEmailSenderService emailService)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
            _emailService = emailService;
        }
        public async Task<bool> CreateAsync(OrderCreateVM request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            try
            {
                string defaultUserID = "";
                var order = _mapper.Map<Order>(request);
                order.ID = Guid.NewGuid();
                order.CreateBy = request.CreateBy;
                order.CreateDate = DateTime.Now;
                order.HexCode = request.HexCode;
                order.IDUser = request.IDUser ?? defaultUserID;
                order.CustomerName = request.CustomerName;
                order.CustomerPhone = request.CustomerPhone;
                order.CustomerEmail = request.CustomerEmail;
                order.OrderStatus = request.OrderStatus;
                order.ShipDate = request.ShipDate;
                order.ShippingAddress = request.ShippingAddress;
                order.ShippingAddressLine2 = request.ShippingAddressLine2;
                order.TrackingCheck = request.TrackingCheck;
                order.VoucherCode = request.VoucherCode;
                order.PaymentMethods = request.PaymentMethods;
                order.PaymentStatus = request.PaymentStatus;
                order.ShippingMethods = request.ShippingMethods;
                order.Cotsts = request.Cotsts ?? 0;
                order.OrderType = request.OrderType;
                order.Status = 1;
                var orderDetailsList = new List<OrderDetails>();
                decimal totalAmount = 0;
                foreach (var directItem in request.OrderDetailsCreateVM)
                {
                    var option = await _dbcontext.Options.FirstOrDefaultAsync(c => c.ID == directItem.IDOptions);
                    if (option != null)
                    {
                        var ordervariant = new OrderDetails()
                        {
                            ID = Guid.NewGuid(),
                            IDOrder = order.ID,
                            IDOptions = option.ID,
                            UnitPrice = option.RetailPrice,
                            Quantity = directItem.Quantity,
                            TotalAmount = (option.RetailPrice * directItem.Quantity) - ((option.Discount ?? 0) * directItem.Quantity),
                            Status = 1,
                            CreateBy = option.CreateBy,
                            CreateDate = DateTime.Now
                        };
                        orderDetailsList.Add(ordervariant);
                        totalAmount += (ordervariant.UnitPrice * ordervariant.Quantity) - ((option.Discount ?? 0) * ordervariant.Quantity);
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    bool stockUpdated = await CheckAndReduceStock(directItem.IDOptions, directItem.Quantity);
                    if (!stockUpdated)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.VoucherCode))
                {
                    var voucher = await _dbcontext.Voucher.FirstOrDefaultAsync(v => v.Code == request.VoucherCode);
                    if (voucher == null || voucher.IsActive == StatusVoucher.IsBeginning || voucher.Quantity <= 0)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    var discountAmount = CalculateDiscountAmount(voucher, totalAmount);
                    totalAmount -= Math.Min(discountAmount, totalAmount - 5000);

                    voucher.Quantity -= 1;
                    if (voucher.Quantity <= 0)
                    {
                        voucher.IsActive = StatusVoucher.Finished;
                    }
                    _dbcontext.Voucher.Update(voucher);
                }
                totalAmount += request.Cotsts ?? 0;

                decimal minimumTotal = 5000;

                if (totalAmount < minimumTotal)
                {
                    order.TotalAmount = minimumTotal;
                }
                else
                {
                    order.TotalAmount = totalAmount;
                }
                order.TotalAmount = totalAmount;

                await _dbcontext.Order.AddAsync(order);
                _dbcontext.OrderDetails.AddRange(orderDetailsList);
                await _dbcontext.SaveChangesAsync();

                var orderHistory = new OrderHistory
                {
                    ID = Guid.NewGuid(),
                    IDOrder = order.ID,
                    IDUser = request.IDUser ?? defaultUserID,
                    EditingHistory = "Đơn hàng được tạo vào ngày "
                                    + DateTime.Now.ToString("dd-MM-yyyy 'lúc' HH:mm") + " với tổng giá trị: "
                                    + @Currency.FormatCurrency(order.TotalAmount.ToString()) + "đ" + "("
                                    + @Currency.NumberToText((double)order.TotalAmount, true) + ")",
                    ChangeDate = DateTime.Now,
                    ChangeType = "",
                    ChangeDetails = "",
                    Status = 1,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,
                };
                await _dbcontext.OrderHistory.AddAsync(orderHistory);
                await _dbcontext.SaveChangesAsync();

                await transaction.CommitAsync();
                string subject = "Đơn hàng của bạn đã được tạo thành công";
                string body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                                color: #333;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #ddd;
                                border-radius: 10px;
                                background-color: #f9f9f9;
                            }}
                            .header {{
                                text-align: center;
                                padding-bottom: 20px;
                            }}
                            .header img {{
                                max-width: 100px;
                            }}
                            .content {{
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 10px;
                            }}
                            .order-info {{
                                margin-top: 20px;
                                padding: 10px;
                                background-color: #eee;
                                border-radius: 5px;
                            }}
                            .product-item {{
                                margin-top: 10px;
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                                display: flex;
                                align-items: center;
                            }}
                            .product-item img {{
                                max-width: 130px;
                                margin-right: 10px;
                            }}
                            .product-details {{
                                flex: 1;
                            }}
                            .footer {{
                                text-align: center;
                                font-size: 12px;
                                color: #999;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <img src='https://example.com/logo.png' alt='Company Logo' />
                                <h2>Cảm ơn bạn đã mua sắm tại chúng tôi!</h2>
                            </div>
                            <div class='content'>
                                <p>Chào {request.CustomerName},</p>
                                <p>Đơn hàng của bạn đã được tạo thành công với mã đơn hàng:</p>
                                <div class='order-info'>
                                    <p><strong>Mã đơn hàng:</strong> {order.HexCode}</p>
                                    <p><strong>Địa chỉ giao hàng:</strong> {order.ShippingAddress}</p>
                                    <p><strong>Tổng số tiền:</strong> {order.TotalAmount.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}</p>
                                </div>
                                <h3>Chi tiết sản phẩm:</h3>";


                foreach (var item in orderDetailsList)
                {
                    var option = await _dbcontext.Options.FindAsync(item.IDOptions);
                    var product = await _dbcontext.ProductDetails
                                    .Include(p => p.Products)
                                    .FirstOrDefaultAsync(p => p.ID == option.IDProductDetails);
                    var size = await _dbcontext.Sizes.FindAsync(option.IDSize);
                    var color = await _dbcontext.Colors.FindAsync(option.IDColor);

                    body += $@"
                                <div class='product-item'>
                                    <img src='{option.ImageURL}' alt='{product.Products.Name}' />
                                    <div class='product-details'>
                                        <p><strong>Tên sản phẩm:</strong> {product.Products.Name}</p>
                                        <p><strong>Size:</strong> {size.Name}</p>
                                        <p><strong>Màu sắc:</strong> {color.Name}</p>
                                        <p><strong>Giá:</strong> {item.UnitPrice.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}</p>
                                        <p><strong>Số lượng:</strong> {item.Quantity}</p>
                                    </div>
                                </div>";
                }

                body += @"
                                <p>Chúng tôi sẽ thông báo cho bạn khi đơn hàng của bạn được vận chuyển.</p>
                                <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                            </div>
                            <div class='footer'>
                                <p>Số điện thoại: 0334539098 | Email hỗ trợ: nhamtuandat317@gmail.com</p>
                                <p>&copy; 2024. Tất cả các quyền được bảo lưu.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                // Gửi email
                await _emailService.SendEmailAsync(request.CustomerEmail, subject, body);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                return false;
            }
        }
        private decimal CalculateDiscountAmount(Voucher voucher, decimal totalAmount)
        {
            if (voucher.Type == Types.Percent)
            {
                return (voucher.ReducedValue / 100m) * totalAmount;
            }
            if (voucher.Type == Types.Cash)
            {
                return voucher.ReducedValue;
            }

            return 0;
        }
        private async Task<bool> CheckAndReduceStock(Guid IDOptions, int quantity)
        {
            if (IDOptions != Guid.Empty && IDOptions != null)
            {
                var variantItem = await _dbcontext.Options.FindAsync(IDOptions);
                if (variantItem != null)
                {
                    if (variantItem.StockQuantity < quantity)
                    {
                        return false;
                    }
                    variantItem.StockQuantity -= quantity;
                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<List<OrderVM>> GetAllActiveAsync()
        {
            var objList = await _dbcontext.Order
               .AsNoTracking()
               .Where(b => b.Status != 0)
               .OrderBy(b => b.OrderStatus == OrderStatus.Delivered || b.OrderStatus == OrderStatus.Cancelled)
               .ThenByDescending(b => b.CreateDate)
               .ProjectTo<OrderVM>(_mapper.ConfigurationProvider)
               .ToListAsync();

            return objList ?? new List<OrderVM>();
        }
        public async Task<List<OrderVM>> GetAllAsync()
        {
            var activeOrders = await _dbcontext.Order
                                      .ToListAsync();

            var activeOrderVMs = _mapper.Map<List<OrderVM>>(activeOrders);

            return activeOrderVMs;
        }
        public async Task<OrderVM> GetByHexCodeAsync(string HexCode)
        {
            var order = await _dbcontext.Order.
                FirstOrDefaultAsync(o => o.HexCode == HexCode);

            var activeOrderVMs = _mapper.Map<OrderVM>(order);

            return activeOrderVMs;
        }
        public async Task<OrderVM> GetByIDAsync(Guid ID)
        {
            var order = await _dbcontext.Order
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Options.ProductDetails.Products)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Options.Sizes)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Options.Colors)
                .Include(o => o.OrderHistory)
                .FirstOrDefaultAsync(o => o.ID == ID && o.Status != 0);

            if (order == null)
            {
                return null;
            }

            var orderVM = _mapper.Map<OrderVM>(order);

            orderVM.OrderDetailsVM = order.OrderDetails.Select(od => new OrderDetailsVM
            {
                ID = od.ID,
                IDOrder = od.IDOrder,
                IDOptions = od.IDOptions,
                ProductName = od.Options.ProductDetails.Products.Name,
                SizeName = od.Options.Sizes?.Name,
                ColorName = od.Options.Colors?.Name,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Discount = od.Discount,
                TotalAmount = od.TotalAmount,
                Status = od.Status
            }).ToList();

            orderVM.OrderHistoryVM = order.OrderHistory
                   .OrderByDescending(h => h.CreateDate)
                   .Select(h => _mapper.Map<OrderHistoryVM>(h))
                   .ToList();
            return orderVM;
        }
        public async Task<List<OrderDetailsVM>> GetOrderDetailsVMByIDAsync(Guid IDOrder)
        {
            var orderDetails = await (
                            from p in _dbcontext.Order
                            join dp in _dbcontext.OrderDetails on p.ID equals dp.IDOrder
                            where dp.IDOrder == IDOrder
                            select new OrderDetailsVM
                            {
                                ID = dp.ID,
                                IDOrder = p.ID,
                                IDOptions = dp.IDOptions,
                                ColorName = dp.Options.Colors.Name,
                                SizeName = dp.Options.Sizes.Name,
                                ProductName = dp.Options.ProductDetails.Products.Name,
                                UnitPrice = dp.UnitPrice,
                                Discount = dp.Discount,
                                Quantity = dp.Quantity,
                                Status = dp.Status,
                                TotalAmount = dp.TotalAmount,
                            }
                        ).ToListAsync();
            if (!orderDetails.Any())
            {
                return new List<OrderDetailsVM>();
            }

            return orderDetails;
        }
        public async Task<bool> RemoveAsync(Guid ID, string IDUserdelete)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Order.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;

                        _dbcontext.Order.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


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
        public async Task<bool> UpdateAsync(Guid ID, OrderUpdateVM request, string IDUserUpdate)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var order = await _dbcontext.Order
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Options)
                    .FirstOrDefaultAsync(o => o.ID == ID);
                var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == IDUserUpdate);
                var username = user != null ? user.UserName : "Unknown User";

                if (order == null)
                {
                    Console.WriteLine($"Không tìm thấy đơn hàng với ID {ID}");
                    await transaction.RollbackAsync();
                    return false;
                }

                if (order.OrderStatus == OrderStatus.Shipped ||
                    order.OrderStatus == OrderStatus.Delivered ||
                    order.OrderStatus == OrderStatus.Cancelled)
                {
                    Console.WriteLine($"Đơn hàng không thể cập nhật vì trạng thái hiện tại là {order.OrderStatus}");
                    await transaction.RollbackAsync();
                    return false;
                }

                var oldStatus = order.OrderStatus;
                var changeDetails = new StringBuilder();
                var editingHistory = new StringBuilder();

                if (IDUserUpdate != null)
                {
                    string userLink = $"<a data-user-id='{IDUserUpdate}'>Người sửa: {username}</a>";
                    changeDetails.AppendLine($"{userLink} vào ngày {DateTime.Now:dd/MM/yyyy HH:mm}");
                    editingHistory.AppendLine("");
                }

                if (order.CustomerName != request.CustomerName)
                {
                    changeDetails.AppendLine($"Tên: {order.CustomerName} => {request.CustomerName}");
                    editingHistory.AppendLine("Thay đổi tên khách hàng");
                    order.CustomerName = request.CustomerName;
                }

                if (order.CustomerPhone != request.CustomerPhone)
                {
                    changeDetails.AppendLine($"Số điện thoại: {order.CustomerPhone} => {request.CustomerPhone}");
                    editingHistory.AppendLine("Thay đổi số điện thoại khách hàng");
                    order.CustomerPhone = request.CustomerPhone;
                }

                if (order.CustomerEmail != request.CustomerEmail)
                {
                    changeDetails.AppendLine($"Gmail: {order.CustomerEmail} => {request.CustomerEmail}");
                    editingHistory.AppendLine("Thay đổi email khách hàng");
                    order.CustomerEmail = request.CustomerEmail;
                }

                if (order.ShippingAddressLine2 != request.ShippingAddressLine2)
                {
                    changeDetails.AppendLine($"Địa chỉ giao hàng cụ thể: {order.ShippingAddressLine2} => {request.ShippingAddressLine2}");
                    editingHistory.AppendLine("Thay đổi địa chỉ giao hàng");
                    order.ShippingAddressLine2 = request.ShippingAddressLine2;
                }

                if (order.Notes != request.Notes)
                {
                    changeDetails.AppendLine($"Ghi chú: {order.Notes} => {request.Notes}");
                    editingHistory.AppendLine("Thay đổi ghi chú");
                    order.Notes = request.Notes;
                }

                foreach (var update in request.OrderDetails)
                {
                    var orderDetail = await _dbcontext.OrderDetails
                        .Include(od => od.Options)
                        .FirstOrDefaultAsync(od => od.IDOrder == ID && od.IDOptions == update.IDOptions);

                    if (orderDetail == null)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    var option = await _dbcontext.Options.FindAsync(update.IDOptions);
                    if (option == null)
                    {
                        Console.WriteLine($"Không tìm thấy tùy chọn với ID {update.IDOptions}");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    if (orderDetail.UnitPrice != option.RetailPrice)
                    {
                        changeDetails.AppendLine($"Giá: {orderDetail.UnitPrice} => {option.RetailPrice}");
                        editingHistory.AppendLine("Thay đổi giá sản phẩm");
                        orderDetail.UnitPrice = option.RetailPrice;
                    }

                    if (orderDetail.Discount != option.Discount)
                    {
                        changeDetails.AppendLine($"Chiết khấu: {orderDetail.Discount} => {option.Discount}");
                        editingHistory.AppendLine("Thay đổi chiết khấu sản phẩm");
                        orderDetail.Discount = option.Discount;
                    }

                    if (orderDetail.Quantity != update.Quantity)
                    {
                        var quantityChange = update.Quantity - orderDetail.Quantity;

                        if (quantityChange > 0)
                        {
                            if (option.StockQuantity < quantityChange)
                            {
                                Console.WriteLine($"Không đủ tồn kho cho tùy chọn với ID {update.IDOptions}");
                                await transaction.RollbackAsync();
                                return false;
                            }

                            option.StockQuantity -= quantityChange;
                            editingHistory.AppendLine($"Giảm tồn kho: {quantityChange}");
                        }
                        else if (quantityChange < 0)
                        {
                            option.StockQuantity += Math.Abs(quantityChange);
                            editingHistory.AppendLine($"Tăng tồn kho: {Math.Abs(quantityChange)}");
                        }

                        changeDetails.AppendLine($"Số lượng: {orderDetail.Quantity} => {update.Quantity}");
                        editingHistory.AppendLine("Thay đổi số lượng sản phẩm");
                        orderDetail.Quantity = update.Quantity;
                    }

                    var discount = orderDetail.Discount ?? 0;
                    orderDetail.TotalAmount = (orderDetail.UnitPrice * orderDetail.Quantity) - (discount * orderDetail.Quantity);

                    orderDetail.ModifiedBy = IDUserUpdate;
                    orderDetail.ModifiedDate = DateTime.Now;

                    _dbcontext.OrderDetails.Update(orderDetail);
                }

                _dbcontext.Options.UpdateRange(order.OrderDetails.Select(od => od.Options));

                order.TotalAmount = order.OrderDetails.Sum(od => od.TotalAmount) + order.Cotsts;

                order.ModifiedBy = IDUserUpdate;
                order.ModifiedDate = DateTime.Now;
                _dbcontext.Order.Update(order);

                if (!string.IsNullOrEmpty(editingHistory.ToString()))
                {
                    var orderHistory = new OrderHistory
                    {
                        ID = Guid.NewGuid(),
                        IDUser = request.IDUser,
                        IDOrder = ID,
                        ChangeDate = DateTime.Now,
                        EditingHistory = editingHistory.ToString(),
                        ChangeType = "OrderUpdate",
                        ChangeDetails = changeDetails.ToString(),
                        CreateBy = request.IDUser,
                        CreateDate = DateTime.Now
                    };

                    _dbcontext.OrderHistory.Add(orderHistory);
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                string subject = "Cập nhật đơn hàng";
                string body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #f9f9f9;
                        border: 1px solid #ddd;
                    }}
                    .header {{
                        background-color: #007bff;
                        color: #fff;
                        padding: 10px;
                        text-align: center;
                        border-radius: 4px 4px 0 0;
                    }}
                    .footer {{
                        font-size: 0.8em;
                        color: #777;
                        text-align: center;
                        margin-top: 20px;
                    }}
                    .details {{
                        margin: 20px 0;
                        padding: 10px;
                        border: 1px solid #ddd;
                        background-color: #fff;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        Cập nhật đơn hàng
                    </div>
                    <p>Xin chào {order.CustomerName},</p>
                    <p>Đơn hàng của bạn với mã đơn hàng <strong>{order.HexCode}</strong> đã được cập nhật.</p>
                    <div class='details'>
                        <p><strong>Chi tiết thay đổi:</strong></p>
                        {changeDetails}
                    </div>
                    <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                    <div class='footer'>
                        Trân trọng,<br>
                        Đội ngũ hỗ trợ
                    </div>
                </div>
            </body>
            </html>";
                await _emailService.SendEmailAsync(request.CustomerEmail, subject, body);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi cập nhật đơn hàng: {ex.Message}");
                return false;
            }
        }
        public async Task<List<OrderVM>> GetByCustomerIDAsync(string IDUser)
        {
            var orders = await _dbcontext.Order
                                         .Where(o => o.IDUser == IDUser && o.Status != 0)
                                         .ToListAsync();
            var sortedOrders = orders
               .OrderByDescending(b => b.CreateDate)
               .ToList();
            return _mapper.Map<List<OrderVM>>(sortedOrders);
        }
        public async Task<bool> IncreaseStockAsync(Guid IDOptions, int quantity)
        {
            if (IDOptions != Guid.Empty && IDOptions != null)
            {
                var variantItem = await _dbcontext.Options.FindAsync(IDOptions);
                if (variantItem != null)
                {
                    variantItem.StockQuantity += quantity;
                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> MarkAsCancelledAsync(Guid IDOrder, string IDUserUpdate)
        {
            var order = await _dbcontext.Order
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ID == IDOrder);

            if (order == null)
            {
                return false;
            }

            if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Cancelled || order.PaymentStatus == PaymentStatus.Success)
            {
                return false;
            }

            var changeDetails = new StringBuilder();
            var editingHistory = new StringBuilder();
            if (order.ModifiedBy != null)
            {
                string userLink = $"<a data-user-id='{IDUserUpdate}'>Người sửa: {IDUserUpdate}</a>";
                changeDetails.AppendLine($"{userLink} vào ngày {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");
                editingHistory.AppendLine("");
            }
            if (order.OrderStatus != OrderStatus.Cancelled)
            {
                changeDetails.AppendLine($"Trạng thái: {order.OrderStatus} => {OrderStatus.Cancelled}");
                editingHistory.AppendLine("Thay đổi trạng thái đơn hàng");
                order.OrderStatus = OrderStatus.Cancelled;
            }

            foreach (var orderItem in order.OrderDetails)
            {
                await IncreaseStockAsync(orderItem.IDOptions, orderItem.Quantity);
            }

            order.ModifiedBy = IDUserUpdate;
            order.ModifiedDate = DateTime.Now;

            _dbcontext.Order.Update(order);

            if (!string.IsNullOrEmpty(editingHistory.ToString()))
            {
                var orderHistory = new OrderHistory
                {
                    ID = Guid.NewGuid(),
                    IDUser = IDUserUpdate,
                    IDOrder = IDOrder,
                    ChangeDate = DateTime.Now,
                    EditingHistory = editingHistory.ToString(),
                    ChangeType = "OrderCancellation",
                    ChangeDetails = changeDetails.ToString(),
                    CreateBy = IDUserUpdate,
                    CreateDate = DateTime.Now
                };

                _dbcontext.OrderHistory.Add(orderHistory);
            }

            await _dbcontext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MarkAsTrackingCheckAsync(Guid IDOrder, string IDUserUpdate)
        {
            var order = await _dbcontext.Order
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ID == IDOrder);
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == IDUserUpdate);
            var username = user != null ? user.UserName : "Unknown User";

            if (order == null)
            {
                return false;
            }

            var changeDetails = new StringBuilder();
            var editingHistory = new StringBuilder();

            if (order.TrackingCheck != true || order.ModifiedBy != null)
            {
                if (order.TrackingCheck != true)
                {
                    string userLink = $"<a data-user-id='{IDUserUpdate}'>Người sửa: {username}</a>";
                    editingHistory.AppendLine("Thay đổi trạng thái đơn hàng");
                    changeDetails.AppendLine($"Trạng thái: Chưa xác nhận => Xác nhận đơn hàng <br>" + userLink);
                    order.TrackingCheck = true;
                }
            }

            order.ModifiedBy = IDUserUpdate;
            order.ModifiedDate = DateTime.Now;

            _dbcontext.Order.Update(order);

            if (!string.IsNullOrEmpty(editingHistory.ToString()))
            {
                var orderHistory = new OrderHistory
                {
                    ID = Guid.NewGuid(),
                    IDUser = IDUserUpdate,
                    IDOrder = IDOrder,
                    ChangeDate = DateTime.Now,
                    EditingHistory = editingHistory.ToString(),
                    ChangeType = "OrderCancellation",
                    ChangeDetails = changeDetails.ToString(),
                    CreateBy = IDUserUpdate,
                    CreateDate = DateTime.Now
                };

                _dbcontext.OrderHistory.Add(orderHistory);
            }

            await _dbcontext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateOrderStatusAsync(Guid IDOrder, string status, string IDUserUpdate)
        {
            var order = await _dbcontext.Order
                           .Include(o => o.OrderDetails)
                           .FirstOrDefaultAsync(o => o.ID == IDOrder);

            if (order == null)
            {
                return false;
            }
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == IDUserUpdate);
            var username = user != null ? user.UserName : "Unknown User";
            var newStatus = Enum.TryParse<OrderStatus>(status, true, out var parsedStatus) ? parsedStatus : order.OrderStatus;
            if (order.OrderStatus == newStatus)
            {
                return true;
            }
            if (!IsValidStatusTransition(order.OrderStatus, newStatus))
            {
                return false;
            }
            var oldStatusDescription = order.OrderStatus.GetDescription();
            var newStatusDescription = newStatus.GetDescription();

            var changeDetails = $"Trạng thái: Từ {oldStatusDescription} sang {newStatusDescription}";
            var editingHistory = "Thay đổi trạng thái đơn hàng";

            if (order.ModifiedBy != null)
            {
                changeDetails = $"Trạng thái: Từ {oldStatusDescription} sang {newStatusDescription},<br> Người sửa: <a data-user-id='{IDUserUpdate}'>{username}</a>";
                editingHistory = "Người cập nhật";
            }

            order.OrderStatus = newStatus;
            order.ModifiedBy = IDUserUpdate;
            order.ModifiedDate = DateTime.Now;

            if (newStatus == OrderStatus.Delivered)
            {
                order.PaymentStatus = PaymentStatus.Success;
            }

            var orderHistory = new OrderHistory
            {
                ID = Guid.NewGuid(),
                IDUser = IDUserUpdate,
                IDOrder = IDOrder,
                ChangeDate = DateTime.Now,
                EditingHistory = editingHistory,
                ChangeType = "OrderStatusUpdate",
                ChangeDetails = changeDetails,
                CreateBy = IDUserUpdate,
                CreateDate = DateTime.Now
            };

            _dbcontext.OrderHistory.Add(orderHistory);
            await _dbcontext.SaveChangesAsync();
            string subject = "Cập nhật trạng thái đơn hàng";
            string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background-color: #f9f9f9;
                            border: 1px solid #ddd;
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #fff;
                            padding: 10px;
                            text-align: center;
                            border-radius: 4px 4px 0 0;
                        }}
                        .footer {{
                            font-size: 0.8em;
                            color: #777;
                            text-align: center;
                            margin-top: 20px;
                        }}
                        .details {{
                            margin: 20px 0;
                            padding: 10px;
                            border: 1px solid #ddd;
                            background-color: #fff;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            Cập nhật trạng thái đơn hàng
                        </div>
                        <p>Xin chào {order.CustomerName},</p>
                        <p>Trạng thái đơn hàng của bạn với mã đơn hàng <strong>{order.HexCode}</strong> đã được cập nhật từ <strong>{oldStatusDescription}</strong> sang <strong>{newStatusDescription}</strong>.</p>
                        <div class='details'>
                            <p><strong>Người cập nhật:</strong> {username}</p>
                        </div>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                        <div class='footer'>
                            Trân trọng,<br>
                            Đội ngũ hỗ trợ
                        </div>
                    </div>
                </body>
                </html>";
            await _emailService.SendEmailAsync(order.CustomerEmail, subject, body);

            return true;
        }
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
                {
                    { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
                    { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
                    { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
                    { OrderStatus.Delivered, new List<OrderStatus>() }, // Không có chuyển tiếp từ Delivered
                    { OrderStatus.Cancelled, new List<OrderStatus>() }  // Không có chuyển tiếp từ Cancelled
                };

            return validTransitions.ContainsKey(currentStatus) && validTransitions[currentStatus].Contains(newStatus);
        }
        public async Task<List<OrderVM>> GetByStatusAsync(OrderStatus OrderStatus)
        {
            var orders = await _dbcontext.Order
                            .Where(o => o.OrderStatus == OrderStatus && o.TrackingCheck == true)
                            .OrderByDescending(b => b.CreateDate)
                            .Select(o => new OrderVM
                            {
                                ID = o.ID,
                                HexCode = o.HexCode,
                                IDUser = o.IDUser,
                                VoucherCode = o.VoucherCode,
                                CreateDate = o.CreateDate,
                                PaymentMethod = o.PaymentMethods,
                                PaymentStatus = o.PaymentStatus,
                                TotalAmount = o.TotalAmount,
                                OrderStatus = o.OrderStatus,
                                CustomerName = o.CustomerName,
                                CustomerPhone = o.CustomerPhone,
                                CustomerEmail = o.CustomerEmail,
                                ShippingAddress = o.ShippingAddress,
                                ShippingAddressLine2 = o.ShippingAddressLine2,
                                Cotsts = o.Cotsts,
                                TrackingCheck = o.TrackingCheck,
                            })
                            .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new List<OrderVM>();
            }

            return orders;
        }
        public async Task<List<OrderVM>> GetByOrderTypeAsync(OrderType OrderType)
        {
            var orders = await _dbcontext.Order
                            .Where(o => o.OrderType == OrderType)
                            .OrderByDescending(b => b.CreateDate)
                            .Select(o => new OrderVM
                            {
                                ID = o.ID,
                                HexCode = o.HexCode,
                                IDUser = o.IDUser,
                                VoucherCode = o.VoucherCode,
                                CreateDate = o.CreateDate,
                                PaymentMethod = o.PaymentMethods,
                                PaymentStatus = o.PaymentStatus,
                                TotalAmount = o.TotalAmount,
                                OrderStatus = o.OrderStatus,
                                CustomerName = o.CustomerName,
                                CustomerPhone = o.CustomerPhone,
                                CustomerEmail = o.CustomerEmail,
                                ShippingAddress = o.ShippingAddress,
                                ShippingAddressLine2 = o.ShippingAddressLine2,
                                Cotsts = o.Cotsts,
                                OrderType = o.OrderType,
                                TrackingCheck = o.TrackingCheck,
                            })
                            .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new List<OrderVM>();
            }

            return orders;
        }
    }
}
