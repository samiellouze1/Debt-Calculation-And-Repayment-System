using Debt_Calculation_And_Repayment_System.Data.Repository;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class USER : IdentityUser,IEntityBase
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? SurName { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [Required]
        public string? Address { get; set; }
    }
}
