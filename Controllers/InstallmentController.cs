using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class InstallmentController : Controller
    {
        private readonly IINSTALLMENTService _installmentService;
        private readonly IDEBTService _debtService;
        public InstallmentController(IINSTALLMENTService installmentservice, IDEBTService debtService)
        {
            _installmentService = installmentservice;
            _debtService = debtService;
        }
        public async Task<IActionResult> InstallmentsByDebt(string id)
        {
            var debt=await _debtService.GetByIdAsync(id);
            var installments = debt.Installments;
            return View("Installments", installments);
        }
    }
}