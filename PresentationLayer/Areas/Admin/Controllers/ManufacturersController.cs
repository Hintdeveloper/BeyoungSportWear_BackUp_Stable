using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using BusinessLogicLayer.Viewmodels.Material;
using Newtonsoft.Json;
using BusinessLogicLayer.Viewmodels.Manufacturer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("Admin")]
    [Route("home")]
    public class ManufacturersController : Controller
    {
        [HttpGet("manufacturer/index")]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = "https://localhost:7241/api/Manufacturer/GetAll";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var manufacturers = JsonConvert.DeserializeObject<List<ManufacturerVM>>(apiData);
                        return View(manufacturers);

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

        [HttpGet("manufacturer/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("manufacturer/create")]
        public async Task<IActionResult> Create(ManufacturerCreateVM manufacturer)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    manufacturer.CreateBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = "https://localhost:7241/api/Manufacturer/ManufacturerCreate";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PostAsJsonAsync(requestURL, manufacturer);
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


        [HttpGet("manufacturer/details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Manufacturer/GetByID/{id}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var material = JsonConvert.DeserializeObject<ManufacturerVM>(apiData);
                        return View(material);

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

        [HttpGet("manufacturer/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    string requestURL = $"https://localhost:7241/api/Manufacturer/GetByID/{ID}";
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    var material = JsonConvert.DeserializeObject<ManufacturerUpdateVM>(apiData);
                    return View(material);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("manufacturer/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID, ManufacturerUpdateVM manufacturer)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    manufacturer.ModifiedBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = $"https://localhost:7241/api/Manufacturer/ManufacturerUpdate/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PutAsJsonAsync(requestURL, manufacturer);
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

        [HttpGet("manufacturer/changestatus/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/Manufacturer/ChangeStatus/{id}";
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
