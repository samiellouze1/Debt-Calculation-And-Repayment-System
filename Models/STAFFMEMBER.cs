namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STAFFMEMBER:USER
    {
        public virtual List<STUDENT> Students { get; set; } = new List<STUDENT>();
    }
}
