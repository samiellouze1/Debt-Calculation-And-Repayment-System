using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var staffmembers = await _staffmemberService.GetAllAsync();
            return View("StaffMembers", staffmembers);
        }
        public async Task<IActionResult> StaffMemberById(string id)
        {
            var staffmember = await _staffmemberService.GetByIdAsync(id);
            return View("StaffMember", staffmember);
        }
        public async Task<IActionResult> StaffMemberByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var staffmember = student.StaffMember;
            return View("StaffMember", staffmember);
        }
    }
}
