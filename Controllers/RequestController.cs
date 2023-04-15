using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class RequestController : Controller
    {
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;

        public RequestController(IREQUESTService requestService, IDEBTREGISTERService debtregisterService)
        {
            _requestService = requestService;
            _debtregisterService = debtregisterService;
        }

        public async Task<IActionResult> RequestByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id);
            var request = debtregister.Requests.Where(r => r.Status == "Accepted").ToList()[0];
            return View("Request", request);
        }
        public IActionResult CreateRequest ()
        {
            var vm = new CreateRequestVM();
            return View("CreateRequest", vm);
        }

        public async Task<IActionResult> CreateRequest(CreateRequestVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateRequest", vm);
            }
            else
            {
                var debtregister = await _debtregisterService.GetByIdAsync(vm.DebtRegisterId);
                var newRequest = new REQUEST()
                {
                    ToBePaidFull = vm.ToBePaidFull,
                    ToBePaidInstallment = vm.ToBePaidInstallment,
                    NumOfMonths = vm.NumOfMonths,
                    InterestRate = vm.InterestRate,
                    RegDate = DateTime.Now,
                    Status = "Not Specified",
                    DebtRegister=debtregister
                };
                await _requestService.AddAsync(newRequest);
                return RedirectToAction("Index","Home");
            }
        }
    }
}
