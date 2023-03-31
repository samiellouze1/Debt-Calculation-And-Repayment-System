using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.Services
{
    public class PAYMENTPLANService: EntityBaseRepository<PAYMENTPLAN>,IPAYMENTPLANService
    {
        public PAYMENTPLANService(AppDbContext context): base(context)
        {

        }
    }
}
