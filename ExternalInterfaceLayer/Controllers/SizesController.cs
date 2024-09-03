using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Sizes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _ISizesService;

        public SizesController(ISizeService ISizesService)
        {
            _ISizesService = ISizesService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _ISizesService.GetAllAsync();
            return Ok(materials);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var materials = await _ISizesService.GetAllActiveAsync();
            return Ok(materials);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var material = await _ISizesService.GetByIDAsync(ID);
            if (material == null)
            {
                return NotFound();
            }
            return Ok(material);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("SizesCreate")]
        public async Task<IActionResult> Create(SizeCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _ISizesService.CreateAsync(request);
            if (success)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to create Sizes");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("SizesUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, SizeUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _ISizesService.UpdateAsync(ID, request);
            if (success)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to update Sizes");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            var obj = await _ISizesService.SetStatus(ID);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest();
        }
    }
}
