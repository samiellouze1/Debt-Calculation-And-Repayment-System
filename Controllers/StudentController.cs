using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StudentController : Controller
    {
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        public StudentController(ISTUDENTService studentService,ISTAFFMEMBERService staffmemberService)
        {
            _studentService = studentService;
            _staffmemberService = staffmemberService;
        }
        public async Task<IActionResult> StudentById(string id)
        {
            var student = _studentService.GetByIdAsync(id);
            return View("Student", student);
        }
        public async Task<IActionResult> AllStudents()
        {
            var students = await _studentService.GetAllAsync();
            return View("Students",students);
        }
        public async Task<IActionResult> StudentsByStaffMember(string id)
        {
            var staffmember = await _staffmemberService.GetByIdAsync(id);
            var students = staffmember.Students;
            return View("Students", students);
        }
    }
}
