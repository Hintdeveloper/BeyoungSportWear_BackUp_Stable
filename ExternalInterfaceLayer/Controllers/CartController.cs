using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Cart;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly ICartService _ICartService;
        public CartController(ICartService ICartService)
        {
            _ICartService = ICartService;
        }
        [HttpPost]
        [Route("CartCreate")]
        public async Task<IActionResult> Create([FromBody] CartCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ICartService.CreateAsync(model);
            if (!result)
            {
                return BadRequest(new { Message = "Error creating Cart" });
            }

            return Ok(new { status = "Success", message = "Successfully." });
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var classifies = await _ICartService.GetAllAsync();
            return Ok(classifies);
        }
        [HttpGet("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var classifies = await _ICartService.GetAllActiveAsync();
            return Ok(classifies);
        }
        [HttpGet("GetByID/{ID}")]
        public async Task<IActionResult> GetById(Guid ID)
        {
            var classify = await _ICartService.GetByIDAsync(ID);
            if (classify == null)
            {
                return NotFound();
            }

            return Ok(classify);
        }

        [HttpGet("cart/user/{IDUser}")]
        public async Task<IActionResult> GetCartsByUserID(string IDUser)
        {
            var carts = await _ICartService.GetByUserIDAsync(IDUser);
            if (carts == null || !carts.Any())
            {
                return NotFound();
            }
            return Ok(carts);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> Update(Guid ID, [FromBody] CartUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ICartService.UpdateAsync(ID, request);
            if (!result)
            {
                return BadRequest(new { Message = "Error updating Cart" });
            }

            return Ok(new { status = "Success", message = "Successfully." });
        }

        [HttpDelete("{ID}")]
        public async Task<IActionResult> Delete(Guid ID, [FromQuery] string IDUserdelete)
        {
            var result = await _ICartService.RemoveAsync(ID, IDUserdelete);
            if (!result)
            {
                return BadRequest(new { Message = "Error deleting Cart" });
            }

            return Ok(new { status = "Success", message = "Successfully." });
        }

    }
}
