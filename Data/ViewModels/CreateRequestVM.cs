using Debt_Calculation_And_Repayment_System.Data.Annotation;
using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateRequestVM
    {
        [Required(ErrorMessage = "ToBePaidFull is required")]
        [Range(0, double.MaxValue, ErrorMessage = "ToBePaidFull must be a positive value")]
        [LessThanTotal("Total", ErrorMessage = "ToBePaidFull must be less than Total")]
        public decimal ToBePaidFull { get; set; }

        [Required(ErrorMessage = "Number of months is required")]
        public int NumOfMonths { get; set; }
        [Required(ErrorMessage = "Total is required")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "ToBePaidInstallment is required")]
        [Range(0, double.MaxValue, ErrorMessage = "ToBePaidInstallment must be a positive value")]
        [LessThanTotal("Total", ErrorMessage = "ToBePaidInstallment must be less than Total")]
        public decimal ToBePaidInstallment { get; set; }

    }
}
