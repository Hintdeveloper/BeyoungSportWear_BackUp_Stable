﻿using BusinessLogicLayer.Viewmodels.Order;
using BusinessLogicLayer.Viewmodels.VNPay;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Interface
{
	public interface IVnPayService
	{
		public string CreatePaymentUrl(OrderCreateVM model, HttpContext context);
		public Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
	}

}
