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
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Data;
using Microsoft.EntityFrameworkCore;

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
        private readonly AppDbContext _context;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService,IINSTALLMENTService installmentService,IPAYMENTService paymentService, ISTAFFMEMBERService staffmemberService,AppDbContext context)
        {
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
            _installmentService = installmentService;
            _paymentService = paymentService;
            _staffmemberService = staffmemberService;
            _context = context;
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> MyDebtRegister()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid,s=>s.DebtRegister,s=>s.StaffMember);
            var debtregister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id, d => d.Requests);
            await UpdateDebtRegisterDebtsChanged(debtregister.Id);
            return View("DebtRegister", debtregister);
        }
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            bool authorize = true;
            var debtregister = await _debtregisterService.GetByIdAsync(id,d=>d.Student);
            await UpdateDebtRegisterDebtsChanged(debtregister.Id);
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
                var vm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
                return RedirectToAction("Error", "Home", vm);
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
                await GenerateInstallments(id);
                await UpdateRequestAfterRequestAccepted(id);
                await UpdateDebtRegisterAfterInstallment(id);
                await GeneratePayments(id);
                await UpdateDebtRegisterAfterGenrationofPayments(id);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var vm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
                return RedirectToAction("Error", "Home", vm);
            }
        }

        #region business
        [HttpPost]
        public async Task UpdateDebtRegisterDebtsChanged(string id)
        {
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            var dr = await _debtregisterService.GetByIdAsync(id,dr=>dr.Debts);
            var amount = 0m;
            var interestamount = 0m;
            foreach (var d in dr.Debts)
            {
                var ed = d.EndDate;
                var sd = d.StartDate;
                var adda = d.Amount;
                var addia = (d.Amount * (ed-sd).Days * dr.InterestRate) / 365;
                amount += adda ;
                interestamount += addia;
            }
            dr.Amount = decimal.Truncate(amount*100)/100;
            dr.InterestAmount = decimal.Truncate(interestamount*100)/100;
            dr.TotalCash = decimal.Truncate((interestamount + amount)*100)/100;
            dr.ToBePaidCash = decimal.Truncate((interestamount + amount) * 100) / 100;
            dr.ToBePaidInstallment = 0;
            dr.Total = decimal.Truncate((interestamount + amount) * 100) / 100;
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task UpdateDebtRegisterRequestActivated(string id)
        {
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            Console.WriteLine("Hello update");
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var dr = request.DebtRegister;
            var tf = request.ToBePaidFull;
            var ti = dr.Total - request.ToBePaidFull;
            dr.TotalCash = tf;
            dr.TotalInstallment = ti;
            dr.ToBePaidCash = tf;
            dr.NumOfMonths = request.NumOfMonths;
            dr.ReqDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task GenerateInstallments(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var debtregister = request.DebtRegister;
            var today = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            var resttopayinstallment = debtregister.TotalInstallment-debtregister.InterestAmount;
            if (resttopayinstallment > 0)
            {
                var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, request.NumOfMonths);
                for (int i = 0; i < request.NumOfMonths; i++)
                {
                    int nod;
                    if (i >= 1)
                    {
                        nod = (today.AddMonths(i) - today.AddMonths(i - 1)).Days;
                    }
                    else
                    {
                        nod = (today - debtregister.RegDate).Days;
                    }
                    var amountafterinterest = iatable[i] + nod * resttopayinstallment * debtregister.InterestRate / 365;
                    var installment = new INSTALLMENT()
                    {
                        InitialAmount = iatable[i],
                        AmountAfterInterest = amountafterinterest,
                        PaymentDate = today.AddMonths(i),
                        NumberOfDays = nod,
                        DebtRegister = debtregister,
                        Rest = resttopayinstallment
                    };
                    resttopayinstallment -= iatable[i];
                    await _installmentService.AddAsync(installment);
                }
            }
        }
        [HttpPost]
        public async Task UpdateRequestAfterRequestAccepted(string requestid)
        {
            var request = await _requestService.GetByIdAsync(requestid, dr => dr.DebtRegister);
            request.Status = "Accepted";
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterInstallment(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var dr = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var ti= dr.Installments.Select(i => i.AmountAfterInterest).ToList().Sum()+dr.InterestAmount;
            var tf = request.ToBePaidFull;
            var totaltobepaid = tf + ti;
            dr.ToBePaid = totaltobepaid;
            dr.ToBePaidInstallment = ti;
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task GeneratePayments(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Requests, dr => dr.Payments, dr => dr.Installments, dr => dr.Student, dr => dr.Debts);
            var resttopayinstallment = debtregister.TotalInstallment - debtregister.InterestAmount;
            var tbpi = debtregister.ToBePaidInstallment - debtregister.InterestAmount;
            var i = 0;
            if (resttopayinstallment>0)
            {
                var parts = DivideDecimalIntoEqualParts(tbpi, request.NumOfMonths);
                var addons = decimal.Truncate(debtregister.InterestAmount / request.NumOfMonths);
                var iatable = DivideDecimalIntoEqualParts(debtregister.Amount, request.NumOfMonths); // for principal and interestamount
                foreach (var p in parts)
                {
                    var payment = new PAYMENT() 
                    { 
                        Type = "Installment",
                        DebtRegister = debtregister,
                        Sum = iatable[i] + decimal.Truncate(p + addons - iatable[i]),
                        Paid = false,
                        PaymentDate = new DateTime(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month, DateTime.Now.AddMonths(i).Day),
                        RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        PrincipalAmount = iatable[i],
                        InterestAmount = decimal.Truncate(p + addons - iatable[i])
                    };
                    i += 1;
                    await _paymentService.AddAsync(payment);
                }
            }
            else
            {
                for (int j=0;j<debtregister.NumOfMonths;j++)
                {
                    var addons = decimal.Truncate((debtregister.Total-debtregister.TotalCash) / request.NumOfMonths);
                    var payment = new PAYMENT() {
                        Type = "Installment",
                        DebtRegister = debtregister,
                        Sum = decimal.Truncate(addons),
                        Paid = false,
                        PaymentDate = new DateTime(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month,
                        DateTime.Now.AddMonths(i).Day),
                        RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        PrincipalAmount=0,
                        InterestAmount=decimal.Truncate(addons)
                    };
                    await _paymentService.AddAsync(payment);
                }
            }
            if (request.ToBePaidFull!=0)
            {
                var paymentfull = new PAYMENT() { Type = "Full", DebtRegister = debtregister, Sum = request.ToBePaidFull, Paid = false, PaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
                await _paymentService.AddAsync(paymentfull);
            }
        }
        [HttpPost]
        public async Task UpdateDebtRegisterAfterGenrationofPayments(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister);
            var dr = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id,  dr => dr.Payments);
            dr.ToBePaidInstallment = dr.Payments.Select(p => p.Sum).ToList().Sum();
            dr.ToBePaid= dr.Payments.Select(p => p.Sum).ToList().Sum()+dr.TotalCash;
            await _context.SaveChangesAsync();
        }
        public static decimal[] DivideDecimalIntoEqualParts(decimal x, int n)
        {
            decimal[] parts = new decimal[n];
            decimal commonPart = Math.Floor(x / n * 100) / 100;
            decimal remaining = x - commonPart * (n - 1);
            for (int i = 0; i < n - 1; i++)
            {
                parts[i] = commonPart;
            }
            parts[n - 1] = remaining;
            return parts;
        }

        #endregion
    }
}
