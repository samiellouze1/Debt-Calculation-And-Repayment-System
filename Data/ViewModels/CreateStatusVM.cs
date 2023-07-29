
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateStatusVM
    {
        [Required(ErrorMessage ="Tip*")]
        public string Type { get; set; }
    }
}
