using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.EntityFrameworkCore;
using Debt_Calculation_And_Repayment_System.Data;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtController : Controller
    {
        private readonly ISCOLARSHIPDEBTService _scolarshipDebtService;
        private readonly ISTUDENTService _studentService;
        public DebtController(ISCOLARSHIPDEBTService scolarshipDebtService, ISTUDENTService studentService)
        {
            _scolarshipDebtService = scolarshipDebtService;
            _studentService = studentService;
        }
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyScolarshipDebts()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            return View(myscolarshipdebts);
        }
        public async Task<IActionResult> MyPayments()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            var mypayments = new List<PAYMENT>();
            foreach(var sd in myscolarshipdebts)
            {
                mypayments.AddRange(sd.Payments.ToList());
            }
            return View(mypayments);
        }
        public async Task<IActionResult> MyPaymentPlans()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            var mypaymentplans = new List<PAYMENTPLAN>();
            foreach(var sd in myscolarshipdebts)
            {
                foreach (var p in sd.Payments.ToList())
                {
                    mypaymentplans.AddRange(p.PaymentPlans.ToList());
                }
            }
            return View(mypaymentplans);
        }
        public async Task<IActionResult> PaymentsperScolarshipDebt(string id)
        {
            var scolarshipdebt=_scolarshipDebtService.GetByIdAsync(id).Result;
            var payments = scolarshipdebt.Payments;
            return View(payments);
        }
    }
}
