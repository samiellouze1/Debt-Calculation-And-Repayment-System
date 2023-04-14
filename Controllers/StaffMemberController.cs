using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StaffMemberController : Controller
    {
        private readonly ISTAFFMEMBERService _staffmemberService;
        public StaffMemberController(ISTAFFMEMBERService staffmemberService)
        {
            _staffmemberService = staffmemberService;
        }
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> StaffMember(string id)
        {
            var staffmember = _staffmemberService.GetByIdAsync(id);
            return View("StaffMember", staffmember);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllStaffMembers ()
        {
            var staffmembers = _staffmemberService.GetAllAsync();
            return View("StaffMembers", staffmembers);
        }
    }
}
