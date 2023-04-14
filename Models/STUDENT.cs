using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        public virtual STAFFMEMBER StaffMember { get; set; }
        public virtual DEBTREGISTER DebtRegister { get; set; }
    }
}
