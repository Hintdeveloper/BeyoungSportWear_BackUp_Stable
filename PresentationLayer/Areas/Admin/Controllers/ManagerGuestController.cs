using BusinessLogicLayer.Viewmodels.ApplicationUser;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using PresentationLayer.Areas.Admin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("admin")]
    public class ManagerGuestController : Controller
    {
        [HttpGet("home/guest")]
        public async Task<IActionResult> Index(string searchQuery)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    List<UserDataVM> users = null;

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        searchQuery = searchQuery.Trim();  // Loại bỏ khoảng trắng thừa ở đầu và cuối

                        // Nếu là số điện thoại (chính xác 10 số)
                        if (Regex.IsMatch(searchQuery, @"^0\d{9}$"))
                        {
                            var result = await GetUserByPhoneNumber(searchQuery);
                            if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                            {
                                users = userList;
                            }
                        }
                        // Nếu là email (chứa ký tự @)
                        else if (searchQuery.Contains('@'))
                        {
                            var result = await GetUserByEmail(searchQuery);
                            if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                            {
                                users = userList;
                            }
                        }
                        // Ngược lại, tìm theo tên
                        else
                        {
                            var result = await GetUserByName(searchQuery);
                            if (result is ViewResult viewResult && viewResult.Model is List<UserDataVM> userList)
                            {
                                users = userList;
                            }
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

                    // Lọc người dùng có vai trò là Client
                    if (users != null)
                    {
                        users = users.Where(u => u.RoleName == "Client").ToList();
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
        [Authorize(Roles = "Admin")]

        [HttpGet("home/guest/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("home/guest/create")]
        public async Task<IActionResult> Create(RegisterUser registerUser, string role)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    role = "Client";

                    registerUser.Password = GenerateRandomPassword(10);
                    registerUser.ConfirmPassword = registerUser.Password;
                    string requestURL = $"https://localhost:7241/api/ApplicationUser/Register?role={role}";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken); // Use jwtToken directly

                    // Tạo MultipartFormDataContent để gửi dữ liệu dạng form
                    var formData = new MultipartFormDataContent();

                    // Thêm các trường dữ liệu vào form-data
                    formData.Add(new StringContent(registerUser.FirstAndLastName ?? string.Empty), nameof(registerUser.FirstAndLastName));
                    formData.Add(new StringContent(registerUser.Username ?? string.Empty), nameof(registerUser.Username));
                    formData.Add(new StringContent(registerUser.Password ?? string.Empty), nameof(registerUser.Password));
                    formData.Add(new StringContent(registerUser.ConfirmPassword ?? string.Empty), nameof(registerUser.ConfirmPassword));
                    formData.Add(new StringContent(registerUser.Email ?? string.Empty), nameof(registerUser.Email));
                    formData.Add(new StringContent(registerUser.PhoneNumber ?? string.Empty), nameof(registerUser.PhoneNumber));
                    formData.Add(new StringContent(registerUser.Gender.ToString() ?? string.Empty), nameof(registerUser.Gender));
                    formData.Add(new StringContent(registerUser.DateOfBirth.ToString("yyyy-MM-dd") ?? string.Empty), nameof(registerUser.DateOfBirth));

                    // Thêm hình ảnh nếu có
                    if (registerUser.Images != null)
                    {
                        var stream = registerUser.Images.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(registerUser.Images.ContentType);
                        formData.Add(fileContent, nameof(registerUser.Images), registerUser.Images.FileName);
                    }

                    // Thêm địa chỉ nếu có
                    formData.Add(new StringContent(registerUser.SpecificAddress ?? string.Empty), "SpecificAddress");
                    formData.Add(new StringContent(registerUser.City ?? string.Empty), "City");
                    formData.Add(new StringContent(registerUser.DistrictCounty ?? string.Empty), "DistrictCounty");
                    formData.Add(new StringContent(registerUser.Commune ?? string.Empty), "Commune");

                    var response = await httpClient.PostAsync(requestURL, formData);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("User's Password: " + registerUser.Password);
                        //return RedirectToAction("Index");
                        return Json(new { isSuccess = true });
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        //return View(registerUser);
                        return Json(new { isSuccess = false, errorMessage = errorMessage });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized();
        }

        [HttpGet("home/guest/details/{ID}")]
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
        [Authorize(Roles = "Admin")]

        [HttpGet("home/guest/edit/{ID}")]
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
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpClient.GetAsync(requestURL);
                    string apiData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var userDTO = JsonConvert.DeserializeObject<UserUpdateDTO>(apiData);
                        if (userDTO != null)
                        {
                            var userVM = new UserUpdateVM
                            {
                                ModifiedBy = userDTO.ModifiedBy,
                                FirstAndLastName = userDTO.FirstAndLastName,
                                Email = userDTO.Email,
                                PhoneNumber = userDTO.PhoneNumber,
                                Gender = userDTO.Gender,
                                DateOfBirth = userDTO.DateOfBirth,
                                AddressUpdateVM = userDTO.AddressUpdateVM,
                                // Không gán trực tiếp Images từ URL vào IFormFile, giữ nó null cho phần edit.
                            };

                            return View(userVM);
                        }
                    }
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            return Unauthorized();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("home/guest/edit/{ID}")]
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

        [HttpGet("home/guest/changestatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(Guid ID)
        {
            if (HttpContext.Request.Cookies.TryGetValue("jwt", out string jwtToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    string requestURL = $"https://localhost:7241/api/ApplicationUser/ChangeStatus/{ID}";
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

        public async Task<IFormFile> DownloadFileAsIFormFileAsync(string url, string fileName)
        {
            using (var httpClient = new HttpClient())
            {
                var fileBytes = await httpClient.GetByteArrayAsync(url);

                var stream = new MemoryStream(fileBytes);
                var formFile = new FormFile(stream, 0, fileBytes.Length, null, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };

                return formFile;
            }
        }
    }
}
