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
        public async Task<IActionResult> DebtRegisterByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
    }
}
