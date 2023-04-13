using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class previewVM
    {
        public string DebtId { get; set; }
        public int NumOfMonths { get; set; }
        public decimal EachMonth { get; set; }
        public decimal Total { get; set; }
        public DateTime FinishDate { get; set; }
        [Range(0,1)]
        public decimal InterestRate { get; set; }
        public string State { get; set; }
    }
}
