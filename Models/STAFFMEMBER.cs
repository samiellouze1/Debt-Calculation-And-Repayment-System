namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STAFFMEMBER:USER
    {
        public virtual ICollection<STUDENT> StudentsHeRegistered { get; set; }
        public virtual ICollection<SCOLARSHIPDEBT> ScolarshipDebtsHeRegistered { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlansHeRegistered { get; set; }
    }
}
