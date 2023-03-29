using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class PaymentPlan
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public int Rate { get; set; }
        public string IdStudent { get; set; }
        [ForeignKey("IdStudent")]
        public virtual Student Student { get; set; }
        public string IdScolarship { get; set; }
        [ForeignKey("IdScolarship")]
        public virtual Scolarship Scolarship {get;set;}
    }
}
