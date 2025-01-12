﻿using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static VoucherStatusUpdateService;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]
    [Route("home")]
    public class VoucherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public VoucherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("voucher/index")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://localhost:7241/api/VoucherM/getall";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var vouchers = JsonConvert.DeserializeObject<List<GetAllVoucherVM>>(jsonResponse);
                return View(vouchers);
            }
            else
            {
                
                return View(new List<GetAllVoucherVM>());
            }
        }


        [HttpGet("voucher/Create")]
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://localhost:7241/api/VoucherM/getallClient";
            var response = await client.GetAsync(url);

            List<UserVM> users = new List<UserVM>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UserVM>>(content);
            }
            ViewBag.Users = users;

            var model = new CreateVoucherVM();
            return View(model);
        }
        [HttpPost("voucher/create")]
        public async Task<IActionResult> Create(CreateVoucherVM model)
        {


            // Lấy ID người dùng từ cookie hoặc Claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin người dùng.");
                return View(model);
            }

            // Gán CreateBy là userId
            model.CreateBy = userId;

            var client = _httpClientFactory.CreateClient();

            var checkUrl = $"https://localhost:7241/api/VoucherM/CheckVoucherCodeExists?code={model.Code}";
            var checkResponse = await client.GetAsync(checkUrl);

            if (!checkResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi kiểm tra mã voucher.");
                return View(model);
            }

            var checkContent = await checkResponse.Content.ReadAsStringAsync();
            var checkResult = JsonConvert.DeserializeObject<CheckVoucherCodeResponse>(checkContent);

            if (checkResult.Exists)
            {
                var usersResponse = await client.GetAsync("https://localhost:7241/api/VoucherM/getallClient");
                List<UserVM> users = new List<UserVM>();
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserVM>>(usersContent);
                }
                ViewBag.Users = users;
                ModelState.AddModelError("Code", "Mã voucher đã tồn tại.");
                return View(model);
            }

            var url = "https://localhost:7241/api/VoucherM/create";
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Tạo voucher thành công!";
                return RedirectToAction("Index");
            }
            else
            {

                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error: {errorMessage}";
                //ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");

                // Khởi tạo lại ViewBag.Users khi trả về View
                var usersResponse = await client.GetAsync("https://localhost:7241/api/VoucherM/getallClient");
                List<UserVM> users = new List<UserVM>();
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserVM>>(usersContent);
                }
                ViewBag.Users = users;
            }

            return View(model);
        }


        [HttpGet("voucher/Update/{id}")]
        public async Task <IActionResult> Update(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7241/api/VoucherM/GetById/{id}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var voucherUpdateVM = JsonConvert.DeserializeObject<UpdateVoucherVM>(jsonResponse);
                return View(voucherUpdateVM);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost("voucher/Update/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateVoucherVM voucherUpdateVM)
        {
          
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7241/api/VoucherM/EditVoucher/{id}";

            var checkUrl = $"https://localhost:7241/api/VoucherM/CheckVoucherCodeExists?code={voucherUpdateVM.Code}";
            var checkResponse = await client.GetAsync(checkUrl);

            if (!checkResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi kiểm tra mã voucher.");
                return View(voucherUpdateVM);
            }

            var checkContent = await checkResponse.Content.ReadAsStringAsync();
            var checkResult = JsonConvert.DeserializeObject<CheckVoucherCodeResponse>(checkContent);

            if (checkResult.Exists)
            {
                var usersResponse = await client.GetAsync("https://localhost:7241/api/VoucherM/getallClient");
                List<UserVM> users = new List<UserVM>();
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserVM>>(usersContent);
                }
                ViewBag.Users = users;
                ModelState.AddModelError("Code", "Mã voucher đã tồn tại.");
                return View(voucherUpdateVM);
            }

            var json = JsonConvert.SerializeObject(voucherUpdateVM);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật voucher thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error: {errorMessage}";
                //ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");
                return View(voucherUpdateVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                TempData["ErrorMessage"] = $"Error: không thể lấy thông tin người dùng";
                //ModelState.AddModelError(string.Empty, "Không thể lấy thông tin người dùng.");
                return View("Index");
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7241/api/VoucherM/Delete/{id}/{userId}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa voucher thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error: {errorMessage}";
                return View("Index");
            }
        }
        public async Task<bool> UpdateVoucherStatusAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync("https://localhost:7241/api/VoucherM/update-status", null);
            return response.IsSuccessStatusCode;
        }


        [HttpGet("voucher/Detail/{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7241/api/VoucherM/GetById/{id}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var voucherUpdateVM = JsonConvert.DeserializeObject<GetAllVoucherVM>(jsonResponse);
                TempData["SuccessMessage"] = "Xóa voucher thành công!";
                return View(voucherUpdateVM);
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa voucher không thành công!";
                return RedirectToAction("Index");
            }
        }
        [HttpGet("voucher/search")]
        public async Task<IActionResult> SearchVouchers([FromQuery] string input)
        {
            var client = _httpClientFactory.CreateClient();

            if (string.IsNullOrEmpty(input))
            {
                // Nếu input là null hoặc trống, lấy danh sách tất cả voucher
                var url = "https://localhost:7241/api/VoucherM/getall"; 
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vouchers = JsonConvert.DeserializeObject<List<GetAllVoucherVM>>(jsonResponse);
                    return View("Index", vouchers); 
                }
                else
                {
                    // Xử lý lỗi nếu cần
                    return View("Index"); // Hoặc một trang lỗi phù hợp
                }
            }
            else
            {
                // Nếu input có giá trị, tìm kiếm theo mã hoặc tên
                var url = $"https://localhost:7241/api/VoucherM/search?input={input}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vouchers = JsonConvert.DeserializeObject<List<GetAllVoucherVM>>(jsonResponse);
                    return View("Index", vouchers); // Hiển thị kết quả tìm kiếm trên trang Index
                }
                else
                {
                    // Xử lý lỗi nếu cần
                    return View("Index"); // Hoặc một trang lỗi phù hợp
                }
            }
        }



        [HttpGet("voucher/UpdateVoucherUser/{id}")]
        public async Task<IActionResult> UpdateVoucherUser(Guid id)
        {
            var client = _httpClientFactory.CreateClient();

            // Fetch voucher details
            var voucherUrl = $"https://localhost:7241/api/VoucherM/GetById/{id}";
            var voucherResponse = await client.GetAsync(voucherUrl);
            if (!voucherResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var voucherContent = await voucherResponse.Content.ReadAsStringAsync();
            var voucherUpdateVM = JsonConvert.DeserializeObject<UpdateVC>(voucherContent);

            // Kiểm tra xem voucherUpdateVM có null không
            if (voucherUpdateVM == null)
            {
                return RedirectToAction("Index");
            }

            // Fetch all users
            var usersUrl = "https://localhost:7241/api/VoucherM/getallClient";
            var usersResponse = await client.GetAsync(usersUrl);
            List<UserVM> users = new List<UserVM>();
            if (usersResponse.IsSuccessStatusCode)
            {
                var usersContent = await usersResponse.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UserVM>>(usersContent) ?? new List<UserVM>();
            }

            var voucherUsersUrl = $"https://localhost:7241/api/VoucherM/GetVoucherUsers/{id}";
            var voucherUsersResponse = await client.GetAsync(voucherUsersUrl);
            List<string> connectedUserIds = new List<string>();
            if (voucherUsersResponse.IsSuccessStatusCode)
            {
                var voucherUsersContent = await voucherUsersResponse.Content.ReadAsStringAsync();
                connectedUserIds = JsonConvert.DeserializeObject<List<string>>(voucherUsersContent) ?? new List<string>();
            }

            ViewBag.SelectedUserIds = connectedUserIds;

            ViewBag.Users = users;

            return View(voucherUpdateVM);
        }

        [HttpPost("voucher/UpdateVoucherUser/{id}")]
        public async Task<IActionResult> UpdateVoucherUser(Guid id, UpdateVC voucherUpdateVM)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin người dùng.");
                return View(voucherUpdateVM);
            }

            // Gán CreateBy là userId
            voucherUpdateVM.CreateBy = userId;
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7241/api/VoucherM/Edit_VoucherUser/{id}";

            var json = JsonConvert.SerializeObject(voucherUpdateVM);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã cập nhật voucher";
                return RedirectToAction("Index");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error: {errorMessage}";

                // Reload data if update fails
                var usersUrl = "https://localhost:7241/api/VoucherM/getallClient";
                var usersResponse = await client.GetAsync(usersUrl);
                List<UserVM> users = new List<UserVM>();
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserVM>>(usersContent) ?? new List<UserVM>();
                }

                // Fetch selected user IDs
                var voucherUsersUrl = $"https://localhost:7241/api/VoucherM/GetVoucherUsers/{id}";
                var voucherUsersResponse = await client.GetAsync(voucherUsersUrl);
                List<string> connectedUserIds = new List<string>();
                if (voucherUsersResponse.IsSuccessStatusCode)
                {
                    var voucherUsersContent = await voucherUsersResponse.Content.ReadAsStringAsync();
                    connectedUserIds = JsonConvert.DeserializeObject<List<string>>(voucherUsersContent) ?? new List<string>();
                }

                ViewBag.SelectedUserIds = connectedUserIds;
                ViewBag.Users = users;

                return View(voucherUpdateVM);
            }
        }
    }
}
