using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.Services
{
    public class REQUESTService: EntityBaseRepository<REQUEST>,IREQUESTService
    {
        public REQUESTService(AppDbContext context): base(context)
        {

        }
    }
}
