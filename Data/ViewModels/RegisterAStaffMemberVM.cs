using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class RegisterAStaffMemberVM
    {
        [Required(ErrorMessage ="İsim*")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Soyisim*")]
        public string SurName { get; set; }
        [Required(ErrorMessage ="Email*")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Adres*")]
        public string Address { get; set; }
        [Required(ErrorMessage ="Telefon")]
        public string PhoneNumber { get; set; }
    }
}
