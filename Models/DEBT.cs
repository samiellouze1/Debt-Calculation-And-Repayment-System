using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class DEBT : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public decimal InitialAmount { get; set; }
        [Required]
        [Range(0,1)]
        public decimal InterestRate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [Required]
        public string StudentId { get; set; }
        public virtual List<PAYMENTPLAN> PaymentPlans { get; set; } = new List<PAYMENTPLAN>();
    }
}
