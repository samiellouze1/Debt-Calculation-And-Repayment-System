using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class InstallmentController : Controller
    {
        private readonly IINSTALLMENTService _installmentService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly IREQUESTService _requestService;
        public InstallmentController(IINSTALLMENTService installmentservice, IDEBTREGISTERService debtregisterService,IREQUESTService requestService)
        {
            _installmentService = installmentservice;
            _debtregisterService = debtregisterService;
            _requestService = requestService;
        }

        public async Task<IActionResult> InstallmentsByDebtRegister(string id)
        {
            var debt=await _debtregisterService.GetByIdAsync(id);
            var installments = debt.Installments;
            return View("Installments", installments);
        }
    }
}