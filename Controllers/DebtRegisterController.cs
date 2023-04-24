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
using System.Net.Mail;
using System.Net;

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
        private readonly IEMAILTEMPLATEService _emailService;
        private readonly AppDbContext _context;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService,IINSTALLMENTService installmentService,IPAYMENTService paymentService, ISTAFFMEMBERService staffmemberService,AppDbContext context,IEMAILTEMPLATEService emailService)
        {
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
            _installmentService = installmentService;
            _paymentService = paymentService;
            _staffmemberService = staffmemberService;
            _emailService = emailService;
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
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
            }
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> Acceptrequest(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Student);
                var student = await _studentService.GetByIdAsync(debtregister.Student.Id, s => s.StaffMember);
                authorize = student.StaffMember.Id == userid;
            }
            if (authorize)
            {
                var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Student);
                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var amounttopay = debtregister.Amount;
                var resttopayinstallment = request.ToBePaidInstallment - debtregister.InterestAmount;
                var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, request.NumOfMonths);
                decimal interestamount = 0m;
                var whatstays = debtregister.Total - request.ToBePaidFull;
                decimal tobepaideachmonth;
                if (request.ToBePaidFull < debtregister.Amount)
                {
                    whatstays = 0;
                    for (int i = 1; i <= request.NumOfMonths; i++)
                    {
                        int nod;
                        if (i >= 2)
                        {
                            nod = (today.AddMonths(i) - today.AddMonths(i - 1)).Days;
                        }
                        else
                        {
                            nod = (today.AddMonths(i) - debtregister.RegDate).Days;
                        }
                        interestamount += nod * resttopayinstallment * debtregister.InterestRate / 365;
                        resttopayinstallment -= iatable[i - 1];
                    }
                    try
                    {
                        tobepaideachmonth = decimal.Truncate((amounttopay / request.NumOfMonths) * 100) / 100 + decimal.Truncate((interestamount + debtregister.InterestAmount) / request.NumOfMonths);
                    }
                    catch
                    {
                        tobepaideachmonth = 0m;
                    }
                }
                else
                {
                    try
                    {
                        tobepaideachmonth = decimal.Truncate(whatstays / request.NumOfMonths);
                    }
                    catch
                    {
                        tobepaideachmonth = 0m;
                    }
                }
                var vm = new AcceptRequestVM() {
                    Id = id,
                    ToBePaidFull = request.ToBePaidFull,
                    ToBePaidInstallment = tobepaideachmonth * request.NumOfMonths,
                    ToBePaidEachMonth = tobepaideachmonth,
                    Total= debtregister.Total,
                    NumOfMonths = request.NumOfMonths,
            };
                return View(vm);
            }
            else
            {
                var errorMessage = "you attempted to accept a request while there's one already accepted";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [HttpPost]
        public  IActionResult Acceptrequest(AcceptRequestVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Accept)
                {
                    var id = vm.Id;
                    return RedirectToAction("AcceptRequestConfirm", "DebtRegister", new { id });
                }
                else
                {
                    var id = vm.Id;
                    return RedirectToAction("AcceptRequest", "DebtRegister", new { id });
                }
            }
            else
            {
                var id = vm.Id;
                return RedirectToAction("AcceptRequest", "DebtRegister", new { id });
            }
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> AcceptRequestConfirm(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            await UpdateDebtRegisterDebtsChanged(request.DebtRegister.Id);
            await UpdateDebtRegisterRequestActivated(id);
            await GenerateInstallments(id);
            await UpdateRequestAfterRequestAccepted(id);
            await UpdateDebtRegisterAfterInstallment(id);
            await GeneratePayments(id);
            await UpdateDebtRegisterAfterGenrationofPayments(id);
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, d => d.Student);
            var student = await _studentService.GetByIdAsync(debtregister.Student.Id);
            student.Status = "In Accounting";
            await _context.SaveChangesAsync();
            var successMessage = "You successfully accepted a request ";
            return RedirectToAction("IndexParam", "Home", new { successMessage });
        }
        #region business
        [HttpPost]
        public async Task UpdateDebtRegisterDebtsChanged(string id)
        {
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
            //var today = new DateTime(2023, 1, 16, 12, 0, 0);
            var resttopayinstallment = debtregister.TotalInstallment-debtregister.InterestAmount;
            if (resttopayinstallment > 0)
            {
                var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, request.NumOfMonths);
                for (int i = 1; i <= request.NumOfMonths; i++)
                {
                    int nod;
                    if (i >= 2)
                    {
                        nod = (today.AddMonths(i) - today.AddMonths(i - 1)).Days;
                    }
                    else
                    {
                        nod = (today.AddMonths(i) - debtregister.RegDate).Days;
                        //nod = 0;
                    }
                    var amountafterinterest = iatable[i-1] + nod * resttopayinstallment * debtregister.InterestRate / 365;
                    var installment = new INSTALLMENT()
                    {
                        InitialAmount = iatable[i-1],
                        AmountAfterInterest = amountafterinterest,
                        PaymentDate = today.AddMonths(i),
                        NumberOfDays = nod,
                        DebtRegister = debtregister,
                        Rest = resttopayinstallment
                    };
                    resttopayinstallment -= iatable[i - 1];
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
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id);
            var resttopayinstallment = debtregister.TotalInstallment - debtregister.InterestAmount;
            var tbpi = debtregister.ToBePaidInstallment - debtregister.Amount;
            var payments = new List<PAYMENT>();
            if (resttopayinstallment>0)
            {
                var interest = decimal.Truncate(tbpi/ request.NumOfMonths);
                var principal = decimal.Truncate ((debtregister.Amount / request.NumOfMonths)*100)/100;
                for (int j=1;j<=debtregister.NumOfMonths;j++)
                {
                    var payment = new PAYMENT() 
                    { 
                        Type = "Installment",
                        DebtRegister = debtregister,
                        Sum = principal+interest,
                        Paid = false,
                        PaymentDate = new DateTime(DateTime.Now.AddMonths(j).Year, DateTime.Now.AddMonths(j).Month, DateTime.Now.AddMonths(j).Day),
                        RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        PrincipalAmount = principal,
                        InterestAmount = interest,
                    };
                    await _paymentService.AddAsync(payment);
                    payments.Add(payment);
                }
            }
            else
            {
                for (int j=1;j<=debtregister.NumOfMonths;j++)
                {
                    var addons = decimal.Truncate((debtregister.Total-debtregister.TotalCash) / request.NumOfMonths);
                    var payment = new PAYMENT() {
                        Type = "Installment",
                        DebtRegister = debtregister,
                        Sum = decimal.Truncate(addons),
                        Paid = false,
                        PaymentDate = new DateTime(DateTime.Now.AddMonths(j).Year, DateTime.Now.AddMonths(j).Month,
                        DateTime.Now.AddMonths(j).Day),
                        RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        PrincipalAmount=0,
                        InterestAmount=decimal.Truncate(addons)
                    };
                    await _paymentService.AddAsync(payment);
                    payments.Add(payment);
                }
            }
            if (request.ToBePaidFull!=0)
            {
                var paymentfull = new PAYMENT() { Type = "Full", DebtRegister = debtregister, Sum = request.ToBePaidFull, Paid = false, PaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
                await _paymentService.AddAsync(paymentfull);
                payments.Add(paymentfull);
            }
            foreach ( var payment in payments)
            {
                var idp = payment.Id;
                DateTime targetDateTime = payment.PaymentDate;
                TimeSpan timeToWait3 = targetDateTime - DateTime.Now.AddDays(3);
                TimeSpan timeToWait1 = targetDateTime.AddDays(1) - DateTime.Now;
                await SendPaymentReminderBefore(idp, timeToWait3);
                await SendPaymentReminderAfter(idp,timeToWait1);
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
            try
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
            catch
            {
                return Enumerable.Repeat(0m, n).ToArray();
            }
        }
        #endregion
        public async Task SendPaymentReminderBefore(string idp,TimeSpan timetowait)
        {
            await Task.Delay(timetowait);
            var payment = await _paymentService.GetByIdAsync(idp, payment => payment.DebtRegister);
            if (!payment.Paid)
            {
                var debtregister = await _debtregisterService.GetByIdAsync(payment.DebtRegister.Id, d => d.Student);
                var id = debtregister.Id;
                var Email = debtregister.Student.Email;
                var emails = await _emailService.GetAllAsync();
                var emailcontent = emails.Where(e => e.Name == "Payment in 3 days").ToList()[0];
                var callbackUrl = Url.Action("PaymentsByDebtRegister", "Payment", new { id }, Request.Scheme, Request.Host.ToString());
                var callbackUrl2 = Url.Action("Login", "Account", null, Request.Scheme, Request.Host.ToString());
                var message = new MailMessage();
                message.From = new MailAddress("debtcalculation1@gmail.com", "Debt Calculation and repayment system");
                message.To.Add(new MailAddress(Email));
                message.Subject = "Payment in 3 days";
                message.Body = emailcontent + $"  <a href=\"{callbackUrl}\">here</a>. PS: You need to be authenicated first at <a href=\"{callbackUrl2}\">here</a>.";
                message.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                    client.Send(message);
                }
            }
        }
        public async Task SendPaymentReminderAfter(string idp,TimeSpan timetowait)
        {
            await Task.Delay(timetowait);
            var payment = await _paymentService.GetByIdAsync(idp, payment => payment.DebtRegister);
            if (!payment.Paid)
            {
                var debtregister = await _debtregisterService.GetByIdAsync(payment.DebtRegister.Id, d => d.Student);
                var id = debtregister.Id;
                var Email = debtregister.Student.Email;
                var emails = await _emailService.GetAllAsync();
                var emailcontent = emails.Where(e => e.Name == "Payment 1 day ago").ToList()[0];
                var callbackUrl = Url.Action("PaymentsByDebtRegister", "Payment", new { id }, Request.Scheme, Request.Host.ToString());
                var callbackUrl2 = Url.Action("Login", "Account", null, Request.Scheme, Request.Host.ToString());

                var message = new MailMessage();
                message.From = new MailAddress("debtcalculation1@gmail.com", "Debt Calculation and repayment system");
                message.To.Add(new MailAddress(Email));
                message.Subject = "Payment 1 day ago";
                message.Body = emailcontent + $"  <a href=\"{callbackUrl}\">here</a>. PS: You need to be authenicated first at <a href=\"{callbackUrl2}\">here</a>.";
                message.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                    client.Send(message);
                }
            }
        }
    }
}
