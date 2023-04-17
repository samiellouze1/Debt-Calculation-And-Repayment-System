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
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            bool authorize = true;
            var debtregister = await _debtregisterService.GetByIdAsync(id, drg => drg.Requests, drg => drg.Payments, drg => drg.Student, d => d.Installments, d => d.Debts);
            if (User.IsInRole(UserRoles.StaffMember))
            {
                var staffid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(id,sm=>sm.Students);
                var studentids = staff.Students.Select(s => s.Id).ToList();
                authorize = studentids.Contains(debtregister.Student.Id);
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
        #region business
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> AcceptRequest(string id)
        {
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var includeProperties = new Expression<Func<STAFFMEMBER, object>>[]
                {
                        x => x.Students.Select(s => s.DebtRegister.Requests)
                };
                var staffuser = await _staffmemberService.GetByIdAsync(userid,includeProperties);
                authorize = staffuser.Students.Select(s => s.DebtRegister).SelectMany(dr => dr.Requests).Select(r=>r.Id).ToList().Contains(id);
            }
            if (authorize)
            {
                var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
                var debtregisterid = request.DebtRegister.Id;
                await UpdateDebtRegisterDebtsChanged(debtregisterid);
                await UpdateDebtRegisterRequestActivated(id);
                var installments = await GenerateInstallments(id);
                await UpdateRequestAfterRequestAccepted(id);
                await UpdateDebtRegisterAfterRequest(debtregisterid, installments);
                var payments = await GeneratePayments(debtregisterid);
                await UpdateDebtRegisterAfterGenrationofPayments(debtregisterid, payments);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Error"] = "You tried to enter a page to which you are not allowed";
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task UpdateDebtRegisterDebtsChanged(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id,dr=>dr.Debts);
            var total=0m;
            foreach (var d in debtregister.Debts)
            {
                var add = (d.Amount * (1+(d.EndDate - d.StartDate).Days * debtregister.InterestRate)) / 365;
                total += add ;
            }
            var newdebtregister = new DEBTREGISTER()
            {
                Id = id,
                Total = total,
                TotalAfterRequest = debtregister.TotalAfterRequest,
                TotalCash=debtregister.TotalCash,
                PaidCash=debtregister.PaidCash,
                NotPaidCash=debtregister.NotPaidCash,
                TotalInstallment=debtregister.TotalInstallment,
                TotalInstallmentAfterRequest=debtregister.TotalInstallmentAfterRequest,
                PaidInstallment=debtregister.PaidInstallment,
                NotPaidInstallment=debtregister.NotPaidInstallment,
                InterestRate=debtregister.InterestRate,
                RegDate=DateTime.Now,
                Installments = debtregister.Installments,
                Payments = debtregister.Payments,
                Requests = debtregister.Requests,
                Student = debtregister.Student,
                Debts = debtregister.Debts
            };
            await _debtregisterService.UpdateAsync(id, newdebtregister);
        }
        public async Task UpdateDebtRegisterRequestActivated(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregister = request.DebtRegister;
            var tc = request.ToBePaidFull;
            var ti = debtregister.Total - request.ToBePaidFull;

            var newdebtregister = new DEBTREGISTER()
            {
                Id = debtregister.Id,
                Total = debtregister.Total,
                TotalAfterRequest = debtregister.TotalAfterRequest,
                TotalCash = tc,
                PaidCash = debtregister.PaidCash,
                NotPaidCash = debtregister.NotPaidCash,
                TotalInstallment = ti,
                TotalInstallmentAfterRequest = debtregister.TotalInstallmentAfterRequest,
                PaidInstallment = debtregister.PaidInstallment,
                NotPaidInstallment = debtregister.NotPaidInstallment,
                InterestRate = debtregister.InterestRate,
                RegDate = DateTime.Now,
                Installments = debtregister.Installments,
                Payments = debtregister.Payments,
                Requests = debtregister.Requests,
                Student = debtregister.Student,
                Debts = debtregister.Debts
            };
            await _debtregisterService.UpdateAsync(request.DebtRegister.Id, newdebtregister);
        }
        public async Task<List<INSTALLMENT>> GenerateInstallments( string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            Console.WriteLine(request.NumOfMonths);
            var debtregister = request.DebtRegister;
            var installments = new List<INSTALLMENT>();
            var today = DateTime.Now;
            for (int i = 0; i < request.NumOfMonths; i++)
            {
                var ia = debtregister.TotalInstallment / request.NumOfMonths;
                var pd = today.AddMonths(i);
                var nod = (pd - today).Days;
                var aai = ia * (1+debtregister.InterestRate * nod / 365);
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
                DebtRegister=request.DebtRegister
            };
            await _requestService.UpdateAsync(requestid, newrequest);
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterRequest(string id, List<INSTALLMENT> installments)
        {
            var dr = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var tiar = installments.Sum(i => i.AmountAfterInterest);
            var tar = tiar + dr.TotalCash;
            foreach (var installment in installments)
            {
                await _installmentService.AddAsync(installment);
            }
            var newdebtregister = new DEBTREGISTER()
            {
                Id=dr.Id,
                Total = dr.Total,
                TotalAfterRequest = tar,
                TotalCash = dr.TotalCash,
                PaidCash = dr.PaidCash,
                NotPaidCash = dr.NotPaidCash,
                TotalInstallment = dr.TotalInstallment,
                TotalInstallmentAfterRequest = tiar,
                PaidInstallment = dr.PaidInstallment,
                NotPaidInstallment = dr.NotPaidInstallment,
                InterestRate = dr.InterestRate,
                RegDate = dr.RegDate,
                Installments = installments,
                Payments = dr.Payments,
                Requests = dr.Requests,
                Student=dr.Student,
                Debts=dr.Debts
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
