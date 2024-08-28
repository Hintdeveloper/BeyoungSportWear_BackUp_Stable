using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly IOptionsService _IOptionsService;
        public OptionsController(IOptionsService IOptionsService)
        {
            _IOptionsService = IOptionsService;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromForm] OptionsCreateSingleVM request)
        {
            if (request.ImageURL == null)
            {
                return BadRequest("No files received from the upload");
            }
            var result = await _IOptionsService.CreateAsync(request);
            if (result)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            else
            {
                return BadRequest(new { status = "Error", message = "There was an error uploading the Options." });
            }
        }
        [HttpGet("GetByProductDetailsId/{IDProductDetails}")]
        public async Task<IActionResult> GetByProductDetailsId(Guid IDProductDetails)
        {
            var options = await _IOptionsService.GetOptionsByProductDetailsIdAsync(IDProductDetails);
            if (options == null || !options.Any())
            {
                return NotFound("No options found for the given ProductDetails ID.");
            }

            return Ok(options);
        }
        [HttpPut]
        [Route("update/{ID}")]
        public async Task<IActionResult> UpdateOption(Guid ID, [FromForm] OptionsUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _IOptionsService.UpdateAsync(ID, request);

            if (!result)
            {
                return NotFound("Option not found.");
            }

            return Ok(new { status = "Success", message = "Successfully." });
        }
        [HttpPost("UpdateIsActive")]
        public async Task<IActionResult> UpdateIsActive([FromBody] UpdateIsActiveRequest request)
        {
            if (request == null || request.IDEntity == Guid.Empty)
            {
                return BadRequest("Invalid request data");
            }

            var result = await _IOptionsService.UpdateIsActiveAsync(request.IDEntity, request.IsActive);

            if (result)
            {
                return Ok("Option IsActive updated successfully");
            }
            else
            {
                return NotFound("Option not found");
            }
        }
        [HttpGet]
        [Route("GetProductDetailsByID/{IDOptions}")]
        public async Task<IActionResult> GetProductDetailsByID(Guid IDOptions)
        {
            var variantdata = await _IOptionsService.GetOptionsByProductDetailsIdAsync(IDOptions);
            return Ok(variantdata);
        }
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _IOptionsService.GetAllAsync();
            return Ok(obj);
        }

        [HttpGet]
        [Route("getallactive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _IOptionsService.GetAllActiveAsync();
            return Ok(obj);
        }

        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IOptionsService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [HttpPost("decrease-quantity")]
        public async Task<IActionResult> DecreaseQuantity([FromBody] DecreaseQuantityRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _IOptionsService.DecreaseQuantityAsync(request.IDOptions, request.QuantityToDecrease);
                    if (success)
                    {
                        return Ok("Số lượng đã được cập nhật.");
                    }
                    return BadRequest("Không thể cập nhật số lượng.");
                }
                return BadRequest(ModelState);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("increase-quantity")]
        public async Task<IActionResult> IncreaseQuantityAsync([FromBody] DecreaseQuantityRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _IOptionsService.IncreaseQuantityAsync(request.IDOptions, request.QuantityToDecrease);
                    if (success)
                    {
                        return Ok("Số lượng đã được cập nhật.");
                    }
                    return BadRequest("Không thể cập nhật số lượng.");
                }
                return BadRequest(ModelState);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }
    }
}
