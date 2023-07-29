using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class ResetPasswordVM
    {
        [Required(ErrorMessage ="Şifre*")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Şifre en az 8 karakter uzunluğunda olmalı ve en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre Onayla*")]
        [Compare(nameof(Password), ErrorMessage = "Şifre ve onay şifresi uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Email*")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Code*")]
        public string Code { get; set; }
    }
}
