using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateDebtVM
    {
        [Required]
        public decimal InitialAmount { get; set; }
        [Required]
        [Range(0, 1)]
        public decimal InterestRate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        public string? StudentId { get; set; }
        public List<STUDENT>? Students { get; set; }
    }
}
