﻿using Debt_Calculation_And_Repayment_System.Data.IServices;
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
        public async Task<IActionResult> MyDebtRegister()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id);
            return View("DebtRegister", debtregister);
        }
        #region business
        public async Task AcceptRequest(string requestid)
        {
            var request =await _requestService.GetByIdAsync(requestid);
            var debtregisterid = request.DebtRegister.Id;
            var installments = GenerateInstallments(debtregisterid, requestid);
            await UpdateDebtRegisterAfterRequest(debtregisterid, installments);
            await UpdateRequestAfterRequest(requestid);
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
            var ir = dr.Requests.Where(r => r.Status == "Accepted").ToList()[0].InterestRate;
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
        public async Task UpdateRequestAfterRequest(string requestid)
        {
            var request = await _requestService.GetByIdAsync(requestid);
            var newrequest = new REQUEST()
            {
                ToBePaidFull = request.ToBePaidFull,
                ToBePaidInstallment = request.ToBePaidInstallment,
                NumOfMonths = request.NumOfMonths,
                InterestRate = request.InterestRate,
                RegDate = request.RegDate,
                Status = "Accepted"
            };
            await _requestService.UpdateAsync(requestid, newrequest);
        }
        public List<PAYMENT> GeneratePayments(string debtregisterid)
        {
            var debtregister = _debtregisterService.GetByIdAsync(debtregisterid).Result;
            var payments = new List<PAYMENT>();
            var nom = debtregister.Requests.Where(r => r.Status == "Accepted").ToList()[0].NumOfMonths;
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
        #endregion
    }
}
