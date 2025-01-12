﻿using BusinessLogicLayer.Viewmodels.Statistical;
using DataAccessLayer.Entity;
public interface IStatisticsService
{
    Task<MonthlyStatistic> CalculateStatistics(string month);
    Task<List<Order>> GetBankPaymentOrders(DateTime selectedMonth);
    Task<Dictionary<Guid, int>> CalculateBestSellingProducts(DateTime startDate, DateTime endDate);
    Task<YearlyStatistic> CalculateYearlyStatistics(string year);
}
