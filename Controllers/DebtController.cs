using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.EntityFrameworkCore;
using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using System.Linq.Expressions;
using Debt_Calculation_And_Repayment_System.Data.Services;
using System.Net.Mail;
using System.Net;
using System.Data;
using Microsoft.Extensions.Hosting;
using NuGet.Packaging.Signing;
using ExcelDataReader;
namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtController : Controller
    {
        private readonly IDEBTService _debtService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly IREQUESTService _requestService;
        private readonly IEMAILTEMPLATEService _emailService;
        IWebHostEnvironment _hostEnvironment;
        IExcelDataReader reader;

        public DebtController(IWebHostEnvironment hostEnvironment,IDEBTService debtService, ISTAFFMEMBERService staffmemberService,IDEBTREGISTERService debtregisterService, ISTUDENTService studentService, IREQUESTService requestService, IEMAILTEMPLATEService emailService)
        {
            _hostEnvironment = hostEnvironment;
            _debtService = debtService;
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _requestService = requestService;
            _emailService = emailService;
        }
        #region getters
        public async Task<IActionResult> DebtsByDebtRegister(string id)
        {
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Student, dr => dr.Debts);
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var staffuser = await _staffmemberService.GetByIdAsync(userid, sm=>sm.Students);
                authorize = staffuser.Students.Select(s => s.Id).ToList().Contains(debtregister.Student.Id);
            }
            else if (User.IsInRole("Student"))
            {
                var userid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(userid);
                authorize = debtregister.Student.Id==student.Id;
            }
            if (authorize)
            {
                var debts = debtregister.Debts.OrderBy(k=>k.StartDate).ToList();
                return View("Debts", debts);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
            }
        }
        #endregion

        #region create debt
        [Authorize(Roles = "Admin, StaffMember")]
        public IActionResult CreateDebt()
        {
            var staffid = User.FindFirstValue("Id");
            var vmstudents = _staffmemberService.GetByIdAsync(staffid, sm => sm.Students).Result.Students.ToList();
            var vm = new CreateDebtVM() 
            { 
                Students = vmstudents
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> CreateDebt(CreateDebtVM debtVM)
        {
            if (ModelState.IsValid)
            {
                var student = await _studentService.GetByIdAsync(debtVM.StudentId, s => s.DebtRegister);
                var debtregister = student.DebtRegister;
                var newdebt = new DEBT()
                {
                    Amount = debtVM.Amount,
                    StartDate = debtVM.StartDate,
                    RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    DebtRegister = debtregister,
                    EndDate = debtVM.EndDate,
                };
                await _debtService.AddAsync(newdebt);

                

                //await SendEmailAfterCreationOfDebt(student.Email);
                var successMessage = "Yeni borç kayıt başarılı.";
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
                var students = await _studentService.GetAllAsync();
                debtVM.Students = students.ToList();
                return View(debtVM);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> ExcelImport(IFormFile fromFiles,string id)
        {
            string path = DocumentUpload(fromFiles);
            string extension = System.IO.Path.GetExtension(fromFiles.FileName);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // read the excel file
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (extension == ".xls")
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet ds = new DataSet();
                ds = reader.AsDataSet();
                reader.Close();
                var depReg =await _debtregisterService.GetByIdAsync(id);
                var student = await _studentService.GetByIdAsync(depReg.StudentId);
                if (ds != null && ds.Tables.Count > 0)
                {
                    // Read the the Table
                    DataTable serviceDetails = ds.Tables[0];
                    for (int i = 0; i < serviceDetails.Rows.Count; i++)
                    {
                        
                       var tutar= serviceDetails.Rows[i][0].ToString();
                       var tarih = serviceDetails.Rows[i][1].ToString();
                        var newdebt = new DEBT()
                        {
                            Amount =DataReader.GetDecimal(tutar),
                            StartDate = DataReader.GetDateTime(tarih),
                            RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                            DebtRegister= depReg
                            

                        };
                        try
                        {
                            await _debtService.AddAsync(newdebt);
                        }
                        catch (Exception e)
                        {

                        }
                       
                        
                        

                    }
                    await DebtRegisterUpdate(depReg.Id);
                    student.Status = "Borcu Girildi";
                    await _studentService.UpdateAsync(student.Id, student);
                    var successMessage = "Yeni borç kayıt başarılı.";
                        return RedirectToAction("IndexParam", "Home", new { successMessage });
                }
            }

            return View();
        }
        #endregion
        public static readonly string[] VALID_EXTENSIONS_EXCEL = new string[2] { ".xls", ".xlsx" };
        public const string UPLOAD_EXCEL = "/Files/Excel/"; 
        private string DocumentUpload(IFormFile fromFiles)
        {
            string extension = System.IO.Path.GetExtension(fromFiles.FileName);
            if (!VALID_EXTENSIONS_EXCEL.Contains(extension.ToLower()))
            {
                return "";
            }
            var uniqName = DateTime.Now.Ticks.ToString();
            var fileName = uniqName + System.IO.Path.GetExtension(fromFiles.FileName);

            var dest_path = System.IO.Path.Combine(_hostEnvironment.WebRootPath+ UPLOAD_EXCEL, fileName);
            using (FileStream filestream = new FileStream(dest_path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return dest_path;
        }
        [Authorize(Roles ="StaffMember, Admin")]
        public async Task<IActionResult> DeleteDebt(string id)
        {
            bool authorize;
            var debt = await _debtService.GetByIdAsync(id, d => d.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(debt.DebtRegister.Id,d=>d.Requests);
            authorize = !debtregister.Requests.Any(r => r.Status == "Onaylı");
            if (authorize)
            {
                var vm = new DeleteDebtVM() { Id = id };
                return View(vm);
            }
            else
            {
                var errorMessage = "Halihazırda kabul edilmiş bir istek varken borcu silemezsiniz";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [Authorize(Roles = "StaffMember, Admin")]
        public async Task<IActionResult> DeleteDebtAll(string id)
        {
            bool authorize;
            var debtReg = await _debtregisterService.GetByIdAsync(id, d => d.Debts);
           
            authorize = !debtReg.Requests.Any(r => r.Status == "Onaylı");
            if (authorize)
            {
                foreach (var r in debtReg.Debts.ToList())
                {
                    await _debtService.DeleteAsync(r.Id);
                }
                await DebtRegisterUpdate(debtReg.Id);
                return RedirectToAction("DebtRegisterById", "DebtRegister", new { id = debtReg.StudentId });
               
            }
            else
            {
                var errorMessage = "Halihazırda kabul edilmiş bir istek varken borcu silemezsiniz";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [HttpPost]
        [Authorize(Roles ="StaffMember, Admin")]
        public async Task<IActionResult> DeleteDebt(DeleteDebtVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Delete)
                {
                    var ds =await _debtService.GetByIdAsync(vm.Id, k => k.DebtRegister);
                    await _debtService.DeleteAsync(vm.Id);
                    await DebtRegisterUpdate(ds.DebtRegister.Id);

                    return RedirectToAction("DebtsByDebtRegister", "Debt", new { id = ds.DebtRegister.Id });
                    //var successMessage = "Borç başarılı bir şekilde silindi" + vm.Id;
                    //return RedirectToAction("IndexParam", "Home", new { successMessage });
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
        private async Task DebtRegisterUpdate(string id)
        {
            var dr = await _debtregisterService.GetByIdAsync(id, dr => dr.Debts, dr => dr.Student);
            var amount = 0m;
            var interestamount = 0m;
            foreach (var d in dr.Debts)
            {
                var ed = dr.Student.ProgramFinishDate;
                var sd = d.StartDate;
                var adda = d.Amount;
                var addia = (d.Amount * (ed - sd).Days * dr.InterestRate) / 365;
                amount += adda;
                interestamount += addia;
            }
            dr.ProgramFinishDate = dr.Student.ProgramFinishDate;
            dr.Amount = decimal.Truncate(amount * 100) / 100;
            dr.InterestAmount = decimal.Truncate(interestamount * 100) / 100;
            dr.TotalCash = decimal.Truncate((interestamount + amount) * 100) / 100;
            dr.ToBePaidCash = decimal.Truncate((interestamount + amount) * 100) / 100;
            dr.ToBePaidInstallment = 0;
            dr.Total = decimal.Truncate((interestamount + amount) * 100) / 100;
            await _debtregisterService.SaveChangesAsync();
           
        }
        public async Task SendEmailAfterCreationOfDebt(string Email)
        {

            var emails = await _emailService.GetAllAsync();
            var emailcontent = emails.FirstOrDefault(e => e.Name == "Borç Eklendi Bildirimi").Content;
            var callbackUrl = Url.Action("MyDebtRegister", "DebtRegister", null, Request.Scheme, Request.Host.ToString());
            var callbackUrl2 = Url.Action("Login", "Account", null, Request.Scheme, Request.Host.ToString());

            var message = new MailMessage();
            message.From = new MailAddress("debtcalculation1@gmail.com", "Bideb Burs Borç Geri ödeme Sistemi");
            message.To.Add(new MailAddress(Email));
            message.Subject = "Yeni borç Tanımlanması";
            message.Body = emailcontent+$"  <a href=\"{callbackUrl}\">here</a>. Not: Önce adresten kimliğinizin doğrulanması gerekir: <a href=\"{callbackUrl2}\"> tıklayınız</a>.";
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
