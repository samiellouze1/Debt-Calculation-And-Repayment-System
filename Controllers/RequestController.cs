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
        private readonly ISTUDENTService _studentService;

        public RequestController(IREQUESTService requestService, IDEBTREGISTERService debtregisterService,ISTUDENTService studentService)
        {
            _requestService = requestService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
        }

        public async Task<IActionResult> RequestsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var request = debtregister.Requests;
            return View("Requests", request);
        }
        public IActionResult CreateRequest ()
        {
            var vm = new CreateRequestVM();
            return View("CreateRequest", vm);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateRequestVM vm)
        {
            var studentid = User.FindFirstValue("Id");
            var student= await _studentService.GetByIdAsync(studentid, s => s.StaffMember, s => s.DebtRegister);
            var debtregister = student.DebtRegister;
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
