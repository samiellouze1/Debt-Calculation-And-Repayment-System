using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class ScolarshipDebtController : Controller
    {
        private readonly ISCOLARSHIPDEBTService _scolarshipDebtService;
        public ScolarshipDebtController(ISCOLARSHIPDEBTService scolarshipDebtService)
        {
            _scolarshipDebtService = scolarshipDebtService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
