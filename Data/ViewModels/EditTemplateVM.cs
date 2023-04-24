
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class EditTemplateVM
    {
        [Required(ErrorMessage="Id is required")]
        public string TemplateId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public bool NeedToLogin { get; set; }
    }
}
