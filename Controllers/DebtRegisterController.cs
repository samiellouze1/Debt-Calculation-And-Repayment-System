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
using ExcelDataReader;
using System.Data;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : BaseController
    {
        private readonly ISTUDENTService _studentService;
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly IINSTALLMENTService _installmentService;
        private readonly IPAYMENTService _paymentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IEMAILTEMPLATEService _emailService;
        private readonly IDOCUMENTService _docService;
        private readonly AppDbContext _context;
        IWebHostEnvironment _hostEnvironment;
        public DebtRegisterController(IWebHostEnvironment hostEnvironment, IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService,IINSTALLMENTService installmentService,IPAYMENTService paymentService, ISTAFFMEMBERService staffmemberService,AppDbContext context,IEMAILTEMPLATEService emailService)
        {
            _hostEnvironment = hostEnvironment;
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
            var student = await _studentService.GetByIdAsync(studentid,s=>s.DebtRegister,s=>s.StaffMember,a=>a.ProgramType);
            var debtregister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id, d => d.Requests,d=>d.Payments,d=>d.Installments,d=>d.Debts);
            student.DebtRegister = debtregister;
           // await UpdateDebtRegisterDebtsChanged(debtregister.Id);
            return View("DebtRegister", student);
        }
        public async Task<IActionResult> DebtRegisterById(string id)
        {
            bool authorize = true;
            var student = await _studentService.GetByIdAsync(id, s => s.DebtRegister,student=>student.ProgramType, s => s.Documents);
            if (student.DebtRegister != null)
            {
                student.DebtRegister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id, d => d.Requests, d => d.Student,d=>d.Debts, d => d.Installments, d => d.Payments);
                
            }
            
            
            if (User.IsInRole(UserRoles.StaffMember))
            {
                var staffid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(staffid,sm=>sm.Students);
                var students = staff.Students;
                var studentids = staff.Students.Select(s => s.Id).ToList();
                authorize = studentids.Contains(id);
            }
            else if (User.IsInRole(UserRoles.Student))
            {
                var studentid = User.FindFirstValue("Id");
                authorize = studentid==id;
            }
            if (authorize)
            {
                return View("DebtRegister", student);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız" });
            }
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> Acceptrequest(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            bool authorize = true;
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, dr => dr.Student);

            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(debtregister.Student.Id, s => s.StaffMember);
                authorize = student.StaffMember.Id == userid;
            }
            if (authorize)
            {
                var ent = new CreateRequestVM();
                ent.Total = debtregister.Total;
                ent.ToBePaidFull = request.ToBePaidFull;
                ent.NumOfMonths = request.NumOfMonths;
                ent.ToBePaidInstallment = request.ToBePaidInstallment;
                ent.FirstInstallmentDate = debtregister.FirstInstallmentDate;
                var sonuc = CalculateRequest(ent, debtregister);
                var vm = new PreviewRequestVM()
                {
                    Id = id,
                    ToBePaidFull = request.ToBePaidFull,
                    ToBePaidInstallment = sonuc.ToBePaidInstallment,
                    ToBePaidEachMonth = sonuc.ToBePaidEachMonth,
                    Total = debtregister.Total,
                    NumOfMonths = request.NumOfMonths,
                    Payments=sonuc.Payments,
                    Description=sonuc.Description
                    
                };
            
                return View(vm);
            }
            else
            {
                var errorMessage = "zaten kabul edilmiş bir istek varken bir isteği kabul etmeye çalıştınız";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [HttpPost]
        public  IActionResult Acceptrequest(PreviewRequestVM vm)
        {
            if (ModelState.ErrorCount<2)
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
            //await UpdateDebtRegisterDebtsChanged(request.DebtRegister.Id);
            await UpdateDebtRegisterRequestActivated(id);
            await GenerateInstallments(id);
            await UpdateRequestAfterRequestAccepted(id);
            await UpdateDebtRegisterAfterInstallment(id);
            await GeneratePayments(id);
            await UpdateDebtRegisterAfterGenrationofPayments(id);
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id, d => d.Student);
            var student = await _studentService.GetByIdAsync(debtregister.Student.Id);
            student.Status = "Muhasebede";
            await _context.SaveChangesAsync();
            var successMessage = "İsteği başarıyla kabul ettiniz ";
            return RedirectToAction("IndexParam", "Home", new { successMessage });
        }
       
        #region business
        //[HttpPost]
        //public async Task UpdateDebtRegisterDebtsChanged(string id)
        //{
           
            
        //}
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
            //dr.RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task GenerateInstallments(string id)
        {
            var request = await _requestService.GetByIdAsync(id,r=>r.DebtRegister,r=>r.DebtRegister.Student);
            var debtregister = request.DebtRegister;
            var today = new DateTime(debtregister.FirstInstallmentDate.Value.Year, debtregister.FirstInstallmentDate.Value.Month, debtregister.FirstInstallmentDate.Value.Day);

            //var today = new DateTime(2023, 1, 16, 12, 0, 0);
            var resttopayinstallment = debtregister.TotalInstallment-debtregister.InterestAmount;

            var installment2 = new INSTALLMENT()
            {
                InitialAmount = 0,
                AmountAfterInterest = debtregister.InterestAmount,
                PaymentDate = today,
                NumberOfDays = 0,
                DebtRegister = debtregister,
                Rest = debtregister.Amount
            };
            //resttopayinstallment -= iatable[i - 1];
            await _installmentService.AddAsync(installment2);
            if (resttopayinstallment > 0)
            {
                var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, request.NumOfMonths);
                for (int i = 1; i <= request.NumOfMonths; i++)
                {
                    int nod;
                    if (i >= 1)
                    {
                        nod = (today.AddMonths(i) - today.AddMonths(i - 1)).Days;
                    }
                    else
                    {
                        nod = (today.AddMonths(i) - debtregister.ProgramFinishDate).Days;
                        //nod = 0;
                    }
                    resttopayinstallment -= iatable[i - 1];
                    var xx = Math.Round(nod * resttopayinstallment * debtregister.InterestRateInstallment / 365, 2, MidpointRounding.AwayFromZero);
                    var amountafterinterest = iatable[i-1] +xx ;
                    var installment = new INSTALLMENT()
                    {
                        InitialAmount = iatable[i-1],
                        AmountAfterInterest = amountafterinterest,
                        PaymentDate = today.AddMonths(i),
                        NumberOfDays = nod,
                        DebtRegister = debtregister,
                        Rest = resttopayinstallment
                    };
                    //resttopayinstallment -= iatable[i - 1];
                    await _installmentService.AddAsync(installment);
                }
            }
        }
        [HttpPost]
        public async Task UpdateRequestAfterRequestAccepted(string requestid)
        {
            var request = await _requestService.GetByIdAsync(requestid, dr => dr.DebtRegister);
            request.Status = "Onaylı";
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
            var debtregister = await _debtregisterService.GetByIdAsync(request.DebtRegister.Id,k=>k.Installments);
            var ent = new CreateRequestVM();
            ent.Total = debtregister.Total;
            ent.ToBePaidFull = request.ToBePaidFull;
            ent.NumOfMonths = request.NumOfMonths;
            ent.ToBePaidInstallment = request.ToBePaidInstallment;
            ent.FirstInstallmentDate = debtregister.FirstInstallmentDate;
            
                var sonuc = CalculateRequest(ent, debtregister);

            foreach(var p in sonuc.Payments)
            {
                p.Paid = false;
                p.RegDate=DateTime.Now;
                p.DebtRegister = debtregister;
                await _paymentService.AddAsync(p);
                
            }
            //var resttopayinstallment = debtregister.TotalInstallment - debtregister.InterestAmount;
            //var tbpi = debtregister.ToBePaidInstallment - debtregister.Amount;
            //var payments = new List<PAYMENT>();
            //if (resttopayinstallment>0)
            //{
            //    var interest = Math.Round(tbpi/ request.NumOfMonths,0,MidpointRounding.AwayFromZero);

            //    var principal = Math.Round(debtregister.Amount / request.NumOfMonths,0,MidpointRounding.AwayFromZero);
            //    for (int j=1;j<=debtregister.NumOfMonths;j++)
            //    {
            //        var p = debtregister.Installments.FirstOrDefault(k => k.PaymentDate == new DateTime(DateTime.Now.AddMonths(j).Year, DateTime.Now.AddMonths(j).Month, DateTime.Now.AddMonths(j).Day));
            //        var payment = new PAYMENT() 
            //        { 
            //            Type = "Taksit",
            //            DebtRegister = debtregister,
            //            Sum = p.InitialAmount+interest,
            //            Paid = false,
            //            PaymentDate = new DateTime(DateTime.Now.AddMonths(j).Year, DateTime.Now.AddMonths(j).Month, DateTime.Now.AddMonths(j).Day),
            //            RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
            //            PrincipalAmount = p.InitialAmount,
            //            InterestAmount = interest,
            //        };
            //        await _paymentService.AddAsync(payment);
            //        payments.Add(payment);
            //    }
            //}
            //else
            //{
            //    for (int j=1;j<=debtregister.NumOfMonths;j++)
            //    {
            //        var addons = Math.Round((debtregister.Total-debtregister.TotalCash) / request.NumOfMonths,0,MidpointRounding.AwayFromZero);
            //        var payment = new PAYMENT() {
            //            Type = "Taksit",
            //            DebtRegister = debtregister,
            //            Sum = addons,
            //            Paid = false,
            //            PaymentDate = new DateTime(DateTime.Now.AddMonths(j).Year, DateTime.Now.AddMonths(j).Month,
            //            DateTime.Now.AddMonths(j).Day),
            //            RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
            //            PrincipalAmount=0,
            //            InterestAmount=addons
            //        };
            //        await _paymentService.AddAsync(payment);
            //        payments.Add(payment);
            //    }
            //}
            //if (request.ToBePaidFull!=0)
            //{
            //    var paymentfull = new PAYMENT() { Type = "Peşin", DebtRegister = debtregister, Sum = request.ToBePaidFull, Paid = false, PaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
            //    await _paymentService.AddAsync(paymentfull);
            //    payments.Add(paymentfull);
            //}

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
                var emailcontent = emails.FirstOrDefault(e => e.Name == "Ödeme Hatırlatma.Son 3 Gün");
                var callbackUrl = Url.Action("PaymentsByDebtRegister", "Payment", new { id }, Request.Scheme, Request.Host.ToString());
                var callbackUrl2 = Url.Action("Login", "Account", null, Request.Scheme, Request.Host.ToString());
                var message = new MailMessage();
                message.From = new MailAddress("debtcalculation1@gmail.com", "Debt Calculation and repayment system");
                message.To.Add(new MailAddress(Email));
                message.Subject = "Bideb Borç Hatırlatma:Son 3 gün";
                message.Body = emailcontent + $"  <a href=\"{callbackUrl}\">here</a>. Not: Önce şu adreste kimliğinizin doğrulanması gerekir: <a href=\"{callbackUrl2}\">tıklayınız</a>.";
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
                message.Subject = "Bideb Borç Hatırlatma:Son 1 gün";
                message.Body = emailcontent + $"  <a href=\"{callbackUrl}\">here</a>. Not: Önce şu adreste kimliğinizin doğrulanması gerekir: <a href=\"{callbackUrl2}\">tıklayınız</a>.";
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

        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> DeclineRequest(string id)
        {
            var request = await _requestService.GetByIdAsync(id, r => r.DebtRegister);
            request.Status = "Rejected";
            await _context.SaveChangesAsync();
            var successMessage = "Talep Red Edildi.";
            return RedirectToAction("IndexParam", "Home", new { successMessage });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> ImportFile(IFormFile fromFiles, string id)
        {
            string path = DocumentUpload(fromFiles);
            string extension = System.IO.Path.GetExtension(fromFiles.FileName);
            var student =await _studentService.GetByIdAsync(id);

            var doc = new DOCUMENT()
            {
                Desc = fromFiles.FileName,
                Student = student,
                RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                FileName = path


            };
            try
            {
                await _docService.AddAsync(doc);
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("DebtRegisterById",  new { id= id });
                
            
        }
        public static readonly string[] VALID_EXTENSIONS = new string[1] { ".pdf"};// ".xlsx", ".doc", ".docx" 
        public const string UPLOAD_EXCEL = "/Files/Excel/";
        private string DocumentUpload(IFormFile fromFiles)
        {
            string extension = System.IO.Path.GetExtension(fromFiles.FileName);
            if (!VALID_EXTENSIONS.Contains(extension.ToLower()))
            {
                return "";
            }
            var uniqName = DateTime.Now.Ticks.ToString();
            var fileName = uniqName + System.IO.Path.GetExtension(fromFiles.FileName);

            var dest_path = System.IO.Path.Combine(_hostEnvironment.WebRootPath + UPLOAD_EXCEL, fileName);
            using (FileStream filestream = new FileStream(dest_path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return dest_path;
        }
    }
}
