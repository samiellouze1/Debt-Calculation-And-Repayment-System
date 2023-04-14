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
        public DateTime RegDate { get; set; }
        [Required]
        [Range(0,1)]
        public decimal InterestRate { get; set; }
        public virtual DEBTREGISTER DebtRegister { get; set; }

    }
}
