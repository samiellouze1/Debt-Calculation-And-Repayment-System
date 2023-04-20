using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public PaymentController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService)
        {
            _studentService = studentService;
            _debtregisterService = debtregisterService;
            _staffmemberService = staffmemberService;
        }

        public async Task<IActionResult> PaymentsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id,  dr => dr.Payments, dr => dr.Student);
            bool authorize = true;
            if (User.IsInRole(UserRoles.StaffMember))
            {
                var userid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(userid,sm=>sm.Students);
                authorize = staff.Students.Select(s => s.Id).ToList().Contains(debtregister.Student.Id);
            }
            else if (User.IsInRole(UserRoles.Student))
            {
                var userid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(userid);
                authorize = student.Id == debtregister.Student.Id;
            }
            if (authorize)
            {
                var payments = debtregister.Payments.OrderBy(p=>p.Type=="Full").ThenBy(p=>p.PaymentDate).ToList();
                return View("Payments", payments);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
            }
        }
    }
}
