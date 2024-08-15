using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.VNPay;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _configuration;
		public VnPayService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string CreatePaymentUrl(OrderCreateVM model, HttpContext context)
		{
			var timeZoneId = "SE Asia Standard Time";
			var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
			var pay = new VnPayLibrary();
			var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
			if (model.OrderDetailsCreateVM != null)
			{
				foreach (var orderVariant in model.OrderDetailsCreateVM)
				{
					orderVariant.IDOrder = model.ID;
				}
			}
			pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
			pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
			pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
			pay.AddRequestData("vnp_Amount", (model.TotalAmount * 100).ToString());
			pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
			pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
			pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
			pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
			pay.AddRequestData("vnp_OrderInfo", $"Người dùng {model.CustomerName} thanh toán đơn hàng giá trị: {Currency.FormatCurrency(model.TotalAmount.ToString())} vnđ");
			pay.AddRequestData("vnp_OrderType", model.ShippingAddress);
			pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
			pay.AddRequestData("vnp_TxnRef", model.HexCode.ToString());

			var paymentUrl =
				pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

			return paymentUrl;
		}

		public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
		{
			var pay = new VnPayLibrary();
			var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

			if (response != null)
			{
				var paymentResponse = new PaymentResponseModel();
				paymentResponse.Token = response.IDUser;
				paymentResponse.OrderId = response.ID;
				paymentResponse.PaymentId = response.ShippingAddress;
				paymentResponse.VnPayResponseCode = response.ShippingAddressLine2;
				paymentResponse.TransactionId = response.HexCode.ToString();
				paymentResponse.OrderDescription = response.CustomerEmail;
				paymentResponse.Success = PaymentStatus.Success;
				return paymentResponse;
			}
			return null;
		}
	}
}
