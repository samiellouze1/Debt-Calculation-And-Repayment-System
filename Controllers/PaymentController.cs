﻿using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
