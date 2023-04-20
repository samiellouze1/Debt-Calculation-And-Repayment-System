using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class AffectVM
    {
        [Required(ErrorMessage="Staff Member is required")]
        public string StaffMemberId { get; set; }
        [Required(ErrorMessage ="Student is required")]
        public string StudentId { get; set; }
        public List<STUDENT> Students { get;set; }
        public List<STAFFMEMBER> StaffMembers { get; set; }
    }
}
