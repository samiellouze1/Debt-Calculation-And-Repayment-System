using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class ProgramTypeVM
    {
        public string Id { get; set; }
        public string Type { get; set; }
         
        public decimal InterestRate { get; set; }
         
        public decimal InterestRateInstallment { get; set; }
        
        public decimal InterestRateDelay { get; set; }
         
        public string Currency { get; set; }
    }
}
