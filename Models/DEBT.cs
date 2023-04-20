using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.Annotation;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class DEBT : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [DateNotInFuture(ErrorMessage ="You provided a date in the future")]
        public DateTime StartDate { get; set; }

        [Required]
        [Compare("StartDate", ErrorMessage = "End Date must be greater than Start Date.")]
        [DateNotInFuture(ErrorMessage ="You provided a date in the future")]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime RegDate { get; set; }

        public virtual DEBTREGISTER DebtRegister { get; set; }
    }
}
