using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        public string VerificationCode { get; set; }
        public string StaffMemberId { get; set; }
        [ForeignKey("StaffMemberId")]
        public virtual STAFFMEMBER StaffMember { get; set; }
        public virtual List<SCOLARSHIPDEBT> ScolarshipDebts { get; set; } = new List<SCOLARSHIPDEBT>();
    }
}
