using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]

    public class ManagerOrderController : Controller
    {
        [HttpGet("home/index_order")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("manager_orderstatus_verII/{ID}")]
        public async Task<IActionResult> Index_Ver_II(Guid ID)
        {
            return View();
        }

        [HttpGet]
        [Route("order_success")]
        public async Task<IActionResult> Order_Success()
        {
            return View();
        }
    }
}
