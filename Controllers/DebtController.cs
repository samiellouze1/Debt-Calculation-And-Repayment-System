using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.EntityFrameworkCore;
using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtController : Controller
    {
        private readonly IDEBTService _debtService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public DebtController(IDEBTService debtService, ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService)
        {
            _debtService = debtService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
        }
        #region getters
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllDebts()
        {
            var DEBTs = _debtService.GetAllAsync().Result;
            return View(DEBTs);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyDebtsStudent()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myDEBTs = student.Debts.ToList();
            return View(myDEBTs);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyDebtsStaffMember()
        {
            var staffId = User.FindFirstValue("Id");
            var staff = _staffmemberService.GetByIdAsync(staffId).Result;
            var myStudents = staff.Students.ToList();
            var mydebts = new List<DEBT>();
            foreach (var std in myStudents)
            {
                mydebts.AddRange(std.Debts.ToList());
            }
            return View(mydebts);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DebtsByStudentAdmin(string id)
        {
            var student = _studentService.GetByIdAsync(id).Result;
            var Debts = student.Debts.ToList();
            return View(Debts);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> DebtsByStudentStaffMember(string id)
        {
            var student = _studentService.GetByIdAsync(id).Result;
            var DEBTs = student.Debts;
            return View(DEBTs);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DebtsByStaffMemberAdmin(string id)
        {
            var staff = _staffmemberService.GetByIdAsync(id).Result;
            var mystudents = staff.Students.ToList();
            var myDebts = new List<DEBT>();
            foreach (var std in mystudents)
            {
                myDebts.AddRange(std.Debts);
            }
            return View(myDebts);
        }
        #endregion

        #region create debt
        [Authorize(Roles = "Admin, StaffMember")]
        public IActionResult CreateDebt()
        {
            var staffid = User.FindFirstValue("Id");
            var vmstudents = _staffmemberService.GetByIdAsync(staffid).Result.Students.ToList();
            var vm = new CreateDebtVM() 
            { 
                Students = vmstudents
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> CreateDebt(CreateDebtVM debtVM)
        {
            var newdebt = new DEBT()
            {
                InitialAmount = debtVM.InitialAmount,
                InterestRate = debtVM.InterestRate,
                StartDate = debtVM.StartDate,
                RegDate = DateTime.Now,
                Paid = debtVM.Paid,
                StudentId = debtVM.StudentId,
            };
            try
            {
                await _debtService.AddAsync(newdebt);
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine(message);
                throw;
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
