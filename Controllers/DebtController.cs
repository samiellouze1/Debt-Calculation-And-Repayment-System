using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtController : Controller
    {
        private readonly ISCOLARSHIPDEBTService _scolarshipDebtService;
        private readonly ISTUDENTService _studentService;
        public DebtController(ISCOLARSHIPDEBTService scolarshipDebtService, ISTUDENTService studentService)
        {
            _scolarshipDebtService = scolarshipDebtService;
            _studentService = studentService;
        }
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyScolarshipDebts(int id)
        {
            var studentId = User.FindFirstValue("Id");
            Console.WriteLine("this");
            Console.WriteLine(studentId);
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myscolarshipdebts = student.ScolarshipDebts.ToList();
            return View(myscolarshipdebts);
        }
    }
}
