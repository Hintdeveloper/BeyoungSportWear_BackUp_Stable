using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _IColorsService;

        public ColorsController(IColorService IColorsService)
        {
            _IColorsService = IColorsService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _IColorsService.GetAllAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _IColorsService.GetAllActiveAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IColorsService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("ColorsCreate")]
        public async Task<IActionResult> Create(ColorCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IColorsService.CreateAsync(request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to create Colors");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("ColorsUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, ColorUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IColorsService.UpdateAsync(ID, request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to update Colors");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            var obj = await _IColorsService.SetStatus(ID);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest();
        }
    }
}
