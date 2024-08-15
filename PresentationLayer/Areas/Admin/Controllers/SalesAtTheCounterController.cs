using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("admin")]

    public class SalesAtTheCounterController : Controller
    {
        [HttpGet("home/SalesAtTheCounter")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("viewshare_sale_at_the_counter")]
        public async Task<IActionResult> View_Share()
        {
            return View();
        }
    }
}
