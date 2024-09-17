using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Address;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserService _IUserService;
        public ApplicationUserController(IApplicationUserService userService)
        {
            _IUserService = userService;
        }

        [HttpGet("GetInformationUser/{ID}")]
        public async Task<ActionResult<UserDataVM>> GetInformationUser(string ID)
        {
            var user = await _IUserService.GetInformationByID(ID);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        [Route("GetAllInformationUserAsync")]
        public async Task<IActionResult> GetAllInformationUserAsync()
        {
            var objCollection = await _IUserService.GetAllInformationAsync();

            return Ok(objCollection);
        }
        [HttpPost]
        [Route("register_with_random_password")]
        public async Task<IActionResult> RegisterWithRandomPasswordAsync([FromBody] RegisterOnly registerOnly, string role)
        {

            var result = await _IUserService.RegisterWithRandomPasswordAsync(registerOnly, role);
            if (result.IsSuccess)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            else
            {
                return BadRequest("Có lỗi trong quá trình thực hiện.");
            }
        }
        [HttpPut]
        [Route("UpdateUser/{ID}")]
        public async Task<IActionResult> UpdateUser(string ID, [FromForm] UserUpdateVM userUpdateVM)
        {
            var result = await _IUserService.UpdateUserAsync(ID, userUpdateVM);
            if (!result)
            {
                return BadRequest("Cập nhật thông tin người dùng thất bại.");
            }
            return Ok("Cập nhật thông tin người dùng thành công.");
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        [Route("GetAllActiveInformationUserAsync")]
        public async Task<IActionResult> GetAllActiveInformationUserAsync()
        {
            var objCollection = await _IUserService.GetAllActiveInformationAsync();

            return Ok(objCollection);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var response = await _IUserService.Login(model);

            if (!response.IsSuccess)
            {
                return Unauthorized(response.Message);
            }

            return Ok(new
            {
                token = response.Token,
                role = response.Roles.FirstOrDefault()
            });
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterUser registerUser, string role)
        {
            registerUser.JoinDate = DateTime.Now;

            var result = await _IUserService.RegisterAsync(registerUser, role);
            if (result.IsSuccess)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            else
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpGet("ChangeStatus/{ID}")]
        public async Task<IActionResult> ChangeStatus(string ID)
        {
            var success = await _IUserService.SetStatus(Guid.Parse(ID));
            if (success == true)
            {
                return Ok(new { status = "Success", message = "Successfully." });
            }
            else
            {
                return BadRequest();
            }
        }
     
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("GetUsersByEmail")]
        public async Task<IActionResult> GetUsersByEmail([FromQuery] string email)
        {
            var users = await _IUserService.GetUsersByEmailAsync(email);
            return Ok(users);
        }

        [HttpGet("GetUsersByPhoneNumber")]
        public async Task<IActionResult> GetUsersByPhoneNumber([FromQuery] string phoneNumber)
        {
            var users = await _IUserService.GetUsersByPhoneNumberAsync(phoneNumber);
            return Ok(users);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("GetUsersByStatus")]
        public async Task<IActionResult> GetUsersByStatus([FromQuery] int status)
        {
            var users = await _IUserService.GetUsersByStatusAsync(status);
            return Ok(users);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("GetUsersByName")]
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            var users = await _IUserService.GetUsersByNameAsync(name);
            return Ok(users);
        }
        //[AllowAnonymous]
        //[HttpGet("get-province")]
        //public async Task<List<RegisterUser>> GetProvince()
        //{
        //    string requestURL = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province";
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("token", "55a7ce93-3111-11ef-8e53-0a00184fe694");
        //    var response = await httpClient.GetAsync(requestURL);

        //    var provinces = new List<RegisterUser>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        var provincesArray = JsonDocument.Parse(responseData)
        //                                                .RootElement
        //                                                .GetProperty("data")
        //                                                .EnumerateArray();
        //        foreach (var province in provincesArray)
        //        {
        //            provinces.Add(new RegisterUser
        //            {
        //                CityID = province.GetProperty("ProvinceID").GetInt32(),
        //                City = province.GetProperty("ProvinceName").GetString()!
        //            });
        //        }
        //    }
        //    else
        //    {
        //        var errorMessage = await response.Content.ReadAsStringAsync();
        //        // Log the error message or inspect it for further details
        //    }
        //    return provinces;
        //}
        //[AllowAnonymous]
        //[HttpGet("get-district")]
        //public async Task<List<RegisterUser>> GetDistrict(int cityID)
        //{
        //    string requestURL = $"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id={cityID}";
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("token", "55a7ce93-3111-11ef-8e53-0a00184fe694");
        //    var response = await httpClient.GetAsync(requestURL);

        //    var district = new List<RegisterUser>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        var document = JsonDocument.Parse(responseData);
        //        var rootElement = document.RootElement;

        //        if (rootElement.TryGetProperty("data", out var districtArray))
        //        {
        //            foreach (var dist in districtArray.EnumerateArray())
        //            {
        //                var address = new RegisterUser
        //                {
        //                    CityID = dist.TryGetProperty("ProvinceID", out var provinceID) ? provinceID.GetInt32() : 0,
        //                    City = dist.TryGetProperty("ProvinceName", out var provinceName) ? provinceName.GetString() ?? string.Empty : string.Empty,
        //                    DistrictID = dist.TryGetProperty("DistrictID", out var districtID) ? districtID.GetInt32() : 0,
        //                    DistrictCounty = dist.TryGetProperty("DistrictName", out var districtName) ? districtName.GetString() ?? string.Empty : string.Empty
        //                };
        //                district.Add(address);
        //            }
        //        }
        //        else
        //        {
        //            // Log or handle the case where "data" is missing
        //        }
        //    }
        //    else
        //    {
        //        var errorMessage = await response.Content.ReadAsStringAsync();
        //        // Log the error message or inspect it for further details
        //    }
        //    return district;
        //}
        //[AllowAnonymous]
        //[HttpGet("get-ward")]
        //public async Task<List<RegisterUser>> GetWard(int city_districtID)
        //{
        //    string requestURL = $"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={city_districtID}";
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("token", "55a7ce93-3111-11ef-8e53-0a00184fe694");
        //    var response = await httpClient.GetAsync(requestURL);
        //    var ward = new List<RegisterUser>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        var document = JsonDocument.Parse(responseData);
        //        var rootElement = document.RootElement;

        //        if (rootElement.TryGetProperty("data", out var wardArray))
        //        {
        //            foreach (var warditem in wardArray.EnumerateArray())
        //            {
        //                var address = new RegisterUser
        //                {
        //                    AddressCreateVM.CityID = warditem.TryGetProperty("ProvinceID", out var provinceID) ? provinceID.GetInt32() : 0,
        //                    City = warditem.TryGetProperty("ProvinceName", out var provinceName) ? provinceName.GetString() ?? string.Empty : string.Empty,
        //                    DistrictID = warditem.TryGetProperty("DistrictID", out var districtID) ? districtID.GetInt32() : 0,
        //                    DistrictCounty = warditem.TryGetProperty("DistrictName", out var districtName) ? districtName.GetString() ?? string.Empty : string.Empty,
        //                    CommuneCode = warditem.TryGetProperty("WardCode", out var wardID) ? wardID.GetString() ?? string.Empty : string.Empty,
        //                    Commune = warditem.TryGetProperty("WardName", out var wardName) ? wardName.GetString() ?? string.Empty : string.Empty

        //                };
        //                ward.Add(address);
        //            }
        //        }
        //        else
        //        {
        //            // Log or handle the case where "data" is missing
        //        }
        //    }
        //    else
        //    {
        //        var errorMessage = await response.Content.ReadAsStringAsync();
        //        // Log the error message or inspect it for further details
        //    }
        //    return ward;
        //}
    }
}
