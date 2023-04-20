using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        [Required(ErrorMessage ="ToBePaidFull is required")]
        public decimal ToBePaidFull { get; set; }
        [Required(ErrorMessage ="ToBePaidInstallment is requried")]
        public decimal ToBePaidInstallment { get; set; }
        [Required(ErrorMessage ="Total")]
        public decimal Total { get; set; }
        [Required(ErrorMessage ="Number of months")]
        public int NumOfMonths { get; set;}

    }
}
