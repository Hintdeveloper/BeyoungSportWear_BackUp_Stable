using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Colors;
using BusinessLogicLayer.Viewmodels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _IProductService;

        public ProductController(IProductService IProductService)
        {
            _IProductService = IProductService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _IProductService.GetAllAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _IProductService.GetAllActiveAsync();
            return Ok(obj);
        }
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IProductService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [HttpPost]
        [Route("ProductCreate")]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IProductService.CreateAsync(request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to create Product");
        }
        [HttpPut]
        [Route("ProductUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, ProductUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _IProductService.UpdateAsync(ID, request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to update Product");
        }
        [HttpDelete("ProductRemove/{ID}/{IDUserDelete}")]
        public async Task<IActionResult> Remove(Guid ID, string IDUserDelete)
        {
            var obj = await _IProductService.RemoveAsync(ID, IDUserDelete);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return NotFound();
        }
    }
}
