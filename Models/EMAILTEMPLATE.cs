using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.Annotation;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class EMAILTEMPLATE : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public bool NeedToLogin { get; set; }
    }
}
