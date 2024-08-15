using BusinessLogicLayer.Viewmodels.ApplicationUser;
using BusinessLogicLayer.Viewmodels.Order;

namespace BusinessLogicLayer.Viewmodels.Statistical
{

    public class MonthlyStatistic
    {
        public DateTime Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ProductsAlmostOutOfStock { get; set; }
        public int TotalProduct { get; set; }
        public int TotalQuantityOptions { get; set; }
        public int TotalOrder { get; set; }
        public int TotalUser { get; set; }
        public int TotalOrdersnosuccess { get; set; }
        public int TotalOrderssuccess { get; set; }
        public int TotalStaffUsers { get; set; }
        public int TotalClientUsers { get; set; }
        public Dictionary<Guid, int> BestSellingProducts { get; set; }
    }
}


