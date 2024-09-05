using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("warning/page_error_404")]
        public async Task<IActionResult> Error_404()
        {
            return View();
        }      
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser registerUser, string role)
        {
            role = "Client";
            string requestURL = $"https://localhost:7241/api/ApplicationUser/Register?role={role}";
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(requestURL, registerUser);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // Log the error message or inspect it for further details
                return BadRequest($"Server returned error: {errorMessage}");
            }

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel userLogin)
        {
            string requestURL = "https://localhost:7241/api/ApplicationUser/Login";
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(requestURL, userLogin);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Response>();
                var token = result.Token;

                HttpContext.Response.Cookies.Append("JWTUserInfo", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                if (roleClaim != null)
                {
                    var role = roleClaim.Value;
                    if (role == "Staff" || role == "Admin")
                    {
                       return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (role == "Client")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Default redirection if no role is found
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login failed");
                return View(userLogin);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
