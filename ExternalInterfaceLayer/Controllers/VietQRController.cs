using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.VietQR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VietQRController : ControllerBase
    {
        private readonly IVietQRService _vietQRService;

        public VietQRController(IVietQRService vietQRService)
        {
            _vietQRService = vietQRService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQR([FromBody] VietQRRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _vietQRService.GenerateQR(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
