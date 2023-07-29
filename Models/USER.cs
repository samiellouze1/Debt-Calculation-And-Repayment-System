using Debt_Calculation_And_Repayment_System.Data.Repository;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class USER : IdentityUser,IEntityBase
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ProgramFinishDate { get; set; }
        [Required]
        public string? Address { get; set; }

        public string? GuarantorName { get; set; }
        
        public string? GuarantorMobile { get; set; }

        public string? GuarantorIdentityNumber { get; set; }
        public string? GuarantorAddress { get; set; }

        public string? IdentityNumber{ get; set; }
        public string? Desc { get; set; }



    }
}
