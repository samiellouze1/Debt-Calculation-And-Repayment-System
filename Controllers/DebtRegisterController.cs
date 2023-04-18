using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Debt_Calculation_And_Repayment_System.Data.Static;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly ISTUDENTService _studentService;
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly IINSTALLMENTService _installmentService;
        private readonly IPAYMENTService _paymentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService,IINSTALLMENTService installmentService,IPAYMENTService paymentService, ISTAFFMEMBERService staffmemberService)
        {
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
            _installmentService = installmentService;
            _paymentService = paymentService;
            _staffmemberService = staffmemberService;
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> MyDebtRegister()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid,s=>s.DebtRegister,s=>s.StaffMember);
            var debtregister = student.DebtRegister;
            await UpdateDebtRegisterDebtsChanged(debtregister.Id);
            return View("DebtRegister", debtregister);
        }
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            bool authorize = true;
            var debtregister = await _debtregisterService.GetByIdAsync(id,drg=>drg.Student,drg=>drg.Requests,s=>s.Payments,s=>s.Installments,s=>s.Debts);
            if (User.IsInRole(UserRoles.StaffMember))
            {
                var staffid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(staffid,sm=>sm.Students);
                var students = staff.Students;
                var studentids = staff.Students.Select(s => s.Id).ToList();
                authorize = studentids.Contains(debtregister.Student.Id);
            }
            else if (User.IsInRole(UserRoles.Student))
            {
                var studentid = User.FindFirstValue("Id");
                authorize = studentid==debtregister.Student.Id;
            }
            if (authorize)
            {
                return View("DebtRegister", debtregister);
            }
            else
            {
                ViewData["Error"] = "You tried to enter a page to which you are not allowed";
                return RedirectToAction("Error", "Home");
            }
        }
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> AcceptRequest(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Student);
                var student = await _studentService.GetByIdAsync(debtregister.Student.Id,s=>s.StaffMember);
                authorize = student.StaffMember.Id == userid;
            }
            if (authorize)
            {
                await UpdateDebtRegisterDebtsChanged(request.DebtRegister.Id);
                await UpdateDebtRegisterRequestActivated(id);
                var installments = await GenerateInstallments(id);
                await UpdateRequestAfterRequestAccepted(id);
                await UpdateDebtRegisterAfterRequest(id, installments);
                var payments = await GeneratePayments(id);
                await UpdateDebtRegisterAfterGenrationofPayments(id, payments);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Error"] = "You tried to enter a page to which you are not allowed";
                return RedirectToAction("Error", "Home");
            }
        }

        #region business
        public async Task UpdateDebtRegisterDebtsChanged(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id,dr=>dr.Debts);
            var amount = 0m;
            var interestamount = 0m;
            foreach (var d in debtregister.Debts)
            {
                var ed = d.EndDate;
                var sd = d.StartDate;
                var adda = d.Amount;
                var addia = (d.Amount * (ed-sd).Days * debtregister.InterestRate) / 365;
                amount += adda ;
                interestamount += addia;
            }
            var newdebtregister = new DEBTREGISTER()
            {
                Id = id,
                Amount= amount,
                InterestAmount=interestamount,
                Total = interestamount+amount,
                ToBePaid=debtregister.ToBePaid,
                TotalCash=interestamount+amount,
                TotalInstallment=debtregister.TotalInstallment,
                PaidInstallment=0,
                ToBePaidCash=interestamount+amount,
                ToBePaidInstallment=0,
                InterestRate=debtregister.InterestRate,
                RegDate=debtregister.RegDate,
                ReqDate=debtregister.ReqDate,
                Requests=debtregister.Requests,
                Debts=debtregister.Debts,
                Installments=debtregister.Installments,
                Student=debtregister.Student,
                Payments=debtregister.Payments
            };
            await _debtregisterService.UpdateAsync(id, newdebtregister);
        }
        public async Task UpdateDebtRegisterRequestActivated(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregister = request.DebtRegister;
            var tf = request.ToBePaidFull;
            var ti = debtregister.Total - request.ToBePaidFull;

            var newdebtregister = new DEBTREGISTER()
            {
                Id=debtregister.Id,
                InterestAmount=debtregister.InterestAmount,
                Total= debtregister.Total,
                ToBePaid=debtregister.ToBePaid,
                TotalCash=tf,
                TotalInstallment=ti,
                PaidInstallment=debtregister.PaidInstallment,
                ToBePaidCash=tf,
                ToBePaidInstallment=ti,
                InterestRate=debtregister.InterestRate,
                RegDate=debtregister.RegDate,
                ReqDate= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Requests = debtregister.Requests,
                Debts = debtregister.Debts,
                Installments = debtregister.Installments,
                Student = debtregister.Student,
                Payments = debtregister.Payments
            };
            await _debtregisterService.UpdateAsync(request.DebtRegister.Id, newdebtregister);
        }
        public async Task<List<INSTALLMENT>> GenerateInstallments(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregister = request.DebtRegister;
            var installments = new List<INSTALLMENT>();
            var today = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            var ia = debtregister.ToBePaidInstallment / request.NumOfMonths;
            for (int i = 0; i < request.NumOfMonths; i++)
            {
                var resttopayinstallment = debtregister.ToBePaidInstallment - ia * i; 
                var nod = (today.AddMonths(i) - debtregister.RegDate).Days;
                var amountafterinterest = ia  + nod * resttopayinstallment * debtregister.InterestRate / 365;
                Console.WriteLine("Hello World");
                Console.WriteLine("Hello World");
                Console.WriteLine("Hello World");
                Console.WriteLine("Hello World");
                Console.WriteLine("Hello World");
                Console.WriteLine("Hello World");
                Console.WriteLine(resttopayinstallment);
                Console.WriteLine(nod);
                Console.WriteLine(amountafterinterest);
                Console.WriteLine(debtregister.InterestRate);

                var installment = new INSTALLMENT()
                {
                    InitialAmount = ia,
                    AmountAfterInterest = amountafterinterest,
                    PaymentDate= today.AddMonths(i),
                    NumberOfDays = nod,
                    DebtRegister=debtregister
                };
                installments.Add(installment);
            }
            return installments;
        }
        [HttpPost]
        public async Task UpdateRequestAfterRequestAccepted(string requestid)
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
                DebtRegister=request.DebtRegister,
            };
            await _requestService.UpdateAsync(requestid, newrequest);
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterRequest(string id, List<INSTALLMENT> installments)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var dr = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var ti= installments.Select(i => i.AmountAfterInterest).ToList().Sum();
            var tf = request.ToBePaidFull;
            var total = tf + ti;

            var newdebtregister = new DEBTREGISTER()
            {
                Id=dr.Id,
                Amount=dr.Amount,
                InterestAmount = dr.InterestAmount,
                Total=total,
                ToBePaid=total,
                TotalCash=tf,
                TotalInstallment=ti,
                PaidInstallment=dr.PaidInstallment,
                ToBePaidCash=tf,
                ToBePaidInstallment=ti,
                InterestRate = dr.InterestRate,
                RegDate = dr.RegDate,
                ReqDate=dr.ReqDate,
                Payments = dr.Payments,
                Requests = dr.Requests,
                Student = dr.Student,
                Debts = dr.Debts
            };
            await _debtregisterService.UpdateAsync(dr.Id, newdebtregister);
            foreach (var installment in installments)
            {
                await _installmentService.AddAsync(installment);
            }
        }
        public async Task<List<PAYMENT>> GeneratePayments(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var tbpi = debtregister.ToBePaidInstallment;
            var parts = DivideDecimalIntoEqualParts(tbpi, request.NumOfMonths);
            var payments = new List<PAYMENT>();
            var i = 0;
            foreach ( var p in parts)
            {
                var payment = new PAYMENT() { Type="Installment",DebtRegister = debtregister, Sum = p, Paid = false, PaymentDate = new DateTime(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month, DateTime.Now.AddMonths(i).Day), RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
                i += 1;
                payments.Add(payment);
            }
            var paymentfull = new PAYMENT() { Type="Full" ,DebtRegister = debtregister, Sum = request.ToBePaidFull, Paid = false, PaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
            payments.Add(paymentfull);
            return payments;
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterGenrationofPayments(string id,List<PAYMENT> payments)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var dr = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var newdebtregister = new DEBTREGISTER()
            {
                Id = dr.Id,
                InterestAmount = dr.InterestAmount,
                Total = dr.Total,
                ToBePaid = dr.ToBePaidCash,
                TotalCash = dr.TotalCash,
                TotalInstallment = dr.TotalInstallment,
                PaidInstallment = dr.PaidInstallment,
                ToBePaidCash = dr.ToBePaidCash,
                ToBePaidInstallment = dr.ToBePaidInstallment,
                InterestRate = dr.InterestRate,
                RegDate = dr.RegDate,
                ReqDate = dr.ReqDate,
                Installments = dr.Installments,
                Requests = dr.Requests,
                Student = dr.Student,
                Debts = dr.Debts
            };
            await _debtregisterService.UpdateAsync(dr.Id, newdebtregister);
            foreach (var payment in payments)
            {
                await _paymentService.AddAsync(payment);
            }
        }
        public static List<decimal> DivideDecimalIntoEqualParts(decimal x, int n)
        {
            List<decimal> parts = new List<decimal>();
            decimal commonPart = Math.Floor(x / n * 100) / 100; // Calculate the common part with 2 decimal precision
            decimal remaining = x - commonPart * (n - 1); // Calculate the remaining part
            for (int i = 0; i < n - 1; i++)
            {
                parts.Add(commonPart); // Add the common part to the list
            }
            parts.Add(remaining); // Add the remaining part to the list
            return parts;
        }

        #endregion
    }
}
