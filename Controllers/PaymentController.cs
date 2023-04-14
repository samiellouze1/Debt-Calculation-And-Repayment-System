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
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTUDENTService _studentService;
        public PaymentController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService)
        {
            _studentService = studentService;
            _debtregisterService = debtregisterService;
        }
        public async Task<IActionResult> PaymentsByDebtRegister(string id)
        {
            var student = await _debtregisterService.GetByIdAsync(id);
            var payments = student.Payments;
            return View("Payments", payments);
        }
    }
}
