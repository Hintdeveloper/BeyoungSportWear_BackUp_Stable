using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.OrderHistory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHistoryController : ControllerBase
    {
        private readonly IOrderHistoryService _IOrderHistoryService;

        public OrderHistoryController(IOrderHistoryService IOrderHistoryService)
        {
            _IOrderHistoryService = IOrderHistoryService;
        }
        [HttpGet("getallactive")]
        public async Task<IActionResult> GetAllActive()
        {
            var orderHistories = await _IOrderHistoryService.GetAllActiveAsync();
            return Ok(orderHistories);
        }
        [HttpGet("by-order/{IDOrder}")]
        public async Task<IActionResult> GetByIDOrderAsync(Guid IDOrder)
        {
            var orderHistories = await _IOrderHistoryService.GetByIDOrderAsync(IDOrder);
            if (orderHistories == null || orderHistories.Count == 0)
                return NotFound();

            return Ok(orderHistories);
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var orderHistories = await _IOrderHistoryService.GetAllAsync();
            return Ok(orderHistories);
        }

        [HttpGet("{ID}")]
        public async Task<IActionResult> GetById(Guid ID)
        {
            var orderHistory = await _IOrderHistoryService.GetByIDAsync(ID);
            if (orderHistory == null)
                return NotFound();

            return Ok(orderHistory);
        }


        [HttpDelete("{ID}")]
        public async Task<IActionResult> Remove(Guid ID, string idUserDelete)
        {
            var result = await _IOrderHistoryService.RemoveAsync(ID, idUserDelete);
            if (result)
                return Ok(new { Message = "Order history deleted successfully." });

            return StatusCode(500, "Internal server error.");
        }
    }
}