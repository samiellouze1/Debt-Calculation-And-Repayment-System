using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class ChangeStatusVM
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<STUDENTSTATUSTYPE>? statuses { get; set; }
    }
}
