using BusinessLogicLayer.Viewmodels.VoucherM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;

namespace PresentationLayer.Controllers
{
    public class UserVoucherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UserVoucherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
       
        [HttpGet("UserVoucher/GetVoucherByUserId")]
        public async Task<IActionResult> GetVouchersByUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                userId = "1";
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_token_here");
            var url = $"https://localhost:7241/api/VoucherM/GetVouchersByUserId?idUser={userId}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var vouchers = JsonConvert.DeserializeObject<List<VoucherViewModel>>(jsonResponse);
                return View(vouchers);
            }
            else
            {
                return View(new List<VoucherViewModel>());
            }
        }
    }
}
