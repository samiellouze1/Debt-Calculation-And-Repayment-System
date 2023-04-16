using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        public IActionResult PreviewRequest ()
        {
            var vm = new CreateRequestVM();
            return View("PreviewRequest", vm);
        }
        [HttpPost]
        public async Task<IActionResult> PreviewRequest(CreateRequestVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            else
            {
                CreateRequest(vm);
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> CreateRequest(CreateRequestVM vm)
        {
            if (ModelState.IsValid)
            {
                var studentid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
                var debtregister = student.DebtRegister;
                var tobepaideachmonth = 0m;
                for ( int i=1;i<=vm.NumOfMonths;i++)
                {
                    var eachmonthafterinterest = (debtregister.TotalAfterInterest - vm.ToBePaidFull) / vm.NumOfMonths;
                    var nod = (DateTime.Now.AddMonths(i)-DateTime.Now).Days;
                    var add = eachmonthafterinterest * (1 + nod * debtregister.InterestRate / 365);
                    tobepaideachmonth += add;
                }
                var newvm = new PreviewRequestVM() { ToBePaidFull = vm.ToBePaidFull, NumOfMonths = vm.NumOfMonths, ToBePaidInstallment = debtregister.TotalAfterInterest - vm.ToBePaidFull, ToBePaidEachMonth= tobepaideachmonth};
                return await CreateRequest(newvm);
            }
            else
            {
                return View(vm);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateRequest(PreviewRequestVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Accept)
                {
                    var studentid = User.FindFirstValue("Id");
                    var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
                    var debtregister = student.DebtRegister;
                    var newrequest = new REQUEST()
                    {
                        ToBePaidFull = vm.ToBePaidFull,
                        ToBePaidInstallment = vm.ToBePaidInstallment,
                        NumOfMonths = vm.NumOfMonths,
                        RegDate = DateTime.Now,
                        Status = "Not Defined",
                        DebtRegister = debtregister,
                    };
                    await _requestService.AddAsync(newrequest);
                    return RedirectToAction("Index", "Home");
                }
                else if (!vm.Accept)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }
    }
}
