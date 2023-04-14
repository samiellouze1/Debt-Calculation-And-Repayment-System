using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly IDEBTREGISTERService _debtregisterService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService)
        {
            _debtregisterService = debtregisterService;
        }
    }
}
