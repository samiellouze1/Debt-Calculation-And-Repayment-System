using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        public string VerificationCode { get; set; }
        public string StaffMemberId { get; set; }
        [ForeignKey("RegUserId")]
        public virtual STAFFMEMBER StaffMember { get; set; }
        public virtual ICollection<SCOLARSHIPDEBT> ScolarshipDebts { get; set; }
    }
}
