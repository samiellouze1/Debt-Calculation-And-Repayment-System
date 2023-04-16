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
        public StudentController(ISTUDENTService studentService,ISTAFFMEMBERService staffmemberService,IUSERService userService)
        {
            _studentService = studentService;
            _staffmemberService = staffmemberService;
        }
        public async Task<IActionResult> AllStudents()
        {
            var students = await _studentService.GetAllAsync(s => s.StaffMember, s => s.DebtRegister);
            return View("Students", students);
        }
        public async Task<IActionResult> StudentById(string id)
        {
            var student = await _studentService.GetByIdAsync(id,s=>s.StaffMember, s => s.StaffMember, s => s.DebtRegister);
            return View("Student", student);
        }
        public async Task<IActionResult> MyStudents()
        {
            var id = User.FindFirstValue("Id");
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            var students = staffmember.Students;
            return View("Students",students);
        }
        public async Task<IActionResult> StudentsByStaffMember(string id)
        {
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            var students = staffmember.Students;
            return View("Students", students);
        }

    }
}
