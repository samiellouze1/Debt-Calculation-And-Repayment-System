using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class InstallmentController : Controller
    {
        private readonly IINSTALLMENTService _installmentService;
        public InstallmentController(IINSTALLMENTService installmentservice)
        {
            _installmentService = installmentservice;
        }

    }
}
