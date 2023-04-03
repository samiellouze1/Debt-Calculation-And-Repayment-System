namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class USER : IdentityUser, IEntityBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public DateTime RegDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProgramId { get; set; }
        public decimal Rate { get; set; }
        public string PaymentPlanId { get; set; }
        public bool Deleted { get; set; }
        public string VerificationCode { get; set; }
        public string PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual PAYMENT Payment { get; set;}
        public string KeyValueId { get; set; }
        [ForeignKey("KeyValueId")]
        public virtual KEYVALUE KeyValue { get; set; }
        public string KeyValueId1 { get; set; }
        [ForeignKey("KeyValueId1")]
        public virtual KEYVALUE KeyValue1 { get; set; }
        public string KeyValueId2 { get; set; }
        [ForeignKey("KeyValueId2")]
        public virtual KEYVALUE KeyValue2 { get; set; }
        public string KeyValueId3 { get; set; }
        [ForeignKey("KeyValueId3")]

        public virtual KEYVALUE KeyValue3 { get; set; }
        public string RegUserId { get; set; }
        [ForeignKey("RegUserId")]
        public virtual USER User2 { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlans { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlans1 { get; set; }
        public virtual ICollection<SCOLARSHIPDEBT> ScolarshipDebtsHeHas { get; set; }
        public virtual ICollection<SCOLARSHIPDEBT> ScolarshipDebtsHeRegistered { get; set; }
        public virtual ICollection<USER> UserRegister { get; set; }
    }
}
