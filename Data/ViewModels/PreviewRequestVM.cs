using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class PreviewRequestVM
    {
        [Required]
        public decimal ToBePaidFull { get; set; }
        [Required]
        public int NumOfMonths { get; set;}
        public bool Accept { get; set; }
        public decimal ToBePaidInstallment { get; set; }
        public decimal ToBePaidEachMonth { get; set; }
    }
}
