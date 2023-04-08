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
        private readonly ISTAFFMEMBERService _staffmemberService;
        public DebtController(ISCOLARSHIPDEBTService scolarshipDebtService, ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService)
        {
            _scolarshipDebtService = scolarshipDebtService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
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
        public async Task<IActionResult> AllScolarshipDebts()
        {
            var scolarshipdebts = _scolarshipDebtService.GetAllAsync().Result;
            return View(scolarshipdebts);
        }
        public async Task<IActionResult> ScolarshipDebtsperStaffMember()
        {
            var staffId = User.FindFirstValue("Id");
            var staff = _staffmemberService.GetByIdAsync(staffId).Result;
            var mystudents = staff.Students.ToList();
            var myscolarshipdebts = new List<SCOLARSHIPDEBT>();
            foreach (var std in mystudents )
            {
                myscolarshipdebts.AddRange(std.ScolarshipDebts);
            }
            return View(myscolarshipdebts);
        }
        public async Task<IActionResult> ScolarshipDebtsByStudentId( string id)
        {
            var student= _studentService.GetByIdAsync(id).Result;
            var scolarshipdebts = student.ScolarshipDebts;
            return View(scolarshipdebts);
        }
    }
}
