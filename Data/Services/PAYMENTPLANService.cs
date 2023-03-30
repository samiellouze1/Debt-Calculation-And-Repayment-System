using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.Services
{
    public class KEYVALUEService: EntityBaseRepository<KEYVALUE>,IKEYVALUEService
    {
        public KEYVALUEService(AppDbContext context): base(context)
    }
}
