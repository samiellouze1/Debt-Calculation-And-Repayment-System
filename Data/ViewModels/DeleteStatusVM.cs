using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class DeleteStatusVM
    {
        public string Id { get; set; }
        public string? Type { get; set; }
        public bool Delete { get; set; }
    }
}
