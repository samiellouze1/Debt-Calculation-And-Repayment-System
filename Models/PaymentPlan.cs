using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PAYMENTPLAN : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        [Required]
        public string? Type { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        public string? DebtId { get; set; }
        [ForeignKey("DebtId")]
        public virtual DEBT? Debt { get; set; }
    }
}
