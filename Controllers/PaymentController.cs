using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPAYMENTService _paymentService;
        private readonly ISTUDENTService _studentService;
        public PaymentController(IPAYMENTService paymentService, ISTUDENTService studentService)
        {
            _paymentService = paymentService;
            _studentService = studentService;
        }
        public async Task<IActionResult> PaymentsByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var payments = student.Payments;
            return View("Payments", payments);
        }
    }
}
