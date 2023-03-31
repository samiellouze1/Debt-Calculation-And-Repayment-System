namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class KEYVALUE:IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public virtual ICollection<USER> USERs { get; set; }
        public virtual ICollection<USER> USERs1 { get; set; }
        public virtual ICollection<USER> USERs2 { get; set; }
        public virtual ICollection<USER> USERs3 { get; set; }
    }
}
