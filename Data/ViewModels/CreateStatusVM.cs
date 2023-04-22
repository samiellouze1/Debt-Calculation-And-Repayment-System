
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateStatusVM
    {
        [Required(ErrorMessage ="Type is required")]
        public string Type { get; set; }
    }
}
