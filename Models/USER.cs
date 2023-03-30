namespace Debt_Calculation_And_Repayment_System.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    public class USER : IdentityUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public string UserNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public Nullable<int> ProgramId { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public Nullable<int> RegUserId { get; set; }
        public Nullable<int> PaymentPlanId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public virtual KEYVALUE KeyValue { get; set; }
        public virtual KEYVALUE KeyValue1 { get; set; }
        public virtual KEYVALUE KeyValue2 { get; set; }
        public virtual KEYVALUE KeyValue3 { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlans { get; set; }
        public virtual ICollection<PAYMENTPLAN> PaymentPlans1 { get; set; }
        public virtual ICollection<SCOLARSHIPDEPT> ScolarshipDebts { get; set; }
        public virtual ICollection<SCOLARSHIPDEPT> ScolarshipDebts1 { get; set; }
        public virtual ICollection<USER> User1 { get; set; }
        public virtual USER User2 { get; set; }
    }
}
