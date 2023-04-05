using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentPlanController : Controller
    {
        private readonly IPAYMENTPLANService _paymentPlanService;
        public PaymentPlanController(IPAYMENTPLANService paymentPlanService)
        {
            _paymentPlanService = paymentPlanService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
