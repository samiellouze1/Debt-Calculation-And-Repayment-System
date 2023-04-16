using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class AffectVM
    {
        public string StaffMemberId { get; set; }
        public string StudentId { get; set; }
        public List<STUDENT> Students { get;set; }
        public List<STAFFMEMBER> StaffMembers { get; set; }
    }
}
