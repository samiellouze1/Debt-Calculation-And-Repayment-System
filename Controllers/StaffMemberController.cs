using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StaffMemberController : Controller
    {
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        public StaffMemberController(ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService)
        {
            _staffmemberService = staffmemberService;
            _studentService = studentService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllStaffMembers ()
        {
            var staffmembers = await _staffmemberService.GetAllAsync(sm => sm.Students);
            return View("StaffMembers", staffmembers);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> MyProfile()
        {
            var id = User.FindFirstValue("Id");
            var staff = await _staffmemberService.GetByIdAsync(id);
            return View("StaffMember",staff);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> StaffMemberById(string id)
        {
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            return View("StaffMember", staffmember);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> StaffMemberByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id, s => s.StaffMember, s => s.DebtRegister);
            var staffmember = student.StaffMember;
            return View("StaffMember", staffmember);
        }
    }
}
