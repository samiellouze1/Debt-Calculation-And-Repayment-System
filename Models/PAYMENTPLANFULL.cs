using Debt_Calculation_And_Repayment_System.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PAYMENTPLANFULL:PAYMENTPLAN, IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
