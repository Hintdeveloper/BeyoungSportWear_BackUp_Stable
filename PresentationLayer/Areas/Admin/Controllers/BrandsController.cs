using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using BusinessLogicLayer.Viewmodels.Colors;
using Newtonsoft.Json;
using BusinessLogicLayer.Viewmodels.Brand;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("Admin")]
    [Route("home")]
    public class BrandsController : Controller
    {
        [HttpGet("brand/index")]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = "https://localhost:7241/api/Brand/GetAll";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var brands = JsonConvert.DeserializeObject<List<BrandVM>>(apiData);
                        return View(brands);
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
        [HttpGet("brand/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("brand/create")]
        public async Task<IActionResult> Create(BrandCreateVM brand)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    brand.CreateBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = "https://localhost:7241/api/Brand/BrandCreate";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PostAsJsonAsync(requestURL, brand);
                    if (response.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true});

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


        [HttpGet("brand/details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Brand/GetByID/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var brand = JsonConvert.DeserializeObject<BrandVM>(apiData);
                        return View(brand);

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

        [HttpGet("brand/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Brand/GetByID/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    var brand = JsonConvert.DeserializeObject<BrandUpdateVM>(apiData);
                    return View(brand);

                }
                catch (Exception)
                {

                    throw;
                }

            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("brand/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID, BrandUpdateVM brand)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    brand.ModifiedBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = $"https://localhost:7241/api/Brand/BrandUpdate/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PutAsJsonAsync(requestURL, brand);
                    if (response.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true});

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

        [HttpGet("brand/changestatus/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    string requestURL = $"https://localhost:7241/api/Brand/ChangeStatus/{id}";
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
