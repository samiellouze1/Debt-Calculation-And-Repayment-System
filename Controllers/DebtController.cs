using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.EntityFrameworkCore;
using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using System.Linq.Expressions;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtController : Controller
    {
        private readonly IDEBTService _debtService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        public DebtController(IDEBTService debtService, ISTAFFMEMBERService staffmemberService,IDEBTREGISTERService debtregisterService,ISTUDENTService studentService)
        {
            _debtService = debtService;
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
        }
        #region getters
        public async Task<IActionResult> DebtsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Student, dr => dr.Debts);
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var staffuser = await _staffmemberService.GetByIdAsync(userid, sm=>sm.Students);
                authorize = staffuser.Students.Select(s => s.Id).ToList().Contains(debtregister.Student.Id);
            }
            else if (User.IsInRole("Student"))
            {
                var userid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(userid);
                authorize = debtregister.Student.Id==student.Id;
            }
            if (authorize)
            {
                var debts = debtregister.Debts;
                return View("Debts", debts);
            }
            else
            {
                var vm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
                return RedirectToAction("Error", "Home",vm);
            }
        }
        #endregion

        #region create debt
        [Authorize(Roles = "Admin, StaffMember")]
        public IActionResult CreateDebt()
        {
            var staffid = User.FindFirstValue("Id");
            var vmstudents = _staffmemberService.GetByIdAsync(staffid, sm => sm.Students).Result.Students.ToList();
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
            var student = await _studentService.GetByIdAsync(debtVM.StudentId,s=>s.DebtRegister);
            var debtregister = student.DebtRegister;
            var newdebt = new DEBT()
            {
                Amount = debtVM.Amount,
                StartDate = debtVM.StartDate,
                RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                DebtRegister=debtregister,
                EndDate=debtVM.EndDate,
            };
            await _debtService.AddAsync(newdebt);
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
