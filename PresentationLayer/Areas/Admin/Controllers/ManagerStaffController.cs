﻿using BusinessLogicLayer.Viewmodels.Address;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PresentationLayer.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ManagerStaffController : Controller
    {
        [HttpGet("home/index_staff")]
        public async Task<IActionResult> Index(string name, string phone, string email)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    List<UserDataVM> users = null;

                    // Kiểm tra từng tiêu chí và gọi các phương thức tương ứng
                    if (!string.IsNullOrEmpty(email))
                    {
                        var result = await GetUserByEmail(email);
                        if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                        {
                            users = userList;
                        }
                    }
                    else if (!string.IsNullOrEmpty(phone))
                    {
                        var result = await GetUserByPhoneNumber(phone);
                        if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                        {
                            users = userList;
                        }
                    }
                    else if (!string.IsNullOrEmpty(name))
                    {
                        var result = await GetUserByName(name);
                        if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                        {
                            users = userList;
                        }
                    }
                    else
                    {
                        string requestURL = $"https://localhost:7241/api/ApplicationUser/GetAllInformationUserAsync";
                        var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        var response = await httpClient.GetAsync(requestURL);

                        if (response.IsSuccessStatusCode)
                        {
                            string apiData = await response.Content.ReadAsStringAsync();
                            users = JsonConvert.DeserializeObject<List<UserDataVM>>(apiData);
                        }
                        else
                        {
                            return BadRequest($"API call failed with status code: {response.StatusCode}");
                        }
                    }

                    // Lọc người dùng theo tất cả các tiêu chí (name, phone, email) và role
                    if (users != null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            users = users.Where(u =>
                                    u.FirstAndLastName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                    .Any(part => part.Equals(name, StringComparison.OrdinalIgnoreCase))
                                    ).ToList();
                        }
                        if (!string.IsNullOrEmpty(phone))
                        {
                            users = users.Where(u => u.PhoneNumber == phone).ToList();
                        }
                        if (!string.IsNullOrEmpty(email))
                        {
                            users = users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();
                        }

                        // Lọc người dùng có vai trò là Staff
                        users = users.Where(u => u.RoleName == "Staff").ToList();
                    }

                    if (users == null || !users.Any())
                    {
                        ViewBag.Message = "Không tìm thấy người dùng nào";
                    }

                    return View(users);

                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Exception: {ex.Message}");
                    return BadRequest("An error occurred while processing the request.");
                }
            }
            return Unauthorized();
        }
        [HttpGet("home/index_staff/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost("home/index_staff/create")]
        public async Task<IActionResult> Create(RegisterUser registerUser, string role)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    role = "Staff";

                    registerUser.Password = GenerateRandomPassword(10);
                    registerUser.ConfirmPassword = registerUser.Password;
                    string requestURL = $"https://localhost:7241/api/ApplicationUser/Register?role={role}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.PostAsJsonAsync(requestURL, registerUser);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("User's Password: " + registerUser.Password);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        // Log the error message or inspect it for further details
                        return BadRequest($"Server returned error: {errorMessage}");
                    }

                }
                catch (Exception)
                {

                    throw;
                }

            }
            return Unauthorized();
        }
        [HttpGet("home/index_staff/details/{ID}")]
        public async Task<IActionResult> Details(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetInformationUser/{ID}";
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
                    return BadRequest();
                    throw;
                }
            }
            return Unauthorized();
        }
        [HttpGet("home/index_staff/edit/{ID}")]
        public async Task<IActionResult> Edit(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetInformationUser/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var users = JsonConvert.DeserializeObject<UserUpdateVM>(apiData);
                        return View(users);
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
        [HttpPost("home/index_staff/edit/{ID}")]
        public async Task<IActionResult> Edit(UserUpdateVM userUpdate, Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);
                    userUpdate.ModifiedBy = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/UpdateUser/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    // Tạo MultipartFormDataContent để gửi dữ liệu dạng form
                    var formData = new MultipartFormDataContent();

                    // Thêm các trường dữ liệu vào form-data
                    formData.Add(new StringContent(userUpdate.ModifiedBy ?? string.Empty), nameof(userUpdate.ModifiedBy));
                    formData.Add(new StringContent(userUpdate.FirstAndLastName ?? string.Empty), nameof(userUpdate.FirstAndLastName));
                    formData.Add(new StringContent(userUpdate.Email ?? string.Empty), nameof(userUpdate.Email));
                    formData.Add(new StringContent(userUpdate.PhoneNumber ?? string.Empty), nameof(userUpdate.PhoneNumber));
                    formData.Add(new StringContent(userUpdate.Gender?.ToString() ?? string.Empty), nameof(userUpdate.Gender));
                    formData.Add(new StringContent(userUpdate.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty), nameof(userUpdate.DateOfBirth));

                    // Thêm hình ảnh nếu có
                    if (userUpdate.Images != null)
                    {
                        var stream = userUpdate.Images.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(userUpdate.Images.ContentType);
                        formData.Add(fileContent, nameof(userUpdate.Images), userUpdate.Images.FileName);
                    }

                    // Thêm địa chỉ nếu có
                    if (userUpdate.AddressUpdateVM != null)
                    {
                        formData.Add(new StringContent(userUpdate.AddressUpdateVM.SpecificAddress ?? string.Empty), "AddressUpdateVM.SpecificAddress");
                        formData.Add(new StringContent(userUpdate.AddressUpdateVM.City ?? string.Empty), "AddressUpdateVM.City");
                        formData.Add(new StringContent(userUpdate.AddressUpdateVM.DistrictCounty ?? string.Empty), "AddressUpdateVM.DistrictCounty");
                        formData.Add(new StringContent(userUpdate.AddressUpdateVM.Commune ?? string.Empty), "AddressUpdateVM.Commune");
                    }

                    var response = await httpClient.PutAsync(requestURL, formData);
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
                catch (Exception)
                {

                    throw;
                }

            }
            return Unauthorized();
        }
        [HttpGet("home/index_staff/changestatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/SetStatus/{ID}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    var response = await httpClient.GetAsync(requestURL);
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
                catch (Exception)
                {

                    throw;
                }
            }
            return Unauthorized();
        }
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);


                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetUsersByEmail?email={email}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly
                    var response = await httpClient.GetAsync(requestURL);



                    if (response.IsSuccessStatusCode)
                    {
                        string apiData = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserDataVM>>(apiData);
                        return View(users);
                    }
                    else
                    {
                        return BadRequest($"API call failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Exception: {ex.Message}");
                    return BadRequest("An error occurred while processing the request.");
                }
            }
            return Unauthorized();
        }
        public async Task<IActionResult> GetUserByPhoneNumber(string phone)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);


                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetUsersByPhoneNumber?phoneNumber={phone}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly
                    var response = await httpClient.GetAsync(requestURL);



                    if (response.IsSuccessStatusCode)
                    {
                        string apiData = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserDataVM>>(apiData);
                        return View(users);
                    }
                    else
                    {
                        return BadRequest($"API call failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Exception: {ex.Message}");
                    return BadRequest("An error occurred while processing the request.");
                }
            }
            return Unauthorized();
        }
        public async Task<IActionResult> GetUserByName(string name)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);


                    string requestURL = $"https://localhost:7241/api/ApplicationUser/GetUsersByName?name={name}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly
                    var response = await httpClient.GetAsync(requestURL);



                    if (response.IsSuccessStatusCode)
                    {
                        string apiData = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserDataVM>>(apiData);
                        return View(users);
                    }
                    else
                    {
                        return BadRequest($"API call failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Exception: {ex.Message}");
                    return BadRequest("An error occurred while processing the request.");
                }
            }
            return Unauthorized();
        }
        public async Task<IActionResult> GetProvince()
        {

            string requestURL = "https://localhost:7241/api/ApplicationUser/get-province";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(requestURL);

            string apiData = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var province = JsonConvert.DeserializeObject<List<RegisterUser>>(apiData);
                return Ok(province);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // Log the error message or inspect it for further details
                return BadRequest();
            }
        }
        public async Task<IActionResult> GetDistrict(int cityID)
        {
            string requestURL = $"https://localhost:7241/api/ApplicationUser/get-district?cityID={cityID}";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(requestURL);

            string apiData = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var district = JsonConvert.DeserializeObject<List<AddressVM>>(apiData);
                return Ok(district);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // Log the error message or inspect it for further details
                return BadRequest();
            }
        }
        public async Task<IActionResult> GetWard(int city_districtID)
        {
            string requestURL = $"https://localhost:7241/api/ApplicationUser/get-ward?city_districtID={city_districtID}";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(requestURL);

            string apiData = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var ward = JsonConvert.DeserializeObject<List<AddressVM>>(apiData);
                return Ok(ward);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                // Log the error message or inspect it for further details
                return BadRequest();
            }
        }

        public string GenerateRandomPassword(int length)
        {
            // Ensure the length is at least 4 to accommodate all required characters
            if (length < 4)
            {
                length = 10;
            }

            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "1234567890";
            const string specialChars = "!@#$%^&*()";
            const string validChars = "abcdefghijklmnopqrstuvwxyz" + upperChars + numberChars + specialChars;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            // Ensure at least one uppercase letter, one number, and one special character
            password.Append(upperChars[random.Next(upperChars.Length)]);
            password.Append(numberChars[random.Next(numberChars.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Fill the rest of the password length with random characters
            while (password.Length < length)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }


    }
}
