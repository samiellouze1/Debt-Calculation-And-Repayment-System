using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Repository;
using Debt_Calculation_And_Repayment_System.Models;

namespace Debt_Calculation_And_Repayment_System.Data.Services
{
    public class STAFFMEMBERService: EntityBaseRepository<STAFFMEMBER>,ISTAFFMEMBERService
    {
        public STAFFMEMBERService(AppDbContext context): base(context)
        {

        }
    }
}
