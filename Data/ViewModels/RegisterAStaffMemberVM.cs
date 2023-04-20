using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class RegisterAStaffMemberVM
    {
        [Required(ErrorMessage ="First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Last Name is required")]
        public string SurName { get; set; }
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Address")]
        public string Address { get; set; }
        [Required(ErrorMessage ="Phone Number is required")]
        public string PhoneNumber { get; set; }
    }
}
