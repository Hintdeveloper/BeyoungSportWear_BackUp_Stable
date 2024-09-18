using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMapper _mapper;

        public StatisticsController(IStatisticsService statisticsService, IMapper mapper)
        {
            _statisticsService = statisticsService;
            _mapper = mapper;
        }

        [HttpGet("year")]
        public async Task<IActionResult> CalculateYearlyStatistics([FromQuery] string year)
        {
            try
            {
                var statistics = await _statisticsService.CalculateYearlyStatistics(year);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating statistics.", error = ex.Message });
            }
        }

        [HttpGet("bank-payment-orders")]
        public async Task<IActionResult> GetBankPaymentOrders([FromQuery] string month)
        {
            try
            {
                DateTime selectedMonth = string.IsNullOrEmpty(month) ? DateTime.Now : DateTime.Parse(month);
                var bankPaymentOrders = await _statisticsService.GetBankPaymentOrders(selectedMonth);
                return Ok(bankPaymentOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving bank payment orders.", error = ex.Message });
            }
        }

        [HttpGet("best-selling-products")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate);
                var bestSellingProducts = await _statisticsService.CalculateBestSellingProducts(start, end);
                return Ok(bestSellingProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving best-selling products.", error = ex.Message });
            }
        }

        [HttpGet("calculate_statistics/{startDate}/{endDate}")]
        public async Task<IActionResult> CalculateStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                var bestSellingProducts = await _statisticsService.CalculateStatistics(startDate, endDate);
                return Ok(bestSellingProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving best-selling products.", error = ex.Message });
            }
        }

    }
}
