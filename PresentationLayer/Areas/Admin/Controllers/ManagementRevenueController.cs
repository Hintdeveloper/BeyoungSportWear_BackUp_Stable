using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]

    public class ManagementRevenueController : Controller
    {
        [HttpGet("home/index_revenue")]

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
