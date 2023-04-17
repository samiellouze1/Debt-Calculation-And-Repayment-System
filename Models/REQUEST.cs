using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class REQUEST:IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public decimal ToBePaidFull { get; set; }
        [Required]
        public decimal ToBePaidInstallment { get; set; }
        [Required]
        public int NumOfMonths { get; set; }
        [Required]
        public DateOnly RegDate { get; set; }
        [Required]
        public string Status { get; set; }
        public virtual DEBTREGISTER DebtRegister { get; set; }
    }
}
