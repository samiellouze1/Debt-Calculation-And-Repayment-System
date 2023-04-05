using Microsoft.Build.Framework;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class ChangePasswordVM
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string OldPassword { get; set; }
    }
}
