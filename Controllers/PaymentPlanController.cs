using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Debt_Calculation_And_RepaymenPlan_System.Controllers
{
    public class PaymenPlanController : Controller
    {
        private readonly IPAYMENTPLANService _paymenPlanService;
        private readonly ISTUDENTService _studentService;
        public PaymenPlanController(IPAYMENTPLANService scolarshipDebtService, ISTUDENTService studentService)
        {
            _paymenPlanService = scolarshipDebtService;
            _studentService = studentService;
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyScolarshipDebts(int id)
        {
            string studentId = User.Identity.Name;
            var student = _studentService.GetByIdAsync(studentId).Result;
            var mypaymenPlans = student.ScolarshipDebts.ToList();
            return View(mypaymenPlans);
        }
    }
}
