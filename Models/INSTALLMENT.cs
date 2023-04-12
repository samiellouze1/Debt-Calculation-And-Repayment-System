using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class INSTALLMENT : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        public DateTime SupposedPaymentDate {get;set; }
        public DateTime ActualPaymentDate { get; set; }
        [Required]
        public string? PaymentPlanInstallmentId { get; set; }
        [ForeignKey("PaymentPlanInstallmentId")]
        public PAYMENTPLANINSTALLMENT? PaymentPlanInstallment { get; set; }
    }
}
