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
        public decimal DebtsTotal { get; set; }
        [Required]
        public decimal PaidCash { get; set; }
        [Required]
        public decimal PaidInstallment { get; set; }
        [Required]
        public decimal NotPaidInstallment { get; set; }
        [Required]
        public decimal InterestRate { get; set; }
        public virtual STUDENT Student { get; set; }
        public virtual REQUEST Request { get; set; }
        public virtual List<DEBT> Debts { get; set; }
    }
}
