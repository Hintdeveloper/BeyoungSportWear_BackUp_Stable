using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Address;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpPost("create_address")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateVM addressCreateVM)
        {
            if (addressCreateVM == null)
            {
                return BadRequest("Invalid address data.");
            }

            var result = await _addressService.CreateAsync(addressCreateVM);

            if (result)
            {
                return Ok(new { Message = "Address created successfully." });
            }

            return StatusCode(500, "An error occurred while creating the address.");
        }
        [HttpGet("user/{IDUser}")]
        public async Task<IActionResult> GetAddressesByUserId(string IDUser)
        {
            if (string.IsNullOrEmpty(IDUser))
            {
                return BadRequest("Invalid user ID.");
            }

            var addresses = await _addressService.GetAddressByIDUserAsync(IDUser);

            if (addresses == null)
            {
                return NotFound("Addresses not found for the given user ID.");
            }

            return Ok(addresses);
        }
        [HttpGet("GetByIDAsync/{ID}")]
        public async Task<IActionResult> GetByIDAsync(Guid ID)
        {
            var address = await _addressService.GetByIDAsync(ID);

            if (address == null)
            {
                return NotFound("Address not found.");
            }

            return Ok(address);
        }
        [HttpPut("UpdateAddress/{ID}")]
        public async Task<IActionResult> UpdateAddress(Guid ID, [FromBody] AddressUpdateVM addressUpdateVM)
        {
            if (addressUpdateVM == null)
            {
                return BadRequest("Invalid address data.");
            }

            var result = await _addressService.UpdateAsync(ID, addressUpdateVM);

            if (result)
            {
                return Ok(new { Message = "Address updated successfully." });
            }

            return StatusCode(500, "An error occurred while updating the address.");
        }

        [HttpDelete("DeleteAddress/{ID}")]
        public async Task<IActionResult> DeleteAddress(Guid ID, [FromQuery] string IDUserDelete)
        {
            var obj = await _addressService.RemoveAsync(ID, IDUserDelete);
            if (obj)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addressService.GetAllAsync();

            if (addresses == null)
            {
                return NotFound("No addresses found.");
            }

            return Ok(addresses);
        }

        [HttpGet("GetAllActive")]
        public async Task<IActionResult> GetAllActiveAddresses()
        {
            var addresses = await _addressService.GetAllActiveAsync();

            if (addresses == null)
            {
                return NotFound("No active addresses found.");
            }

            return Ok(addresses);
        }
        [HttpPut("set-default/{id}")]
        public async Task<IActionResult> SetDefaultAddress(Guid id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "ID người dùng không được để trống." });
            }

            var result = await _addressService.SetDefaultAddressAsync(id, userId);

            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy địa chỉ hoặc không thể đặt làm mặc định." });
            }

            return Ok(new { message = "Địa chỉ đã được đặt làm mặc định thành công." });
        }
    }
}
