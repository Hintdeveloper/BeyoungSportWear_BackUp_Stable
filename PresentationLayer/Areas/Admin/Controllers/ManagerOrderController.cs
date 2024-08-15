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
        [HttpGet("managercreate_order")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpGet("managerupdate_order/{ID}")]
        public async Task<IActionResult> Update(Guid ID)
        {
            return View();
        }
        [HttpGet]
        [Route("manager_orderstatus")]
        public async Task<IActionResult> Index_Ver_2()
        {
            return View();
        }
    }
}
