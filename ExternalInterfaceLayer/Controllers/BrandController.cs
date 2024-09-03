using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Brand;
using BusinessLogicLayer.Viewmodels.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _IBrandService;

        public BrandController(IBrandService IBrandService)
        {
            _IBrandService = IBrandService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _IBrandService.GetAllAsync();
            return Ok(obj);
        }
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _IBrandService.GetAllActiveAsync();
            return Ok(obj);
        }
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IBrandService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("BrandCreate")]
        public async Task<IActionResult> Create(BrandCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IBrandService.CreateAsync(request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Brand uploaded successfully." });
            }
            return BadRequest("Failed to create Brand");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("BrandUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, BrandUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IBrandService.UpdateAsync(ID, request);
            if (obj)
            {
                return Ok("Successful action.");
            }
            return BadRequest("Failed to update Brand");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            var obj = await _IBrandService.SetStatus(ID);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest();
        }
    }
}
