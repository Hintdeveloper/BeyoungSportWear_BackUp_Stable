using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherMController : ControllerBase
    {
        private readonly IVoucherMServiece _voucherService;
        public VoucherMController(IVoucherMServiece voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateVoucherVM voucherCreateVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _voucherService.Create(voucherCreateVM);
            if (!created)
                return BadRequest("Could not create voucher");

            return Ok(new { status = "Success", message = "Successfully." });
        }
        //[HttpPut("EditVoucher/{ID}")]
        //public async Task<IActionResult> UpdateVoucher(Guid ID, [FromBody] UpdateVoucherVM voucherUpdateVM)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updated = await _voucherService.UpdateAsync(ID, voucherUpdateVM);
        //    if (!updated)
        //        return BadRequest("Could not update voucher");

        //    return Ok(new { status = "Success", message = "Successfully." });
        //}

        [HttpDelete("Delete/{ID}/{IDUserDelete}")]
        public async Task<IActionResult> DeleteVoucher(Guid ID, string IDUserdelete)
        {
            var deleted = await _voucherService.RemoveAsync(ID, IDUserdelete);
            if (!deleted)
                return NotFound();

            return Ok(new { status = "Success", message = "Successfully." });
        }
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var vouchers = await _voucherService.GetAll();
            return Ok(vouchers);
        }
        [HttpGet]
        [Route("getallClient")]
        public async Task<IActionResult> GetAllClient()
        {
            var client = await _voucherService.GetClientsAsync();
            return Ok(client);
        }

        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetbyId(Guid Id)
        {
            var voucher = await _voucherService.GetByIDAsync(Id);

            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateVoucherStatus()
        {
            await _voucherService.UpdateVoucherStatusesAsync();
            return Ok("Voucher statuses updated successfully.");
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchVouchers([FromQuery] string input)
        {
            var vouchers = await _voucherService.SearchVouchersAsync(input);
            return Ok(vouchers);
        }
        [HttpPut("Edit_VoucherUser/{ID}")]
        public async Task<IActionResult> UpdateVoucherUser(Guid ID, [FromBody] UpdateVC voucherUpdateVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Xử lý dữ liệu voucherUpdateVM và cập nhật vào cơ sở dữ liệu
            var updated = await _voucherService.UpdateVoucherUser(ID, voucherUpdateVM);
            if (!updated)
                return BadRequest("Could not update voucher");

            return Ok(new { status = "Success", message = "Successfully." });
        }
        [HttpGet("GetVoucherUsers/{id}")]
        public async Task<IActionResult> GetVoucherUsers(Guid id)
        {
            try
            {
                var users = await _voucherService.GetVoucherUsersAsync(id);
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception and return a server error response
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("CheckVoucherCodeExists")]
        public async Task<IActionResult> CheckVoucherCodeExists(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Code is required.");
            }

            bool exists = await _voucherService.IsVoucherCodeExistsAsync(code);

            return Ok(new { Exists = exists });
        }
        [HttpPost("ToggleStatus/{id}/{userId}")]
        public async Task<IActionResult> ToggleStatus(Guid id, string userId)
        {
            var result = await _voucherService.ToggleVoucherStatusAsync(id, userId);

            if (!result)
            {
                return BadRequest(new { status = "Error", message = "Failed to update voucher status." });
            }

            return Ok(new { status = "Success", message = "Voucher status updated successfully." });
        }

        [HttpGet("GetVouchersByUserId")]
        public async Task<IActionResult> GetVouchersByUserIdWithStatusAsync(string idUser)
        {
            var vouchers = await _voucherService.GetVouchersByUserIdWithStatusAsync(idUser);

            if (vouchers == null || !vouchers.Any())
            {
                return NotFound("No vouchers found for the given user ID.");
            }

            return Ok(vouchers);
        }
        [HttpGet("filter-by-date")]
        public async Task<IActionResult> FilterVouchersByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var vouchers = await _voucherService.FilterVouchersByDateRangeAsync(startDate, endDate);
                return Ok(vouchers); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error filtering vouchers", details = ex.Message });
            }
        }
        [HttpGet("search-by-status")]
        public async Task<IActionResult> SearchByStatus(int isActive)
        {
            try
            {
                var vouchers = await _voucherService.GetVouchersByStatus(isActive);
                return Ok(vouchers);

            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Error filtering vouchers", details = ex.Message });
            }
        }
            
        }
    
}
