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
using BusinessLogicLayer.Viewmodels.Sizes;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]
    [Route("home")]
    public class SizeController : Controller
    {
        [HttpGet("size/index")]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = "https://localhost:7241/api/Sizes/GetAll";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var sizes = JsonConvert.DeserializeObject<List<SizeVM>>(apiData);
                        return View(sizes); 
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

        [HttpGet("size/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("size/create")]
        public async Task<IActionResult> Create(SizeCreateVM size)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    size.CreateBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                    string requestURL = "https://localhost:7241/api/Sizes/SizesCreate";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PostAsJsonAsync(requestURL, size);
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


        [HttpGet("size/details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Sizes/GetByID/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var sizes = JsonConvert.DeserializeObject<SizeVM>(apiData);
                        return View(sizes);
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

        [HttpGet("size/edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Sizes/GetByID/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    var sizes = JsonConvert.DeserializeObject<SizeUpdateVM>(apiData);
                    return View(sizes);
                }
                catch (Exception)
                {

                    throw;
                } 
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("size/edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, SizeUpdateVM size)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    size.ModifiedBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                    string requestURL = $"https://localhost:7241/api/Sizes/SizesUpdate/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PutAsJsonAsync(requestURL, size);
                    if (response.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true});

                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        //return BadRequest();
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

        [HttpGet("size/changestatus/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Sizes/ChangeStatus/{id}";
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
