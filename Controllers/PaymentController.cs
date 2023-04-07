﻿using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPAYMENTService _paymentService;
        public PaymentController(IPAYMENTService paymentService)
        {
            _paymentService = paymentService;
        }
        public async Task<IActionResult> PaymentPlansperPayment ( string id)
        {
            var payment = _paymentService.GetByIdAsync(id).Result;
            var paymentplans = payment.PaymentPlans;
            return View(paymentplans);
        }
        public void GeneratePaymentPlansDefault()
        {
        }
    }
}
