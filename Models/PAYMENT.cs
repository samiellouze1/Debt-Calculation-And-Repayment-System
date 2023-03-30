namespace Debt_Calculation_And_Repayment_System.Models
{
    using System;
    using System.Collections.Generic;
    public class PAYMENT
    {
        public int Id { get; set; }
        public Nullable<int> PaymentPlanId { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> OverdueAmount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public virtual PAYMENTPLAN PaymentPlan { get; set; }

    }
}
