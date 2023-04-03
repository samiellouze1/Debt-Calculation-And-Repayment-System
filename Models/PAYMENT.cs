namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class PAYMENT: IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal OverdueAmount { get; set; }
        public decimal Total { get; set; }
        public DateTime RegDate { get; set; }
        public bool Deleted { get; set; }
        [Required]
        public string PaymentPlanId { get; set; }
        [ForeignKey("PaymentPlanId")]
        public virtual PAYMENTPLAN PaymentPlan { get; set; }

    }
}
