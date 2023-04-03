using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        public string VerificationCode { get; set; }
        public string PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual PAYMENT Payment { get; set; }
        public string RegUserId { get; set; }
        [ForeignKey("RegUserId")]
        public virtual STAFFMEMBER RegUser { get; set; }
        public virtual ICollection<SCOLARSHIPDEBT> ScolarshipDebtsHeHas { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlansHeHas { get; set; }
    }
}
