using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        [Required]
        public string? StaffMemberId { get; set; }
        [ForeignKey("StaffMemberId")]
        public virtual STAFFMEMBER? StaffMember { get; set; }
        public virtual List<DEBT>? Debts { get; set; } = new List<DEBT>();
    }
}
