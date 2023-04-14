using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IDEBTREGISTERService _debtregisterService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService,ISTAFFMEMBERService staffmemberService,ISTUDENTService studentService)
        {
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllDebtRegisters()
        {
            var debtregisters = _debtregisterService.GetAllAsync();
            return View("DebtRegisters",debtregisters);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> MyDebtRegistersStaffMember()
        {
            var staffid = User.FindFirstValue("Id");
            var staffmember =await _staffmemberService.GetByIdAsync(staffid);
            var debtregisters = staffmember.Students.Select(s => s.DebtRegister);
            return View("DebtRegisters", debtregisters);
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> MyDebtRegisterStudent()
        {
            var studentid=User.FindFirstValue("Id");
            var student= await _studentService.GetByIdAsync(studentid);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
    }
}
