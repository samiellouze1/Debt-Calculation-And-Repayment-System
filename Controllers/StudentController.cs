using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> Student(string id)
        {
            var student = _studentService.GetByIdAsync(id);
            return View("Student", student);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllStudents()
        {
            var students = await _studentService.GetAllAsync();
            return View("Students",students);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> MyStudents()
        {
            var staffid = User.FindFirstValue("Id");
            var staffmember=await _staffmemberService.GetByIdAsync(staffid);
            var students = staffmember.Students;
            return View("Students", students);
        }
    }
}
