using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Category;
using BusinessLogicLayer.Viewmodels.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _ICategoryService;

        public CategoryController(ICategoryService ICategoryService)
        {
            _ICategoryService = ICategoryService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = await _ICategoryService.GetAllAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            var obj = await _ICategoryService.GetAllActiveAsync();
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _ICategoryService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("CategoryCreate")]
        public async Task<IActionResult> Create(CategoryCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _ICategoryService.CreateAsync(request);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return BadRequest("Failed to create");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("CategoryUpdate/{ID}")]
        public async Task<IActionResult> Update(Guid ID, CategoryUpdateVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = await _ICategoryService.UpdateAsync(ID, request);
            if (obj)
            {
                return Ok("Successful action.");
            }
            return BadRequest("Failed to update");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            var obj = await _ICategoryService.SetStatus(ID);
            if (obj)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            return NotFound();
        }
        [AllowAnonymous]
        [HttpGet("{categoryID}/products")]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryID)
        {
            var products = await _ICategoryService.GetProductsByCategoryAsync(categoryID);
            return Ok(products);
        }
        [AllowAnonymous]
        [HttpGet("{id}/products/price-range")]
        public async Task<IActionResult> GetProductsByPriceRangeAsync(Guid id, [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            if (minPrice > maxPrice)
            {
                return BadRequest("Minimum price cannot be greater than maximum price.");
            }

            var products = await _ICategoryService.GetProductsByPriceRangeAsync(id, minPrice, maxPrice);
            if (products == null || !products.Any())
            {
                return NotFound("No products found within the given price range for the specified category.");
            }

            return Ok(products);
        }
    }
}
