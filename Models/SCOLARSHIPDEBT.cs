namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class SCOLARSHIPDEBT : IEntityBase
    {
        [Key]
        [Required]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public decimal Rate { get; set; }
        public int NumberOfDays { get; set; }
        public decimal Interest { get; set; }
        public DateTime RegDate { get; set; }
        public int RegUserId { get; set; }
        public virtual USER UserRegister { get; set; }
        public int UserId { get; set; }
        public virtual USER User { get; set; }
        public bool Deleted { get; set; }
    }
}
