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
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime FinishDate { get; set; }
        [Required]
        public decimal Rate { get; set; }
        [Required]
        public string NumberOfDays { get; set; }
        [Required]
        public decimal Interest { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        [Required]
        public bool Deleted { get; set; }
        [Required]
        public string RegUserId { get; set; }
        [ForeignKey("RegUserId")]
        public virtual STAFFMEMBER RegUser { get; set; }
        [Required]
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual STUDENT Student { get; set; }
    }
}
