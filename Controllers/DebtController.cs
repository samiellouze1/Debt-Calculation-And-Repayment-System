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
            var Debts = await _debtService.GetAllAsync();
            return View(Debts);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyDebtsStudent()
        {
            var studentId = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentId);
            var myDEBTs = student.DebtRegister.Debts.ToList();
            return View(myDEBTs);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyDebtsStaffMember()
        {
            var staffId = User.FindFirstValue("Id");
            var staff = await _staffmemberService.GetByIdAsync(staffId);
            var myStudents = staff.Students.ToList();
            var mydebts = new List<DEBT>();
            mydebts.AddRange(myStudents.Select(s=>s.DebtRegister).SelectMany(s => s.Debts).ToList());
            return View(mydebts);
        }

        public async Task<IActionResult> DebtsByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var Debts = student.DebtRegister.Debts;
            return View(Debts);
        }
        public async Task<IActionResult> DebtsByStaffMember(string id)
        {
            var staff = await _staffmemberService.GetByIdAsync(id);
            var mystudents = staff.Students;
            var myDebts = new List<DEBT>();
            myDebts.AddRange(mystudents.Select(s=>s.DebtRegister).SelectMany(s=>s.Debts).ToList());
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
                //InitialAmount = debtVM.InitialAmount,
                //InterestRate = debtVM.InterestRate,
                //StartDate = debtVM.StartDate,
                //RegDate = DateTime.Now,
                //Paid = debtVM.Paid,
                //StudentId = debtVM.StudentId,
            };
            await _debtService.AddAsync(newdebt);
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
