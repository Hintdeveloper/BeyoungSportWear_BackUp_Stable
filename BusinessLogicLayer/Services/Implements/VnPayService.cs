using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.VNPay;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDBContext _dbContext;
		public VnPayService(IConfiguration configuration, ApplicationDBContext applicationDBContext)
		{
			_configuration = configuration;
			_dbContext = applicationDBContext;
		}
        public string CreatePaymentUrl(OrderCreateVM model, HttpContext context)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model không thể null.");
            }

            if (string.IsNullOrEmpty(model.HexCode))
            {
                throw new ArgumentException("HexCode không thể rỗng.", nameof(model.HexCode));
            }

            if (model.TotalAmount <= 0)
            {
                throw new ArgumentException("Tổng số tiền phải lớn hơn 0.", nameof(model.TotalAmount));
            }

            var timeZoneId = "SE Asia Standard Time";
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            if (model.OrderDetailsCreateVM != null && model.OrderDetailsCreateVM.Any())
            {
                foreach (var orderVariant in model.OrderDetailsCreateVM)
                {
                    if (orderVariant.IDOrder != model.ID)
                    {
                        orderVariant.IDOrder = model.ID;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Chi tiết đơn hàng không thể rỗng.", nameof(model.OrderDetailsCreateVM));
            }

            if (string.IsNullOrEmpty(_configuration["Vnpay:Version"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:Command"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:TmnCode"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:CurrCode"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:Locale"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:BaseUrl"]) ||
                string.IsNullOrEmpty(_configuration["Vnpay:HashSecret"]))
            {
                throw new InvalidOperationException("Cấu hình VNPAY không hợp lệ.");
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

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }
        public async Task<int> CheckInventory(Guid ID)
		{
			var options_check = await _dbContext.Options.Where(c => c.ID == ID).FirstOrDefaultAsync();
            if (options_check == null)
            {
                throw new Exception("Product not found.");
            }

            return options_check.StockQuantity;
        }
        public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
		{
			var pay = new VnPayLibrary();
			var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

			if (response != null)
			{
                var paymentResponse = new PaymentResponseModel
                {
                    Token = response.IDUser,
                    OrderId = response.ID,
                    PaymentId = response.ShippingAddress,
                    VnPayResponseCode = response.ShippingAddressLine2,
                    TransactionId = response.HexCode.ToString(),
                    OrderDescription = response.CustomerEmail
                };
                var productIds = response.OrderDetailsCreateVM.Select(c => c.IDOptions).ToList();

                foreach (var productId in productIds)
                {
                    var stockQuantity = await CheckInventory(productId);
                    if (stockQuantity <= 0)
                    {
                        paymentResponse.VnPayResponseCode = "02"; 
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch thất bại do hết hàng.";
                        return paymentResponse; 
                    }
                }
                switch (response.ShippingAddressLine2)
                {
                    case "00":
                        paymentResponse.Success = PaymentStatus.Success;
                        paymentResponse.Message = "Giao dịch thành công!";
                        break;
                    case "01":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch chưa hoàn tất.";
                        break;
                    case "02":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch bị lỗi.";
                        break;
                    case "04":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng giao dịch chưa thành công ở VNPAY).";
                        break;
                    case "05":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "VNPAY đang xử lý giao dịch này (Giao dịch hoàn tiền).";
                        break;
                    case "06":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng.";
                        break;
                    case "07":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch bị nghi ngờ gian lận.";
                        break;
                    case "09":
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = "Giao dịch hoàn trả bị từ chối.";
                        break;
                    default:
                        paymentResponse.Success = PaymentStatus.Failure;
                        paymentResponse.Message = $"Mã lỗi giao dịch không xác định: {response.ShippingAddressLine2}";
                        break;
                }

                // Xử lý mã lỗi vnp_ResponseCode
                switch (paymentResponse.VnPayResponseCode)
                {
                    case "00":
                        paymentResponse.Message = "Giao dịch thành công.";
                        break;
                    case "07":
                        paymentResponse.Message = "Trừ tiền thành công. Giao dịch bị nghi ngờ.";
                        break;
                    case "09":
                        paymentResponse.Message = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ Internet Banking tại ngân hàng.";
                        break;
                    case "10":
                        paymentResponse.Message = "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần.";
                        break;
                    case "11":
                        paymentResponse.Message = "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
                        break;
                    case "12":
                        paymentResponse.Message = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
                        break;
                    case "13":
                        paymentResponse.Message = "Giao dịch không thành công do: Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).";
                        break;
                    case "24":
                        paymentResponse.Message = "Giao dịch không thành công do: Khách hàng hủy giao dịch.";
                        break;
                    case "51":
                        paymentResponse.Message = "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                        break;
                    case "65":
                        paymentResponse.Message = "Giao dịch không thành công do: Tài khoản của quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                        break;
                    case "75":
                        paymentResponse.Message = "Ngân hàng thanh toán đang bảo trì.";
                        break;
                    case "79":
                        paymentResponse.Message = "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.";
                        break;
                    case "99":
                        paymentResponse.Message = "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê).";
                        break;
                    default:
                        paymentResponse.Message = $"Mã lỗi phản hồi không xác định: {paymentResponse.VnPayResponseCode}";
                        break;
                }

                return paymentResponse;
            }
			return null;
		}
        public async Task<bool> RequestRefundAsync(string transactionId, decimal amount, string orderInfo, string ipAddress, string transactionNo)
        {
            try
            {
                var refundUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
                var tmnCode = _configuration["Vnpay:TmnCode"];
                var hashSecret = _configuration["Vnpay:HashSecret"];

                var requestId = Guid.NewGuid().ToString("N");
                var version = "2.1.0";
                var transactionType = "03";
                var transactionDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
                var createDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
                if (string.IsNullOrEmpty(transactionNo))
                {
                    transactionNo = "00000000";
                }
                orderInfo = "Yeu cau hoan tien";

                var checksumData = $"{requestId}|{version}|refund|{tmnCode}|{transactionType}|{transactionId}|{amount * 100}|{transactionNo}|{transactionDate}|Merchant|{createDate}|{ipAddress}|{orderInfo}";
                var vnpSignature = GenerateSignature(checksumData, hashSecret);

                Console.WriteLine("Checksum Data: " + checksumData);
                Console.WriteLine("Generated Signature: " + vnpSignature);
                Console.WriteLine("hashSecret : " + hashSecret);

                var requestPayload = new
                {
                    vnp_RequestId = requestId,
                    vnp_Version = version,
                    vnp_Command = "refund",
                    vnp_TmnCode = tmnCode,
                    vnp_TransactionType = transactionType,
                    vnp_TxnRef = transactionId,
                    vnp_Amount = (amount * 100).ToString(),
                    vnp_OrderInfo = orderInfo,
                    vnp_TransactionNo = transactionNo,
                    vnp_TransactionDate = transactionDate,
                    vnp_CreateBy = "Merchant",
                    vnp_CreateDate = createDate,
                    vnp_IpAddr = ipAddress,
                    vnp_SecureHash = vnpSignature
                };

                using (var client = new HttpClient())
                {
                    var jsonContent = JsonConvert.SerializeObject(requestPayload);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Gửi yêu cầu POST tới API
                    var response = await client.PostAsync(refundUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

                        // In toàn bộ phản hồi từ API để kiểm tra
                        Console.WriteLine("Response Data: " + responseString);

                        if (responseData != null && responseData.ContainsKey("vnp_ResponseCode"))
                        {
                            // In chi tiết mã phản hồi và thông báo lỗi (nếu có)
                            Console.WriteLine($"Response Code: {responseData["vnp_ResponseCode"]}");
                            Console.WriteLine($"Response Message: {responseData["vnp_Message"]}");

                            if (responseData["vnp_ResponseCode"] == "00")
                            {
                                // Yêu cầu hoàn tiền thành công
                                return true;
                            }
                            else
                            {
                                // Thông báo lỗi chi tiết nếu yêu cầu thất bại
                                Console.WriteLine($"Refund failed: {responseData["vnp_Message"]}");
                                return false;
                            }
                        }
                        else
                        {
                            // Không có dữ liệu phản hồi hoặc mã phản hồi không tồn tại
                            Console.WriteLine("Error: No response data or vnp_ResponseCode not found.");
                            return false;
                        }
                    }
                    else
                    {
                        // Xử lý khi yêu cầu HTTP không thành công
                        Console.WriteLine($"HTTP Error: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ chi tiết hơn
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }
        private string GenerateSignature(string data, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
