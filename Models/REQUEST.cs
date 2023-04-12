using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class REQUEST:IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        [Required]
        public string? NumOfMonths { get; set; }
        [Required]
        public bool? Status { get; set; }
        [Required]
        public string? DebtId { get; set; }
        [ForeignKey("DebtId")]
        public virtual DEBT? Debt { get; set; }
        
    }
}
