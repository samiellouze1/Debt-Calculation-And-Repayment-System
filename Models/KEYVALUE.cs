namespace Debt_Calculation_And_Repayment_System.Models
{
    using Debt_Calculation_And_Repayment_System.Data.Repository;
    using System;
    using System.Collections.Generic;
    public class KEYVALUE:IEntityBase
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public virtual ICollection<USER> USERs { get; set; }
        public virtual ICollection<USER> USERs1 { get; set; }
        public virtual ICollection<USER> USERs2 { get; set; }
        public virtual ICollection<USER> USERs3 { get; set; }
    }
}
