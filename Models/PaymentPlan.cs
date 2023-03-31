namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class PAYMENTPLAN : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public int NumOfInstallment { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime FinishDate { get; set; }
        [Required]
        public decimal PrincipalAmount { get; set; }
        [Required]
        public decimal InterestAmount { get; set; }
        [Required]
        public decimal MonthlyTotalAmount { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [Required]
        public bool Deleted { get; set; }
        [Required]
        public virtual ICollection<PAYMENT> Payments { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual USER User { get; set; }
        [Required]
        public string RegUserId { get; set; }
        [ForeignKey("RegUserId")]
        public virtual USER UserRegister { get; set; }
    }
}
