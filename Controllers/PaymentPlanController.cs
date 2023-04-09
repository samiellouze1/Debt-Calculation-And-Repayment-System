using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentPlanController : Controller
    {
        private readonly IPAYMENTPLANService _paymentplanService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        public PaymentPlanController(IPAYMENTPLANService paymentService, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService)
        {
            _paymentplanService = paymentService;
            _staffmemberService = staffmemberService;
            _studentService = studentService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllPaymentPlans()
        {
            var paymentplans = _paymentplanService.GetAllAsync().Result;
            return View(paymentplans);
        }
        public void GeneratePaymentPlansDefault()
        {
        }
        public async Task<IActionResult> MyPaymentPlans()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myDebts = student.Debts.ToList();
            var mypaymentplans = new List<PAYMENTPLAN>();
            foreach (var sd in myDebts)
            {
                mypaymentplans.AddRange(sd.PaymentPlans.ToList());
            }
            return View(mypaymentplans);
        }
        public async Task<IActionResult> StaffPaymentPlans()
        {
            var staffId = User.FindFirstValue("Id");
            var students = _staffmemberService.GetByIdAsync(staffId).Result.Students;
            var paymentplans = new List<PAYMENTPLAN>();
            foreach (var std in students)
            {
                foreach (var sd in std.Debts)
                {
                    paymentplans.AddRange(sd.PaymentPlans);
                }
            }
            return View(paymentplans);
        }
    }
}
