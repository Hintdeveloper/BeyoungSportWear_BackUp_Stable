using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]

    public class ManagerVoucherController : Controller
    {
        [HttpGet("home/index_voucher")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet("managercreate_voucher")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpGet("managerupdate_voucher/{ID}")]
        public async Task<IActionResult> Update(Guid ID)
        {
            return View();
        }
    }
}
