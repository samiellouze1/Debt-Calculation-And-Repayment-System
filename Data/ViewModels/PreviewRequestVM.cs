using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class PreviewRequestVM
    {
        [Required(ErrorMessage ="ToBePaidFull is required")]
        public decimal ToBePaidFull { get; set; }
        [Required(ErrorMessage ="Number of months is required")]
        public int NumOfMonths { get; set;}
        public bool Accept { get; set; }
        [Required(ErrorMessage="Total is required")]
        public decimal Total { get; set; }
        [Required(ErrorMessage ="ToBePaidInstallment is required")]
        public decimal ToBePaidInstallment { get; set; }
        public decimal ToBePaidEachMonth { get; set; }
    }
}
