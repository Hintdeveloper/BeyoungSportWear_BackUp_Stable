using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [Route("/account")]
        public async Task<IActionResult> Account()
        {
            return View();
        }
    }
}
