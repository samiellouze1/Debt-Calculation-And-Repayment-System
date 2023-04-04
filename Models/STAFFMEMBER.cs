namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STAFFMEMBER:USER
    {
        public virtual ICollection<STUDENT> Students { get; set; }
    }
}
