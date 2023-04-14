using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PAYMENT:IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public decimal Sum { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public virtual DEBTREGISTER DebtRegister { get; set; }
    }
}
