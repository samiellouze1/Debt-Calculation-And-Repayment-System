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
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var includeProperties = new Expression<Func<STAFFMEMBER, object>>[]
                {
                        x => x.Students.Select(s => s.DebtRegister.Debts)
                };
                var staffuser = await _staffmemberService.GetByIdAsync(userid, includeProperties);
                authorize = staffuser.Students.Select(s => s.DebtRegister).SelectMany(dr => dr.Debts).Select(r => r.Id).ToList().Contains(id);
            }
            else if (User.IsInRole("Student"))
            {
                var userid = User.FindFirstValue("Id");
                var includeProperties = new Expression<Func<STUDENT, object>>[]
                {
                        x => x.DebtRegister.Requests
                };
                var student = await _studentService.GetByIdAsync(userid, includeProperties);
                authorize = student.DebtRegister.Requests.Select(r => r.Id).ToList().Contains(id);
            }
            if (authorize)
            {
                var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
                var debts = debtregister.Debts;
                return View("Debts", debts);
            }
            else
            {
                ViewData["Error"] = "You tried to enter a page to which you are not allowed";
                return RedirectToAction("Error", "Home");
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
                RegDate = DateTime.Now,
                DebtRegister=debtregister,
                EndDate=debtVM.EndDate,
            };
            await _debtService.AddAsync(newdebt);
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
