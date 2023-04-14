using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService,ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService, IREQUESTService requestService)
        {
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
        }
        public async Task<IActionResult> DebtRegisterByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
        public async Task AcceptRequest(string debtregisterid, string requestid)
        {
            var installments = GenerateInstallments(debtregisterid, requestid);
            await UpdateDebtRegisterAfterRequest(debtregisterid, installments);
            var payments = GeneratePayments(debtregisterid);
            await UpdateDebtRegisterAfterGenrationofPayments(debtregisterid, payments);
        }
        public List<INSTALLMENT> GenerateInstallments(string debtregisterid, string requestid)
        {
            var debt = _debtregisterService.GetByIdAsync(debtregisterid).Result;
            var request = _requestService.GetByIdAsync(requestid).Result;
            var installments = new List<INSTALLMENT>();
            var today = DateTime.Now;
            for (int i = 0; i < request.NumOfMonths; i++)
            {
                var ia = debt.TotalInstallment / request.NumOfMonths;
                var pd = today.AddMonths(i);
                var nod = (pd - today).Days;
                var aai = ia * request.InterestRate * nod / 36500;
                var installment = new INSTALLMENT()
                {
                    InitialAmount = ia,
                    AmountAfterInterest = aai,
                    PaymentDate = pd,
                    NumberOfDays=nod,
                    RegDate=DateTime.Now,
                };
                installments.Add(installment);
            }
            return installments;
        }
        public async Task UpdateDebtRegisterAfterRequest(string debtregisterid, List<INSTALLMENT> installments)
        {
            var dr = await _debtregisterService.GetByIdAsync(debtregisterid);
            var tiar = installments.Sum(i => i.AmountAfterInterest);
            var ir = dr.Request.InterestRate;
            var tar = tiar + dr.TotalCash;
            var newdebtregister = new DEBTREGISTER()
            {
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
            };
            await _debtregisterService.UpdateAsync(debtregisterid, newdebtregister);
        }
        public List<PAYMENT> GeneratePayments(string debtregisterid)
        {
            var debtregister = _debtregisterService.GetByIdAsync(debtregisterid).Result;
            var payments = new List<PAYMENT>();
            var nom = debtregister.Request.NumOfMonths;
            var sum = debtregister.TotalAfterRequest/nom;
            var today = DateTime.Now;
            for (int i = 0; i< nom;i++)
            {
                var newpaymentinstallment = new PAYMENT() { Sum= sum,Paid=false,Type="Installment",PaymentDate=today.AddMonths(i),RegDate=today};
                payments.Add(newpaymentinstallment);
            }
            var newpaymentfull= new PAYMENT() { Sum=debtregister.TotalCash,PaymentDate=today,Paid=false,Type="Full",RegDate=today};
            payments.Add(newpaymentfull);
            return payments;
        }
        public async Task UpdateDebtRegisterAfterGenrationofPayments(string debtregisterid,List<PAYMENT> payments)
        {
            var dr = await _debtregisterService.GetByIdAsync(debtregisterid);
            var newdebtregister = new DEBTREGISTER()
            {
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
                Payments = payments
            };
            await _debtregisterService.UpdateAsync(debtregisterid, newdebtregister);
        }
    }
}
