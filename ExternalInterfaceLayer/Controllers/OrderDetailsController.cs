using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.OrderDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailsService _IOrderDetailsService;

        public OrderDetailsController(IOrderDetailsService IOrderDetailsService)
        {
            _IOrderDetailsService = IOrderDetailsService;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> AddAsync([FromBody] OrderDetailsCreateVM request)
        {
            if (request == null) return BadRequest();
            var result = await _IOrderDetailsService.CreateAsync(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllAsync()
        {
            var orderDetails = await _IOrderDetailsService.GetAllAsync();
            if (orderDetails == null || orderDetails.Count == 0)
            {
                return NoContent();
            }

            return Ok(orderDetails);
        }

        [HttpGet]
        [Route("allactive")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var objCollection = await _IOrderDetailsService.GetAllActiveAsync();

            return Ok(objCollection);
        }
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByIDAsync(Guid ID)
        {
            var orderDetail = await _IOrderDetailsService.GetByIDAsync(ID);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }
    }
}