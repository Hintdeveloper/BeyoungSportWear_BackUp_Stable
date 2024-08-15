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
    private int totalOrders;
    private int totalOrdersnosuccess;
    private int totalOrderssuccess;
    private int TotalQuantityOptions;
    private int TotalProduct;
    private int TotalUser;

    public StatisticsService(ApplicationDBContext dbcontext, IMapper IMapper)
    {
        _mapper = IMapper;
        _dbcontext = dbcontext;
    }
    public async Task<Dictionary<Guid, int>> CalculateBestSellingProducts(DateTime startDate, DateTime endDate)
    {
        var orders = await _dbcontext.Order
                    .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate && order.OrderStatus == OrderStatus.Delivered)
                    .Include(order => order.OrderDetails)
                    .ToListAsync();

        var productSalesCount = new Dictionary<Guid, int>();

        foreach (var order in orders)
        {
            foreach (var orderVariant in order.OrderDetails)
            {
                var productId = orderVariant.IDOptions;
                var quantitySold = orderVariant.Quantity;

                if (!productSalesCount.ContainsKey(productId))
                {
                    productSalesCount[productId] = quantitySold;
                }
                else
                {
                    productSalesCount[productId] += quantitySold;
                }
            }
        }

        var sortedProducts = productSalesCount.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        return sortedProducts;
    }

    public async Task<MonthlyStatistic> CalculateStatistics(string month)
    {
        DateTime selectedMonth = string.IsNullOrEmpty(month) ? DateTime.Now : DateTime.Parse(month);

        var startDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var bankPaymentOrders = await GetBankPaymentOrders(selectedMonth);
        var totalStockQuantity = await _dbcontext.Options
            .SumAsync(option => option.StockQuantity);
        decimal totalBankPayments = bankPaymentOrders.Sum(order => order.TotalAmount);

        var orders = _dbcontext.Order.Where(o => o.CreateDate >= startDate && o.CreateDate <= endDate && o.OrderStatus == OrderStatus.Delivered);

        //decimal totalRevenue = 0;
        //if (await orders.AnyAsync())
        //{
        //    totalRevenue = await orders.SumAsync(o => o.TotalAmount);
        //}
        //totalRevenue = Math.Max(0, totalRevenue);

        int totalOrders = await _dbcontext.Order
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate)
            .CountAsync();

        int totalProduct = await _dbcontext.ProductDetails
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate)
            .CountAsync();

        int totalQuantityOptions = await _dbcontext.Options
            .Where(order => order.CreateDate >= startDate && order.CreateDate <= endDate)
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
            //TotalRevenue = totalRevenue,
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

            decimal monthlyRevenue = 0;
            if (await orders.AnyAsync())
            {
                monthlyRevenue = await orders.SumAsync(o => o.TotalAmount);
            }
            monthlyRevenue = Math.Max(0, monthlyRevenue);

            yearlyStatistics.MonthlyRevenues.Add(monthlyRevenue);
        }

        return yearlyStatistics;
    }

    public async Task<List<Order>> GetBankPaymentOrders(DateTime selectedMonth)
    {
        var bankPaymentOrders = await _dbcontext.Order
                    .Where(order => order.PaymentMethods == PaymentMethod.ChuyenKhoanNganHang
                                 && order.CreateDate.Year == selectedMonth.Year
                                 && order.CreateDate.Month == selectedMonth.Month)
                    .ToListAsync();

        return bankPaymentOrders;
    }
}
