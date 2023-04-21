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
        public decimal Amount { get; set; }
        [Required]
        public decimal InterestAmount { get; set; }
        [Required]
        public decimal Total { get; set; }
        public decimal ToBePaid { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalInstallment { get; set; }
        public decimal ToBePaidCash { get; set; }
        public decimal ToBePaidInstallment { get; set; }
        [Required]
        [Range(0,1)]
        public decimal InterestRate { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        public DateTime ReqDate { get; set; }
        public int NumOfMonths { get; set; }
        public string StudentId { get; set; }
        public virtual STUDENT Student { get; set; }
        public virtual List<DEBT> Debts { get; set; } = new List<DEBT>();
        public virtual List<PAYMENT> Payments { get; set; } = new List<PAYMENT>();
        public virtual List<INSTALLMENT> Installments { get; set; } = new List<INSTALLMENT>();
        public virtual List<REQUEST> Requests { get; set; } = new List<REQUEST>();
    }
}
