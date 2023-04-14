using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class DEBTREGISTER:IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public decimal TotalCash { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public decimal TotalAfterInterest { get; set; }
        [Required]
        public decimal TotalAfterRequest { get; set; }
        [Required]
        public decimal PaidCash { get; set; }
        [Required]
        public decimal NotPaidCash { get; set; }
        [Required]
        public decimal TotalInstallmentAfterRequest { get; set; }
        [Required]
        public decimal TotalInstallment { get; set; }
        [Required]
        public decimal PaidInstallment { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [Required]
        public decimal NotPaidInstallment { get; set; }
        [Required]
        [Range(0,1)]
        public decimal InterestRate { get; set; }
        public virtual STUDENT Student { get; set; }
        public virtual REQUEST Request { get; set; }
        public virtual List<DEBT> Debts { get; set; } = new List<DEBT>();
        public virtual List<PAYMENT> Payments { get; set; } = new List<PAYMENT>();
        public virtual List<INSTALLMENT> Installments { get; set; } = new List<INSTALLMENT>();
    }
}
