using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPAYMENTService _paymentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public PaymentController(IPAYMENTService paymentService, ISTAFFMEMBERService staffmemberService)
        {
            _paymentService = paymentService;
            _staffmemberService = staffmemberService;
        }
        public async Task<IActionResult> PaymentPlansperPayment(string id)
        {
            var payment = _paymentService.GetByIdAsync(id).Result;
            var paymentplans = payment.PaymentPlans;
            return View(paymentplans);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllPayments()
        {
            var payments = _paymentService.GetAllAsync().Result;
            return View(payments);
        }
        public void GeneratePaymentPlansDefault()
        {
        }
        public async Task<IActionResult> StaffPayments()
        {
            var staffId = User.FindFirstValue("Id");
            var students = _staffmemberService.GetByIdAsync(staffId).Result.Students;
            var payments = new List<PAYMENT>();
            foreach (var std in students)
            {
                foreach (var sd in std.ScolarshipDebts)
                {
                    payments.AddRange(sd.Payments);
                }
            }
            return View(payments);
        }
    }
}
