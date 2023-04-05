using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class ScolarshipDebtController : Controller
    {
        private readonly ISCOLARSHIPDEBTService _scolarshipDebtService;
        private readonly ISTUDENTService _studentService;
        public ScolarshipDebtController(ISCOLARSHIPDEBTService scolarshipDebtService, ISTUDENTService studentService)
        {
            _scolarshipDebtService = scolarshipDebtService;
            _studentService = studentService;
        }
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyScolarshipDebts(int id)
        {
            string studentId = User.Identity.Name;
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            return View(myscolarshipdebts);

        }
    }
}
