using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class CheckOutController : Controller
    {
        [HttpGet("checkout_user")]
        public async Task<IActionResult> CheckOut()
        {
            return View();
        }
        public async Task<IActionResult> PaymentCallback()
        {
            return View();
        }
    }
}
