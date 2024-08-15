using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherUserController : ControllerBase
    {
        private readonly IVoucherUserService _voucherUserService;

        public VoucherUserController(IVoucherUserService voucherUserService)
        {
            _voucherUserService = voucherUserService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherUserCreateVM createVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _voucherUserService.CreateAsync(createVM);
            if (!result)
                return BadRequest("Could not create VoucherUser");

            return Ok(new { status = "Success", message = "Successfully." });
        }

        [HttpGet("{IDVoucher}/{IDUser}")]
        public async Task<IActionResult> GetByID(Guid IDVoucher, string IDUser)
        {
            var voucherUser = await _voucherUserService.GetByIDAsync(IDVoucher, IDUser);
            if (voucherUser == null)
                return NotFound();

            return Ok(voucherUser);
        }

        [HttpPut("{IDVoucher}/{IDUser}")]
        public async Task<IActionResult> Update(Guid IDVoucher, string IDUser, [FromBody] VoucherUserUpdateVM updateVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _voucherUserService.UpdateAsync(IDVoucher, IDUser, updateVM);
            if (!result)
                return BadRequest("Could not update VoucherUser");

            return Ok(new { status = "Success", message = "Successfully." });
        }

        [HttpDelete("{IDVoucher}/{IDUser}/{IDUserDelete}")]
        public async Task<IActionResult> Delete(Guid IDVoucher, string IDUser, Guid IDUserDelete)
        {
            var result = await _voucherUserService.RemoveAsync(IDVoucher, IDUser, IDUserDelete);
            if (!result)
                return NotFound();

            return Ok(new { status = "Success", message = "Successfully." });
        }
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAll()
        {
            var Obj = await _voucherUserService.GetAllAsync();
            return Ok(Obj);
        }

        [HttpGet("getallactive")]
        public async Task<IActionResult> GetAllActive()
        {
            var Obj = await _voucherUserService.GetAllActiveAsync();
            return Ok(Obj);
        }
    }
}
