using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Colors;
using BusinessLogicLayer.Viewmodels.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _IMaterialService;

        public MaterialController(IMaterialService IMaterialService)
        {
            _IMaterialService = IMaterialService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _IMaterialService.GetAllAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _IMaterialService.GetAllActiveAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IMaterialService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("MaterialCreate")]
        public async Task<IActionResult> Create(MaterialCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IMaterialService.CreateAsync(request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to create Material");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("MaterialUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, MaterialUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IMaterialService.UpdateAsync(ID, request);
            if (obj)
            {
                return Ok("Successful action.");
            }
            return BadRequest("Failed to update Material");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            var obj = await _IMaterialService.SetStatus(ID);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest();
        }
    }
}
