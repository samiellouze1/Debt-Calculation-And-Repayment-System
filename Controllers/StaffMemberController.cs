using Debt_Calculation_And_Repayment_System.Data.IServices;
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
    }
}
