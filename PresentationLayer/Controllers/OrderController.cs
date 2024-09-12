using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public OrderController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet("order_user")]
        public async Task<IActionResult> Order()
        {
            return View();
        }

        [HttpGet("order_update_user/{ID}")]
        public async Task<IActionResult> OrderUpdate(Guid ID)
        {
            return View();
        }

    }
}
