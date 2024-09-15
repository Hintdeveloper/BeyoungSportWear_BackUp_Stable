using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpGet]
        [Route("{ID}/User")]
        public async Task<ActionResult<List<VoucherUserVM>>> GetUserInVoucher(Guid ID)
        {
            try
            {
                var user = await _voucherService.GetUserInVoucher(ID);
                if (user == null)
                {
                    return NotFound(); 
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{code}")]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetByCodeAsync(code);

            if (voucher == null)
            {
                return NotFound("Voucher không tồn tại."); 
            }

            return Ok(voucher);
        }
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllAsync()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return Ok(vouchers);
        }
        [HttpGet]
        [Route("getallactive")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var vouchers = await _voucherService.GetAllActiveAsync();
            return Ok(vouchers);
        }
        [HttpGet("GetByID/{ID}")]
        public async Task<IActionResult> GetVoucher(Guid ID)
        {
            var voucher = await _voucherService.GetByIDAsync(ID);
            if (voucher == null)
                return NotFound();

            return Ok(voucher);
        }
        [HttpGet]
        [Route("GetVoucherByIDUser/{IDUser}")]
        public async Task<IActionResult> GetVoucherByIDUser(string IDUser)
        {
            var datavoucher = await _voucherService.GetVoucherByUser(IDUser);
            return Ok(datavoucher);
        }
        [HttpGet("vouchers-public-private")]
        public async Task<ActionResult<List<VoucherVM>>> GetVouchersAsync([FromQuery] string? idUser = null)
        {
            var vouchers = await _voucherService.GetVouchersAsync(idUser);
            return Ok(vouchers);
        }
    }
}
