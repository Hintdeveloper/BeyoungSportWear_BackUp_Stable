using BusinessLogicLayer.Viewmodels.Category;
using BusinessLogicLayer.Viewmodels.ProductDetails;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PresentationLayer.Areas.Admin.Models;
using PresentationLayer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        [Route("index/category")]
        public async Task<IActionResult> Index(Guid? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            // URL lấy danh sách danh mục
            string categoryRequestURL = "https://localhost:7241/api/Category/GetAllActive";
            var httpClient = new HttpClient();
            // Lấy danh sách danh mục
            var categoryResponse = await httpClient.GetAsync(categoryRequestURL);
            var categories = JsonConvert.DeserializeObject<List<CategoryVM>>(await categoryResponse.Content.ReadAsStringAsync());

            // Xây dựng URL để lấy sản phẩm
            string productRequestURL = "https://localhost:7241/api/ProductDetails/GetAllActive"; // Default URL

            if (categoryId.HasValue)
            {
                // Nếu có categoryId thì lấy sản phẩm theo danh mục
                productRequestURL = $"https://localhost:7241/api/Category/{categoryId}/products";
            }

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                // Nếu có khoảng giá, thêm vào URL
                productRequestURL = categoryId.HasValue
                    ? $"https://localhost:7241/api/Category/{categoryId}/products/price-range?minPrice={minPrice.Value}&maxPrice={maxPrice.Value}"
                    : $"https://localhost:7241/api/ProductDetails/products/price-range?minPrice={minPrice.Value}&maxPrice={maxPrice.Value}";
            }

            // Lấy danh sách sản phẩm
            var productResponse = await httpClient.GetAsync(productRequestURL);
            var products = JsonConvert.DeserializeObject<List<ProductDetailsVM>>(await productResponse.Content.ReadAsStringAsync());

            var viewModel = new ShopViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategoryID = categoryId
            };

            return View(viewModel);
        }
    }
}
