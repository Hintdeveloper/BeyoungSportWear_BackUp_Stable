using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Viewmodels.VNPay
{
	public class PaymentResponseModel
	{
		public string OrderDescription { get; set; }
		public string TransactionId { get; set; }
		public string TransactionStatus { get; set; }
		public Guid OrderId { get; set; }
		public string PaymentMethod { get; set; }
		public string PaymentId { get; set; }
		public PaymentStatus Success { get; set; }
		public string Message { get; set; }
		public string Token { get; set; }
		public string VnPayResponseCode { get; set; }
	}
}
