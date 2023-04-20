
using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class SendEmailViewModel
    {
        [Required(ErrorMessage="Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }
        public List<USER> Users { get; set; }
    }
}
