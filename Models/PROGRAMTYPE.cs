using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.Annotation;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PROGRAMTYPE : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Range(0, 1)]
        public decimal InterestRate { get; set; }
        [Required]
        [Range(0, 1)]
        public decimal InterestRateInstallment { get; set; }
        [Required]
        [Range(0, 1)]
        public decimal InterestRateDelay { get; set; }
        [Required]
        public string Currency { get; set; }
        
    }
}
