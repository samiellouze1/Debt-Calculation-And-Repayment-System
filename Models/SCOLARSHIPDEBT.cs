namespace Debt_Calculation_And_Repayment_System.Models
{
    using System;
    using System.Collections.Generic;
    public class SCOLARSHIPDEBT
    {
        public string Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> Day { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public Nullable<int> RegUserId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public virtual USER User { get; set; }
        public virtual USER User1 { get; set; }
    }
}
