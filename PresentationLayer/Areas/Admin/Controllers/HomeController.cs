using BusinessLogicLayer.Viewmodels.ApplicationUser;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        [HttpGet("home/index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet("home/account")]
        public async Task<IActionResult> Account(UserDataVM user)
        {
            if(HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    user.ID = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetInformationUser/{user.ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var users = JsonConvert.DeserializeObject<UserDataVM>(apiData);
                        return View(users);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return Unauthorized();
        }
    }
}
