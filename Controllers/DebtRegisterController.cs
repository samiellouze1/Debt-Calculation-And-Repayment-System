using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Runtime.CompilerServices;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly ISTUDENTService _studentService;
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly IINSTALLMENTService _installmentService;
        private readonly IPAYMENTService _paymentService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService,IINSTALLMENTService installmentService,IPAYMENTService paymentService)
        {
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
            _installmentService = installmentService;
            _paymentService = paymentService;
        }
        public async Task<IActionResult> MyDebtRegister()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid,s=>s.DebtRegister,s=>s.StaffMember);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id,drg=>drg.Requests,drg=>drg.Payments,drg=>drg.Student,d=>d.Installments,d=>d.Debts);
            return View("DebtRegister", debtregister);
        }
        #region business
        public async Task<IActionResult> AcceptRequest(string id)
        {
            var request =await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregisterid = request.DebtRegister.Id;
            var installments = await GenerateInstallments(id);
            await UpdateRequestAfterRequest(id);
            await UpdateDebtRegisterAfterRequest(debtregisterid, installments);
            var payments = await GeneratePayments(debtregisterid);
            await UpdateDebtRegisterAfterGenrationofPayments(debtregisterid, payments);
            return RedirectToAction("Index", "Home");
        }
        public async Task<List<INSTALLMENT>> GenerateInstallments( string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregister = request.DebtRegister;
            var installments = new List<INSTALLMENT>();
            var today = DateTime.Now;
            for (int i = 0; i < request.NumOfMonths; i++)
            {
                var ia = debtregister.TotalInstallment / request.NumOfMonths;
                var pd = today.AddMonths(i);
                var nod = (pd - today).Days;
                var aai = ia * (debtregister.InterestRate * nod / 36500+1);
                var installment = new INSTALLMENT()
                {
                    InitialAmount = ia,
                    AmountAfterInterest = aai,
                    PaymentDate = pd,
                    NumberOfDays=nod,
                    RegDate=DateTime.Now,
                    DebtRegister=debtregister
                };
                installments.Add(installment);
            }
            return installments;
        }
        [HttpPost]
        public async Task UpdateRequestAfterRequest(string requestid)
        {
            var request = await _requestService.GetByIdAsync(requestid, dr => dr.DebtRegister);
            var newrequest = new REQUEST()
            {
                Id=request.Id,
                ToBePaidFull = request.ToBePaidFull,
                ToBePaidInstallment = request.ToBePaidInstallment,
                NumOfMonths = request.NumOfMonths,
                RegDate = request.RegDate,
                Status = "Accepted",
                DebtRegister=request.DebtRegister
            };
            await _requestService.UpdateAsync(requestid, newrequest);
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterRequest(string id, List<INSTALLMENT> installments)
        {
            var dr = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var tiar = installments.Sum(i => i.AmountAfterInterest);
            var ir = dr.InterestRate;
            var tar = tiar + dr.TotalCash;
            foreach (var installment in installments)
            {
                await _installmentService.AddAsync(installment);
            }
            var newdebtregister = new DEBTREGISTER()
            {
                Id=dr.Id,
                Total = dr.Total,
                TotalAfterInterest = dr.TotalAfterInterest,
                TotalAfterRequest = tar,
                TotalCash = dr.TotalCash,
                PaidCash = dr.PaidCash,
                NotPaidCash = dr.NotPaidCash,
                TotalInstallment = dr.TotalInstallment,
                TotalInstallmentAfterRequest = tiar,
                PaidInstallment = dr.PaidInstallment,
                NotPaidInstallment = dr.NotPaidInstallment,
                InterestRate = ir,
                RegDate = dr.RegDate,
                Installments = installments,
                Payments = dr.Payments,
                Requests = dr.Requests,
                Student=dr.Student,
                Debts=dr.Debts,
            };
            await _debtregisterService.UpdateAsync(id, newdebtregister);
        }
        public async Task<List<PAYMENT>> GeneratePayments(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var payments = new List<PAYMENT>();
            var nom = debtregister.Requests.Where(r => r.Status == "Accepted").ToList()[0].NumOfMonths;
            var sum = debtregister.TotalAfterRequest/nom;
            var today = DateTime.Now;
            for (int i = 0; i< nom;i++)
            {
                var newpaymentinstallment = new PAYMENT() { Sum= sum,Paid=false,Type="Installment",PaymentDate=today.AddMonths(i),RegDate=today,DebtRegister=debtregister};
                payments.Add(newpaymentinstallment);
            }
            var newpaymentfull= new PAYMENT() { Sum=debtregister.TotalCash,PaymentDate=today,Paid=false,Type="Full",RegDate=today,DebtRegister=debtregister};
            payments.Add(newpaymentfull);
            return payments;
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterGenrationofPayments(string id,List<PAYMENT> payments)
        {
            var dr = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            foreach (var payment in payments)
            {
                await _paymentService.AddAsync(payment);
            }
            var newdebtregister = new DEBTREGISTER()
            {
                Id = dr.Id,
                Total = dr.Total,
                TotalAfterInterest = dr.TotalAfterInterest,
                TotalAfterRequest = dr.TotalAfterRequest,
                TotalCash = dr.TotalCash,
                PaidCash = dr.PaidCash,
                NotPaidCash = dr.NotPaidCash,
                TotalInstallment = dr.TotalInstallment,
                TotalInstallmentAfterRequest = dr.TotalInstallmentAfterRequest,
                PaidInstallment = dr.PaidInstallment,
                NotPaidInstallment = dr.NotPaidInstallment,
                InterestRate = dr.InterestRate,
                RegDate = dr.RegDate,
                Installments = dr.Installments,
                Payments = payments,
                Debts = dr.Debts,
                Requests=dr.Requests,
                Student=dr.Student
            };
            await _debtregisterService.UpdateAsync(id, newdebtregister);
        }
        #endregion
    }
}
