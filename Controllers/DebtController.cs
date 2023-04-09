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
        private readonly IDEBTService _DEBTService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public DebtController(IDEBTService DEBTService, ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService)
        {
            _DEBTService = DEBTService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
        }
        public async Task<IActionResult> AllDebts()
        {
            var DEBTs = _DEBTService.GetAllAsync().Result;
            return View(DEBTs);
        }
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyDebts()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myDEBTs = student.Debts.ToList();
            return View(myDEBTs);
        }
        public async Task<IActionResult> PaymentPlanssperDEBT(string id)
        {
            var Debt=_DEBTService.GetByIdAsync(id).Result;
            var paymentplans = Debt.PaymentPlans;
            return View(paymentplans);
        }
        public async Task<IActionResult> DebtsperStaffMember(string id)
        {
            var staff = _staffmemberService.GetByIdAsync(id).Result;
            var mystudents = staff.Students.ToList();
            var myDebts = new List<DEBT>();
            foreach (var std in mystudents )
            {
                myDebts.AddRange(std.Debts);
            }
            return View(myDebts);
        }
        public async Task<IActionResult> DebtsperLoggedInStaffMember()
        {
            var staffid = User.FindFirstValue("Id");
            var staff = _staffmemberService.GetByIdAsync(staffid).Result;
            var mystudents = staff.Students.ToList();
            var myDebts = new List<DEBT>();
            foreach (var std in mystudents)
            {
                myDebts.AddRange(std.Debts);
            }
            return View(myDebts);
        }
        public async Task<IActionResult> DebtsByStudentId(string id)
        {
            var student= _studentService.GetByIdAsync(id).Result;
            var DEBTs = student.Debts;
            return View(DEBTs);
        }
    }
}
