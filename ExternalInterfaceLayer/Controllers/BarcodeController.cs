using BusinessLogicLayer.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeController : ControllerBase
    {
        private readonly IBarcodeGeneratorService _barcodeGeneratorService;

        public BarcodeController(IBarcodeGeneratorService barcodeGeneratorService)
        {
            _barcodeGeneratorService = barcodeGeneratorService;
        }

        [HttpGet("generate_barcode/{KeyCode}")]
        public async Task<IActionResult> GenerateBarcode(string KeyCode)
        {
            try
            {
                string base64Barcode = await _barcodeGeneratorService.GenerateBarcode(KeyCode);

                return Ok(new { Barcode = base64Barcode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra trong quá trình tạo mã vạch: {ex.Message}");
            }
        }
    }
}
