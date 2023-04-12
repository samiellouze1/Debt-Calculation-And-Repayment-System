using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class DecideRequestVM
    {
        [Required]
        public string RequestId { get; set; }
        [Required]
        public string decision { get; set; }
    }
}
