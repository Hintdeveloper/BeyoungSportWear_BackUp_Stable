using BusinessLogicLayer.Viewmodels.Colors;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]
    [Route("home")]
    public class ColorController : Controller
    {
        [HttpGet("color/index")]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = "https://localhost:7241/api/Colors/GetAll";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var colors = JsonConvert.DeserializeObject<List<ColorVM>>(apiData);
                        return View(colors);
                    }
                }
                catch (Exception)
                {
                    return BadRequest();
                    throw;
                }
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("color/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("color/create")]
        public async Task<IActionResult> Create(ColorCreateVM colors)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    colors.CreateBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = "https://localhost:7241/api/Colors/ColorsCreate";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PostAsJsonAsync(requestURL, colors);
                    if (response.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true });

                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        // Log the error message or inspect it for further details
                        //return BadRequest($"Server returned error: {errorMessage}");
                        return Json(new { isSuccess = false, errorMessage = errorMessage });

                    }
                }
                catch (Exception)
                {
                    return BadRequest();
                    throw;
                }
            }
            return Unauthorized();
        }


        [HttpGet("color/details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Colors/GetByID/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var colors = JsonConvert.DeserializeObject<ColorVM>(apiData);
                        return View(colors);

                    }
                }
                catch (Exception)
                {
                    return BadRequest();
                    throw;
                }
            }
            return Unauthorized();

        }
        [Authorize(Roles = "Admin")]

        [HttpGet("color/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Colors/GetByID/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    var colors = JsonConvert.DeserializeObject<ColorUpdateVM>(apiData);
                    return View(colors);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("color/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID, ColorUpdateVM colors)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    colors.ModifiedBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = $"https://localhost:7241/api/Colors/ColorsUpdate/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PutAsJsonAsync(requestURL, colors);
                    if (response.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true });

                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        // Log the error message or inspect it for further details
                        //return BadRequest($"Server returned error: {errorMessage}");
                        return Json(new { isSuccess = false, errorMessage = errorMessage });

                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("colors/changestatus/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Colors/ChangeStatus/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest();
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
