using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PAYMENTPLANINSTALLMENT: PAYMENTPLAN, IEntityBase
    {
        [Required]
        public int? NumOfInstallments { get; set; }
        public virtual List<INSTALLMENT>? Installments { get; set; }
        [Required]
        public decimal? AmountAfterInstallments { get; set; }
    }
}
