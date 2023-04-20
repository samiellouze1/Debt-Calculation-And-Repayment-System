using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Debt_Calculation_And_Repayment_System.Models
{
    public class STUDENT:USER
    {
        public virtual STAFFMEMBER StaffMember { get; set; } = new STAFFMEMBER();
        public virtual DEBTREGISTER DebtRegister { get; set; } = new DEBTREGISTER();
        public bool StaffMemberAssigned { get; set; } = false;
    }
}
