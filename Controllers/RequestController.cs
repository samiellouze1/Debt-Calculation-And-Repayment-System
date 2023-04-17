using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class RequestController : Controller
    {
        private readonly IREQUESTService _requestService;
        private readonly IDEBTREGISTERService _debtregisterService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;

        public RequestController(IREQUESTService requestService, IDEBTREGISTERService debtregisterService,ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService)
        {
            _requestService = requestService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
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
                var request = debtregister.Requests;
                return View("Requests", request);
            }
            else
            {
                ViewData["Error"] = "You tried to enter a page to which you are not allowed";
                return RedirectToAction("Error", "Home");
            }
        }
        [Authorize(Roles ="Student")]
        public IActionResult PreviewRequest ()
        {
            var vm = new CreateRequestVM();
            return View(vm);
        }
        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> PreviewRequest(CreateRequestVM vm)
        {
            if (ModelState.IsValid)
            {
                var studentid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
                var debtregister = student.DebtRegister;
                var sum = 0m;
                for (int i = 1; i <= vm.NumOfMonths; i++)
                {
                    var monthafterinterest = (debtregister.Total - vm.ToBePaidFull) / vm.NumOfMonths;
                    var nod = (new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day).AddMonths(i) - new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day)).Days;
                    var add = monthafterinterest * (1 + nod * debtregister.InterestRate / 365);
                    sum += add;
                }
                var tobepaideachmonth = sum / vm.NumOfMonths;
                var newvm = new PreviewRequestVM() { ToBePaidFull = vm.ToBePaidFull, NumOfMonths = vm.NumOfMonths, ToBePaidInstallment = debtregister.Total - vm.ToBePaidFull, ToBePaidEachMonth = tobepaideachmonth };
                return View("CreateRequest", newvm ); // Change the view here to "PreviewRequestConfirm"
            }
            else
            {
                return View(vm);
            }
        }
        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> CreateRequest(PreviewRequestVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Accept)
                {
                    var studentid = User.FindFirstValue("Id");
                    var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
                    var debtregister = student.DebtRegister;
                    var newrequest = new REQUEST()
                    {
                        ToBePaidFull = vm.ToBePaidFull,
                        ToBePaidInstallment = vm.ToBePaidInstallment,
                        NumOfMonths = vm.NumOfMonths,
                        RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                        Status = "Not Defined",
                        DebtRegister = debtregister,
                    };
                    await _requestService.AddAsync(newrequest);
                    return RedirectToAction("Index", "Home");
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
    }
}
