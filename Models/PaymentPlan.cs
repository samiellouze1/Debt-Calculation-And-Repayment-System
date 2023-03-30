namespace Debt_Calculation_And_Repayment_System.Models
{
    using System;
    using System.Collections.Generic;
    public class PAYMENTPLAN
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> UserId1 { get; set; }
        public Nullable<int> NumOfInstallment { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> MonthlyTotalAmount { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public Nullable<int> RegUserId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public virtual ICollection<PAYMENT> Payments { get; set; }
        public virtual USER User { get; set; }
        public virtual USER User1 { get; set; }
    }
}
