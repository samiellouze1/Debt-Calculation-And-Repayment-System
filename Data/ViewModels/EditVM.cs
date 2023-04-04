using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class EditVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
