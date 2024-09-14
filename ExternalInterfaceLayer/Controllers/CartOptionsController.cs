using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.CartOptions;
using DataAccessLayer.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartOptionsController : ControllerBase
    {
        private readonly ICartOptionsService _ICartOptionsService;
        private readonly ApplicationDBContext _dbcontext;

        public CartOptionsController(ICartOptionsService ICartProductDetailsService, ApplicationDBContext ApplicationDBContext)
        {
            _ICartOptionsService = ICartProductDetailsService;
            _dbcontext = ApplicationDBContext;
        }

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddAsync([FromBody] CartOptionsCreateVM request)
        {
            if (request == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }
            var result = await _ICartOptionsService.CreateAsync(request);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                var option = await _dbcontext.Options.FirstOrDefaultAsync(o => o.ID == request.IDOptions);
                if (option != null && request.Quantity > option.StockQuantity)
                {
                    return BadRequest("Số lượng yêu cầu vượt quá số lượng có sẵn trong kho.");
                }
                return NotFound("Số lượng không hợp lệ");
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var objCollection = await _ICartOptionsService.GetAllAsync();

            return Ok(objCollection);
        }

        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var objCollection = await _ICartOptionsService.GetAllActiveAsync();

            return Ok(objCollection);
        }

        [HttpGet]
        [Route("{IDCart}/{IDOptions}")]
        public async Task<IActionResult> GetByIdAsync(string IDCart, Guid? IDOptions)
        {
            var objVM = await _ICartOptionsService.GetByIDAsync(IDCart, IDOptions);

            return Ok(objVM);
        }

        [HttpDelete]
        [Route("Delete/{IDCart}/{IDOptions}")]
        public async Task<IActionResult> RemoveAsync(string IDCart, Guid? IDOptions)
        {
            //var objDelete = await _ICartOptionsService.GetByIDAsync(IDCart, IDOptions);
            //if (objDelete == null) return NotFound();

            var result = await _ICartOptionsService.RemoveAsync(IDCart, IDOptions);

            return Ok(result);
        }

        [HttpPut]
        [Route("{IDCart}/{IDOptions}")]
        public async Task<IActionResult> UpdateAsync(string IDCart, Guid? IDOptions, [FromBody] CartOptionsUpdateVM request)
        {

            if (request == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var result = await _ICartOptionsService.UpdateAsync(IDCart, IDOptions, request);

            if (result)
            {
                return Ok(request);
            }
            else
            {
                var option = await _dbcontext.Options.FirstOrDefaultAsync(o => o.ID == IDOptions);
                if (option != null && request.Quantity > option.StockQuantity)
                {
                    return BadRequest("Số lượng yêu cầu vượt quá số lượng có sẵn trong kho.");
                }
                return NotFound("Không tìm thấy giỏ hàng hoặc sản phẩm.");
            }
        }

        [HttpGet]
        [Route("GetAllByCartIDAsync/{IDCart}")]
        public async Task<IActionResult> GetAllByCartIDAsync(string IDCart)
        {
            var objVM = await _ICartOptionsService.GetAllByCartIDAsync(IDCart);

            return Ok(objVM);
        }
    }
}