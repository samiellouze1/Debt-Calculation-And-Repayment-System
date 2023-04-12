using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class InstallmentController : Controller
    {
        private readonly IINSTALLMENTService _installmentService;
        private readonly IPAYMENTPLANINSTALLMENTService _paymentPlanInstallmentService;
        public InstallmentController(IINSTALLMENTService installmentservice, IPAYMENTPLANINSTALLMENTService paymentPlanInstallmentService)
        {
            _installmentService = installmentservice;
            _paymentPlanInstallmentService = paymentPlanInstallmentService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> InstallmentsByPaymentPlanAdmin( string id)
        {
            var paymentplan = await _paymentPlanInstallmentService.GetByIdAsync(id);
            var installments = paymentplan.Installments.ToList();
            return View(installments);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> InstallmentsByPaymentPlanStaffMember(string id)
        {
            var paymentplan = await _paymentPlanInstallmentService.GetByIdAsync(id);
            var installments = paymentplan.Installments.ToList();
            return View(installments);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> InstallmentsByPaymentPlanStudent(string id)
        {
            var paymentplan = await _paymentPlanInstallmentService.GetByIdAsync(id);
            var installments = paymentplan.Installments.ToList();
            return View(installments);
        }
    }
}
