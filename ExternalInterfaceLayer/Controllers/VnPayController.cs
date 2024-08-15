using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalInterfaceLayer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VnPayController : ControllerBase
	{
		private readonly IVnPayService _vnPayService;

		public VnPayController(IVnPayService vnPayService)
		{
			_vnPayService = vnPayService;
		}

		[HttpPost("create-payment-url")]
		public IActionResult CreatePaymentUrl([FromBody] OrderCreateVM orderCreateVM)
		{
			if (orderCreateVM == null)
			{
				return BadRequest("Invalid order data.");
			}

			var paymentUrl = _vnPayService.CreatePaymentUrl(orderCreateVM, HttpContext);
			return Ok(new { Url = paymentUrl });
		}

		[HttpGet("payment-execute")]
		public async Task<IActionResult> PaymentExecute([FromQuery] IQueryCollection queryCollection)
		{
			var paymentResponse = await _vnPayService.PaymentExecute(queryCollection);

			if (paymentResponse != null)
			{
				return Ok(paymentResponse);
			}

			return BadRequest("Payment execution failed.");
		}

	}
}
