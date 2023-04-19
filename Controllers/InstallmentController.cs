using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class InstallmentController : Controller
    {
        private readonly IINSTALLMENTService _installmentService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly IREQUESTService _requestService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        public InstallmentController(IINSTALLMENTService installmentservice, IDEBTREGISTERService debtregisterService,IREQUESTService requestService, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService)
        {
            _installmentService = installmentservice;
            _debtregisterService = debtregisterService;
            _requestService = requestService;
            _staffmemberService = staffmemberService;
            _studentService = studentService;
        }
        public async Task<IActionResult> InstallmentsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Installments, drg=>drg.Student);
            bool authorize = true;
            if (User.IsInRole(UserRoles.StaffMember))
            {
                var userid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(userid, sm => sm.Students);
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
                var installments = debtregister.Installments;
                return View("Installments", installments);
            }
            else
            {
                var vm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
                return RedirectToAction("Error", "Home", vm);
            }
        }
    }
}