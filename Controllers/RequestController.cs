﻿using Debt_Calculation_And_Repayment_System.Data.IServices;
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
        private readonly ISTUDENTService _studentService;
        private readonly IDEBTService _debtService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IPAYMENTPLANINSTALLMENTService _paymentplaninstallmentService;
        private readonly IINSTALLMENTService _installmentService;
        public RequestController(IREQUESTService requestService,ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService, IDEBTService debtService, IPAYMENTPLANINSTALLMENTService paymentplaninstallmentService, IINSTALLMENTService installmentService)
        {
            _requestService = requestService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
            _debtService = debtService;
            _paymentplaninstallmentService = paymentplaninstallmentService;
            _installmentService = installmentService;
        }
        #region sendrequest
        [Authorize(Roles = "Student")]
        public IActionResult SendRequest(string id)
        {
            var vm = new SendRequestVM() { DebtId = id };
            return View(vm);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> SendRequest(SendRequestVM srvm)
        {
            if (!ModelState.IsValid)
            {
                return View(srvm);
            }
            else
            {
                await PreviewPaymentPlans(srvm.DebtId, srvm.NumOfMonths);
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region previewpaymentplans
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> PreviewPaymentPlans(string debtid,int numofmonths)
        {

            #region get
            var debt = await _debtService.GetByIdAsync(debtid);
            var NumOfMonths = numofmonths;
            var startdate = debt.StartDate;
            var interest = debt.InterestRate;
            var initialamount = debt.InitialAmount;
            #endregion

            var amoundpaidfull = debt.PaymentPlanFulls.ToList().Sum(ppf => ppf.Amount);
            var amountpaidinstallment = debt.PaymenPlanInstallments.ToList().Where(ppi => ppi.Paid == true).Sum(ppi => ppi.Amount);
            var amounttopayinstallment = debt.InitialAmount - amoundpaidfull - amountpaidinstallment;
            var insterestcalculs = new List<InterestCalculVM>();
            var installments = new List<INSTALLMENT>();
            for (var monthnum = 1; monthnum <= NumOfMonths; monthnum++)
            {
                insterestcalculs.Add(new InterestCalculVM()
                {
                    PaymentDate = startdate.AddMonths(monthnum),
                    InitialAmount = initialamount / NumOfMonths,
                    AmountafterInstallments = initialamount * interest * (startdate.AddMonths(monthnum) - startdate.AddMonths(monthnum - 1)).Days / 36500
                });
            }
            var preview = new previewVM()
            {
                EachMonth = insterestcalculs.Sum(ic => ic.AmountafterInstallments) / NumOfMonths,
                Total = insterestcalculs.Sum(ic => ic.AmountafterInstallments),
                FinishDate = startdate.AddMonths(NumOfMonths),
                InterestRate = interest,
                State = "not decided"
            };
            return View(preview);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> PreviewPaymentPlans(previewVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            else
            { 
                if (vm.State == "Approved")
                {
                    var request = new REQUEST()
                    {
                        NumOfMonths = vm.NumOfMonths,
                        DebtId = vm.DebtId,
                        Status = "not decided"
                    };
                    await _requestService.AddAsync(request);
                }
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region deciderequest
        [HttpPost]
        [Authorize(Roles = "StaffMember")]
        public IActionResult DecideRequest()
        {
            var vm = new DecideRequestVM();
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> DecideRequest(DecideRequestVM vm)
        {
            var request = await _requestService.GetByIdAsync(vm.RequestId);
            await _requestService.UpdateAsync(vm.RequestId, new REQUEST() { NumOfMonths = request.NumOfMonths, Status = vm.decision, DebtId = request.DebtId });
            await GeneratePaymentPlans(new GeneratePaymentPlansVM() { DebtId = request.DebtId, NumOfMonths = request.NumOfMonths });
            return View(vm);
        }
        #endregion

        #region generatepaymentplans
        [HttpPost]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> GeneratePaymentPlans(GeneratePaymentPlansVM gppvm)
        {
            var debt = _debtService.GetByIdAsync(gppvm.DebtId).Result;
            var NumOfMonths = gppvm.NumOfMonths;
            var startdate = debt.StartDate;
            var interest = debt.InterestRate;
            var initialamount = debt.InitialAmount;
            var amoundpaidfull = debt.PaymentPlanFulls.ToList().Sum(ppf => ppf.Amount);
            var amountpaidinstallment = debt.PaymenPlanInstallments.ToList().Where(ppi => ppi.Paid == true).Sum(ppi => ppi.Amount);
            var amounttopayinstallment = debt.InitialAmount - amoundpaidfull - amountpaidinstallment;
            var insterestcalculs = new List<InterestCalculVM>();
            var installments = new List<INSTALLMENT>();
            for (var monthnum = 1; monthnum <= NumOfMonths; monthnum++)
            {
                insterestcalculs.Add(new InterestCalculVM()
                {
                    PaymentDate = startdate.AddMonths(monthnum),
                    InitialAmount = initialamount / NumOfMonths,
                    AmountafterInstallments = initialamount * interest * (startdate.AddMonths(monthnum) - startdate.AddMonths(monthnum - 1)).Days / 36500
                });
            }
            foreach (var ppi in debt.PaymenPlanInstallments.ToList())
            {
                await _paymentplaninstallmentService.DeleteAsync(ppi.Id);
            }
            var paymentinstallment = new PAYMENTPLANINSTALLMENT()
            {
                Amount = debt.InitialAmount,
                Type = "I",
                Paid = false,
                DebtId = gppvm.DebtId,
                NumOfInstallments = NumOfMonths,
                AmountAfterInstallments = insterestcalculs.Sum(ic => ic.AmountafterInstallments),
            };
            await _paymentplaninstallmentService.AddAsync(paymentinstallment);
            foreach (var ic in insterestcalculs)
            {
                await _installmentService.AddAsync(new INSTALLMENT()
                {
                    Amount = paymentinstallment.AmountAfterInstallments / NumOfMonths,
                    Paid = false,
                    SupposedPaymentDate = ic.PaymentDate,
                    PaymentPlanInstallmentId = paymentinstallment.Id
                });
            }
            return View();
        }
        #endregion

        #region getters
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RequestsByDebtAdmin(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> RequestsByDebtStaffMember(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> RequestsByDebtStudent(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RequestsByStudentAdmin(string studentid)
        {
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> RequestsByStudentStaffMember(string studentid)
        {
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RequestsByStaffMemberAdmin(string staffid)
        {
            var staff = await _staffmemberService.GetByIdAsync(staffid);
            var requests = staff.Students.SelectMany(s => s.Debts).Select(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllRequests()
        {
            var requests = await _requestService.GetAllAsync();
            return View(requests);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyRequestsStaffMember()
        {
            var staffid = User.FindFirstValue("Id");
            var staff = await _staffmemberService.GetByIdAsync(staffid);
            var requests = staff.Students.SelectMany(s => s.Debts).SelectMany(d => d.Requests).ToList();
            return View(requests);
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> MyRequestsStudent()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests).ToList();
            return View(requests);
        }
        #endregion
    }
}
