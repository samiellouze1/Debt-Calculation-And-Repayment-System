using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class RequestController : BaseController
    {
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly AppDbContext _context;

        public RequestController(IREQUESTService requestService, IDEBTREGISTERService debtregisterService,ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService,AppDbContext context)
        {
            _requestService = requestService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
            _context = context;
        }
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> RequestsByStaffMember(string id)
        {
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                authorize = id == User.FindFirstValue("Id");
            }
            if (authorize)
            {
                var staff = await _staffmemberService.GetByIdAsync(id, s => s.Students);
                var students = staff.Students;
                var requests = new List<REQUEST>();
                foreach (var std in students)
                {
                    var actstd = await _studentService.GetByIdAsync(std.Id, s => s.DebtRegister);
                    var debtregister = await _debtregisterService.GetByIdAsync(actstd.DebtRegister.Id, d => d.Requests);
                    requests.AddRange(debtregister.Requests.ToList());
                }
                return View("Requests", requests);
            }
            else
            {
                var errorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> MyRequests()
        {
            var id = User.FindFirstValue("Id");
            var staff = await _staffmemberService.GetByIdAsync(id, s => s.Students);
            var students = staff.Students;
            var requests = new List<REQUEST>();
            foreach (var std in students)
            {
                var actstd = await _studentService.GetByIdAsync(std.Id, s => s.DebtRegister);
                var debtregister = await _debtregisterService.GetByIdAsync(actstd.DebtRegister.Id, d => d.Requests);
                if (debtregister!=null &&debtregister.Requests != null)
                {
                    requests.AddRange(debtregister.Requests.ToList());
                }
                
            }
            return View("Requests", requests);
        }

        public async Task<IActionResult> RequestsByDebtRegister(string id)
        {
            bool authorize = true;
            var debtregister = await _debtregisterService.GetByIdAsync(id, dr => dr.Requests, dr => dr.Student);
            if (User.IsInRole("StaffMember"))
            {
                var userid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(userid, sm=>sm.Students);
                authorize = staff.Students.Select(s => s.Id).ToList().Contains(debtregister.Student.Id);
            }
            if (User.IsInRole("Student"))
            {
                var userid = User.FindFirstValue("Id");
                authorize = debtregister.Student.Id == userid;
            }
            if (authorize)
            {
                var requests = debtregister.Requests;
                return View("Requests", requests.OrderBy(r=>r.RegDate).ToList());
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız" });
            }
        }
        [Authorize(Roles = "Admin,StaffMember,Student")]
        public async Task<IActionResult> PreviewRequest (string userid=null)
        {
            
            if (string.IsNullOrEmpty(userid))
            {
                userid = User.FindFirstValue("Id");
            }
            var student = await _studentService.GetByIdAsync(userid,s=>s.DebtRegister);

            



            var vm = new CreateRequestVM() {FirstInstallmentDate=student.DebtRegister.FirstInstallmentDate, studentId=userid, Total=student.DebtRegister.Total, ToBePaidFull=student.DebtRegister.Total};
            return View("PreviewRequest",vm);
        }
        [Authorize(Roles = "Admin,StaffMember,Student")]
        [HttpPost]
        public async Task<IActionResult> PreviewRequest(CreateRequestVM vm)
        {
            bool authorize;
            //var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(vm.studentId, s => s.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id,d=>d.Requests,d=>d.Student);
            authorize = !debtregister.Requests.Any(r => r.Status == "Onaylı");
            authorize = debtregister.Total == vm.Total && authorize;
            authorize = vm.ToBePaidFull >= 0 && vm.ToBePaidInstallment >= 0 && authorize;
            authorize = vm.ToBePaidFull+vm.ToBePaidInstallment==debtregister.Total && authorize;
            authorize = !(vm.ToBePaidInstallment != 0 && vm.NumOfMonths == 0) && authorize;
            authorize = !(vm.ToBePaidInstallment == 0 && vm.NumOfMonths > 0) && authorize;
            authorize = (!User.IsInRole("Student") || vm.FirstInstallmentDate == debtregister.FirstInstallmentDate) && authorize;
            if (authorize)
                {
                if (ModelState.IsValid)
                {
                    var ss = CalculateRequest(vm, debtregister);
                    return View("CreateRequest", ss);
                }
                else
                {
                    return View(vm);
                }
            }
            else
            {
                var vmm = new ErrorViewModel() { ErrorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız" };
                return RedirectToAction("Error", "Home", vmm);
            }
        }
        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> CreateRequest(PreviewRequestVM vm)
        {
            bool authorize;
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id, d => d.Requests);
            //authorize = debtregister.Requests.Count(r => r.Status == "Onaylı")>0;
            authorize = debtregister.Total - vm.ToBePaidFull >= 0 ;
            debtregister.FirstInstallmentDate = vm.FirstInstallmentDate;
            if (authorize)
            {
                if (ModelState.ErrorCount<3)
                {
                    if (vm.Accept)
                    {
                        var newrequest = new REQUEST()
                        {
                            ToBePaidFull = vm.ToBePaidFull,
                            ToBePaidInstallment = debtregister.Total-vm.ToBePaidFull,
                            NumOfMonths = vm.NumOfMonths,
                            RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                            Status = "Bekliyor",
                            DebtRegister = debtregister,
                        };
                        await _requestService.AddAsync(newrequest);
                        student.Status = "Talep Aşamasında";
                        await _context.SaveChangesAsync();
                        var successMessage = "Talebiniz Gönderildi.Teşekkürler ";
                        return RedirectToAction("IndexParam", "Home", new { successMessage });
                    }
                    else if (!vm.Accept)
                    {
                        return RedirectToAction("Index", "Home");
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
            else
            {
                var vmm = new ErrorViewModel() { ErrorMessage = "Halihazırda kabul edilmiş bir talep varken ya bir talepte bulunmaya çalıştınız ya da yanlış veri girdiniz." };
                return RedirectToAction("Error", "Home", vmm);
            }
        }
       
    }
}
