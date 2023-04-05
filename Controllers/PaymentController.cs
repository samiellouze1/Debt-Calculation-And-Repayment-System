using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPAYMENTService _paymentService;
        private readonly ISTUDENTService _studentService;
        public PaymentController(IPAYMENTService scolarshipDebtService, ISTUDENTService studentService)
        {
            _paymentService = scolarshipDebtService;
            _studentService = studentService;
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyScolarshipDebts(int id)
        {
            string studentId = User.Identity.Name;
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            return View(myscolarshipdebts);
        }
    }
}
