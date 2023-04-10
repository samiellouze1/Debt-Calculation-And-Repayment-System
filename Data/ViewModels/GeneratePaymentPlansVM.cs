using Microsoft.Build.Framework;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class GeneratePaymentPlansVM
    {
        [Required]
        public string DebtId { get; set; }
        [Required]
        public int NumOfMonths { get; set; }
    }
}
