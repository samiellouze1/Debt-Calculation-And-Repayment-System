using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class CreateDebtVM
    {
        [Required(ErrorMessage ="Amount Is Required")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage ="Start Date is required")]
        public DateTime StartDate { get; set; }
        [Required]
        [Compare("StartDate", ErrorMessage = "End Date must be greater than Start Date.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage ="Student is required")]
        public string StudentId { get; set; }
        public List<STUDENT>? Students { get; set; }
    }
}
