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
        public string Id { get; set; }
        [Required]
        public decimal InitialAmount { get; set; }
        [Required]
        public decimal AmountAfterInterest { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public int NumberOfDays { get; set; }
        public virtual DEBTREGISTER DebtRegister { get; set; }
    }
}
