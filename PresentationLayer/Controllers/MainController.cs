using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public MainController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("contact_hotline")]
        public async Task<IActionResult> Contact()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Route("details_product/{id}")]
        public async Task<IActionResult> Product()
        {
            return View();
        }
        [HttpGet]
        [Route("cart_index")]
        public async Task<IActionResult> Cart()
        {
            return View();
        }

    }
}
