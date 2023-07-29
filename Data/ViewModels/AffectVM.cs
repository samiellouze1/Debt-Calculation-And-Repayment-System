using Debt_Calculation_And_Repayment_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Data.ViewModels
{
    public class AffectVM
    {
        [Required(ErrorMessage="Görevli Seçiniz")]
        public string StaffMemberId { get; set; }
        [Required(ErrorMessage ="Bursiyer seçiniz")]
        public string StudentId { get; set; }
        public List<STUDENT>? Students { get;set; }
        public List<STAFFMEMBER>? StaffMembers { get; set; }
    }
}
