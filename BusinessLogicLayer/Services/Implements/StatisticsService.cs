using AutoMapper;
using BusinessLogicLayer.Viewmodels.Statistical;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using static DataAccessLayer.Entity.Base.EnumBase;

public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDBContext _dbcontext;
    private readonly IMapper _mapper;

    public StatisticsService(ApplicationDBContext dbcontext, IMapper mapper)
    {
        _dbcontext = dbcontext;
        _mapper = mapper;
    }

    public async Task<Dictionary<Guid, int>> CalculateBestSellingProducts(DateTime startDate, DateTime endDate)
    {
        return await _dbcontext.Order
            .Where(order => order.CreateDate >= startDate
                         && order.CreateDate <= endDate
                         && order.OrderStatus == OrderStatus.Delivered)
            .SelectMany(order => order.OrderDetails)
            .GroupBy(orderDetail => orderDetail.Options.ProductDetails.ID)
            .Select(group => new
            {
                ProductId = group.Key,
                QuantitySold = group.Sum(orderDetail => orderDetail.Quantity)
            })
            .OrderByDescending(result => result.QuantitySold)
            .ToDictionaryAsync(result => result.ProductId, result => result.QuantitySold);
    }

    public async Task<MonthlyStatistic> CalculateStatistics(DateTime startDate, DateTime endDate)
    {
        var bankPaymentOrders = await GetBankPaymentOrders(new DateTime(startDate.Year, startDate.Month, 1));
        var totalStockQuantity = await _dbcontext.Options.SumAsync(option => option.StockQuantity);
        decimal totalBankPayments = bankPaymentOrders.Sum(order => order.TotalAmount);

        int totalOrders = await _dbcontext.Order
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate)
            .CountAsync();

        int totalProduct = await _dbcontext.ProductDetails
            .Where(product => product.CreateDate >= startDate && product.CreateDate <= endDate)
            .CountAsync();

        int totalQuantityOptions = await _dbcontext.Options
            .Where(option => option.CreateDate >= startDate && option.CreateDate <= endDate)
            .SumAsync(c => c.StockQuantity);

        int totalOrdersnosuccess = await _dbcontext.Order
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate && order.OrderStatus == OrderStatus.Cancelled)
            .CountAsync();

        int totalOrderssuccess = await _dbcontext.Order
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate && order.OrderStatus == OrderStatus.Delivered)
            .CountAsync();

        int totalUser = await _dbcontext.ApplicationUser
            .Where(user => user.JoinDate >= startDate && user.JoinDate <= endDate)
            .CountAsync();

        const int threshold = 10;
        int productsAlmostOutOfStock = await _dbcontext.ProductDetails
            .Select(pd => new
            {
                ProductId = pd.ID,
                TotalQuantity = pd.Options.Sum(o => o.StockQuantity)
            })
            .Where(p => p.TotalQuantity <= threshold)
            .CountAsync();

        var bestSellingProducts = await CalculateBestSellingProducts(startDate, endDate);

        return new MonthlyStatistic
        {
            TotalOrder = totalOrders,
            Month = startDate,
            TotalQuantityOptions = totalQuantityOptions,
            TotalProduct = totalProduct,
            BestSellingProducts = bestSellingProducts,
            TotalUser = totalUser,
            ProductsAlmostOutOfStock = productsAlmostOutOfStock,
            TotalOrdersnosuccess = totalOrdersnosuccess,
            TotalOrderssuccess = totalOrderssuccess
        };
    }

    public async Task<YearlyStatistic> CalculateYearlyStatistics(string year)
    {
        DateTime selectedYear = string.IsNullOrEmpty(year) ? DateTime.Now : DateTime.Parse(year);
        int selectedYearNumber = selectedYear.Year;

        var yearlyStatistics = new YearlyStatistic
        {
            Year = selectedYearNumber,
            MonthlyRevenues = new List<decimal>()
        };

        for (int month = 1; month <= 12; month++)
        {
            var startDate = new DateTime(selectedYearNumber, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var orders = _dbcontext.Order
                .Where(o => o.CreateDate >= startDate && o.CreateDate <= endDate && o.OrderStatus == OrderStatus.Delivered);

            decimal monthlyRevenue = await orders.AnyAsync()
                ? await orders.SumAsync(o => o.TotalAmount)
                : 0;

            yearlyStatistics.MonthlyRevenues.Add(monthlyRevenue);
        }

        return yearlyStatistics;
    }

    public async Task<List<Order>> GetBankPaymentOrders(DateTime selectedMonth)
    {
        return await _dbcontext.Order
            .Where(order => order.PaymentMethods == PaymentMethod.ChuyenKhoanNganHang
                         && order.CreateDate.Year == selectedMonth.Year
                         && order.CreateDate.Month == selectedMonth.Month)
            .ToListAsync();
    }
}
