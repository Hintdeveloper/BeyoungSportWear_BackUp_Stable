using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]
    public class ManagerProductDetailsController : Controller
    {
      
        [HttpGet("home/index_productdetails")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("managercreate_productdetails")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("managerupdate_productdetails/{ID}")]
        public async Task<IActionResult> Update(Guid ID)
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("managerupdate_productdetails/{ID}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("managerupdate_productdetails_ver1/{ID}")]
        public async Task<IActionResult> Update_Ver_I(Guid ID)
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("managerupdate_productdetails_ver1/{ID}")]
        public async Task<IActionResult> Update_Ver_I()
        {
            return View();
        }
    }
}
