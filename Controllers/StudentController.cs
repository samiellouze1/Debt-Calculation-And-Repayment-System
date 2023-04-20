using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
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
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllStudents()
        {
            var students = await _studentService.GetAllAsync(s => s.StaffMember, s => s.DebtRegister);
            return View("Students", students.OrderByDescending(s=>s.RegDate).ToList());
        }
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyProfile ()
        {
            var id = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(id);
            return View("Student",student);
        }
        public async Task<IActionResult> StudentById(string id)
        {
            var student = await _studentService.GetByIdAsync(id, s => s.StaffMember, s => s.DebtRegister);
            bool authorize=true;
            if (User.IsInRole("StaffMember"))
            {
                var staffid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(staffid, sm=>sm.Students);
                var staffstudentids = staff.Students.Select(s => s.Id).ToList();
                authorize = staffstudentids.Contains(student.Id);
            }
            if ( User.IsInRole("Student"))
            {
                var studentid = User.FindFirstValue("Id");
                authorize = studentid==id;
            }
            if (authorize)
            {
                return View("Student", student);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
            }
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> MyStudents()
        {
            var id = User.FindFirstValue("Id");
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            var students = staffmember.Students;
            return View("Students",students.OrderByDescending(s=>s.RegDate).ToList());
        }
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> StudentsByStaffMember(string id)
        {
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                authorize = id == User.FindFirstValue("Id");
            }
            if (authorize)
            {
                var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
                var students = staffmember.Students;
                return View("Students", students.OrderByDescending(s => s.RegDate).ToList());
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
            }
        }

    }
}
