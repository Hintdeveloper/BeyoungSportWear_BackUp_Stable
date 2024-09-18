using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.VNPay;
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
        [HttpPost("request-refund")]
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.TransactionId) || model.Amount <= 0)
            {
                return BadRequest("Thông tin hoàn tiền không hợp lệ.");
            }

            // Thực hiện yêu cầu hoàn tiền
            var ipAddress = "192.168.0.102";
            var transactionNo = ""; // Cung cấp số giao dịch nếu cần
            var orderInfo = ""; // Cung cấp thông tin đơn hàng nếu cần

            var success = await _vnPayService.RequestRefundAsync(model.TransactionId, model.Amount, orderInfo, ipAddress, transactionNo);

            if (success)
            {
                return Ok(new { Message = "Yêu cầu hoàn tiền thành công." });
            }
            else
            {
                return StatusCode(500, "Yêu cầu hoàn tiền thất bại.");
            }
        }
    }
}
