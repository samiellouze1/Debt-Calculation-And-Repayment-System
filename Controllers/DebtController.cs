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
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public DebtController(IDEBTService debtService, ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService,IDEBTREGISTERService debtregisterService)
        {
            _debtService = debtService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
        }
        #region getters
        public async Task<IActionResult> DebtsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id);
            var debts = debtregister.Debts;
            return View("Debts",debts);
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
