using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        [Required]
        public decimal ToBePaidFull { get; set; }
        [Required]
        public decimal ToBePaidInstallment { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public int NumOfMonths { get; set;}

    }
}
