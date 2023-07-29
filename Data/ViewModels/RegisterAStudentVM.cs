using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class RegisterAStudentVM
    {
        [Required(ErrorMessage = "İsim*")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Soyisim*")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Email*")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Adres*")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Telefon*")]
        public string PhoneNumber { get; set; }
        //[Required(ErrorMessage ="Faiz Oranı*")]
        //public decimal InterestRate { get; set; }
        [Required(ErrorMessage ="Program*")]
        public string ProgramID { get; set; }

        [Required(ErrorMessage = "Son Ödeme Tarihi*")]
        public DateTime ProgramFinishDate { get; set; }

        [Required(ErrorMessage = "Program*")]
        public List<PROGRAMTYPE> ProgramTypes { get; set; }= new List<PROGRAMTYPE> ();

        public string GuarantorName { get; set; }

        public string GuarantorMobile { get; set; }

        public string GuarantorIdentityNumber { get; set; }

        public string GuarantorAddress { get; set; }

        public string IdentityNumber { get; set; }
        public string Desc { get; set; }

    }
}
