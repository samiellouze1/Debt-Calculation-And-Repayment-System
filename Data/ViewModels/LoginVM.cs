using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage="Email*")]
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre*")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
