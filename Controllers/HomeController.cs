﻿
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}