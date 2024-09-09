using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.ProductDetails;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using BusinessLogicLayer.Viewmodels.Options;
using Microsoft.AspNetCore.Authorization;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly IProductDetailsService _IProductDetailsService;

        public ProductDetailsController(IProductDetailsService IProductDetailsService)
        {
            _IProductDetailsService = IProductDetailsService;
        }
        [HttpPost]
        [Route("productdetails_create")]
        public async Task<IActionResult> Create([FromBody] ProductDetailsCreateVM request)
        {
            var result = await _IProductDetailsService.CreateAsync(request);
            if (result)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            else
            {
                return BadRequest(new { status = "Error", message = "There was an error uploading the productDetails." });
            }
        }
        [AllowAnonymous]
        [HttpGet("GetByProductDetailsId/{IDProductDetails}")]
        public async Task<IActionResult> GetByProductDetailsId(Guid IDProductDetails)
        {
            var options = await _IProductDetailsService.GetOptionProductDetailsByIDAsync(IDProductDetails);
            if (options == null || !options.Any())
            {
                return NotFound("No options found for the given ProductDetails ID.");
            }

            return Ok(options);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(int pageIndex = 0, int pageSize = 10)
        {
            var obj = await _IProductDetailsService.GetAllAsync(pageIndex, pageSize);
            return Ok(obj);
        }

        [HttpPost("UpdateIsActive")]
        public async Task<IActionResult> UpdateIsActive([FromBody] UpdateIsActiveRequest request)
        {
            if (request == null || request.IDEntity == Guid.Empty)
            {
                return BadRequest("Invalid request data");
            }

            var result = await _IProductDetailsService.UpdateIsActiveAsync(request.IDEntity, request.IsActive);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound("Option not found");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllActive")]
        public async Task<IActionResult> GetAllActive(int pageIndex = 0, int pageSize = 10)
        {
            var obj = await _IProductDetailsService.GetAllActiveAsync(pageIndex, pageSize);
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByID/{ID}")]
        public async Task<IActionResult> GetByID(Guid ID)
        {
            var obj = await _IProductDetailsService.GetByIDAsync(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByIDAsyncVer_1/{ID}")]
        public async Task<IActionResult> GetByIDAsyncVer_1(Guid ID)
        {
            var obj = await _IProductDetailsService.GetByIDAsyncVer_1(ID);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [HttpPut]
        [Route("Update/{ID}")]
        public async Task<IActionResult> Update(Guid ID, [FromBody] ProductDetailsUpdateVM request)
        {
            var productdetails = await _IProductDetailsService.GetByIDAsyncVer_1(ID);

            if (productdetails != null)
            {
                var data = await _IProductDetailsService.UpdateAsync(ID, request);
                return Ok(data);
            }

            return NotFound();
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{ID}/{IDUserDelete}")]
        public async Task<IActionResult> Remove(Guid ID, string IDUserDelete)
        {
            var success = await _IProductDetailsService.RemoveAsync(ID, IDUserDelete);
            if (success)
            {
                return Ok();
            }
            return NotFound();
        }
        [AllowAnonymous]
        [HttpPost("search")]
        public ActionResult<IEnumerable<Product>> Search([FromBody] List<SearchCondition> conditions)
        {
            var result = _IProductDetailsService.Search(conditions).ToList();

            if (result.Count == 0)
            {
                return NotFound("No products found matching the search criteria.");
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("GetProductDetails_IDNameAsync/ids")]
        public async Task<IActionResult> GetProductDetails_IDNameAsync()
        {
            try
            {
                var productIds = await _IProductDetailsService.GetProductDetails_IDNameAsync();
                return Ok(productIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("GetOptionsByProductDetailsId/{IDProductDetails}")]
        public async Task<ActionResult<List<OptionsVM>>> GetOptionsByProductDetailsIdAsync(Guid IDProductDetails)
        {
            try
            {
                var options = await _IProductDetailsService.GetOptionProductDetailsByIDAsync(IDProductDetails);
                return Ok(options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetProductDetailInfo/{IDProductDetails}")]
        public async Task<IActionResult> GetProductDetailInfo(Guid IDProductDetails, [FromQuery] string size, [FromQuery] string color)
        {
            try
            {
                var productDetail = await _IProductDetailsService.GetProductDetailInfo(IDProductDetails, size, color);
                return Ok(productDetail);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("products/price-range")]
        public async Task<IActionResult> GetProductsByPriceRangeAsync(/*Guid IDProductDetails,*/ [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            if (minPrice > maxPrice)
            {
                return BadRequest("Minimum price cannot be greater than maximum price.");
            }

            var products = await _IProductDetailsService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            if (products == null || !products.Any())
            {
                return NotFound("No products found within the given price range for the specified category.");
            }

            return Ok(products);
        }
    }
}
