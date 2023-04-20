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
                var requests = debtregister.Requests;
                return View("Requests", requests.OrderBy(r=>r.RegDate).ToList());
            }
            else
            {
                var vm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
                return RedirectToAction("Error", "Home", vm);
            }
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> PreviewRequest ()
        {
            var userid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(userid,s=>s.DebtRegister);

            var vm = new CreateRequestVM() { Total=student.DebtRegister.Total, ToBePaidFull=student.DebtRegister.Total};
            return View("PreviewRequest",vm);
        }
        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> PreviewRequest(CreateRequestVM vm)
        {
            bool authorize;
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid, s => s.DebtRegister);
            var debtregister = await _debtregisterService.GetByIdAsync(student.DebtRegister.Id,d=>d.Requests);
            authorize = !debtregister.Requests.Any(r => r.Status == "Accepted");
            authorize = debtregister.Total == vm.Total;
            authorize = vm.ToBePaidFull >= 0 && vm.ToBePaidInstallment >= 0;
            authorize = vm.ToBePaidFull+vm.ToBePaidInstallment==debtregister.Total;
            authorize = !(vm.ToBePaidInstallment != 0 && vm.NumOfMonths == 0);
            if (authorize)
                {
                if (ModelState.IsValid)
                {
                    var sum = 0m;
                    var tbpi = debtregister.Total - vm.ToBePaidFull;
                    var ia = tbpi/vm.NumOfMonths;
                    var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,12,0,0);
                    for (int i = 0; i < vm.NumOfMonths; i++)
                    {
                        var rpi = tbpi - ia * i;
                        var nod = (today.AddMonths(i) - debtregister.RegDate).Days;
                        var amountafterinterest = ia + nod * rpi * debtregister.InterestRate / 365;
                        sum += amountafterinterest;
                    }
                    var tobepaideachmonth = decimal.Truncate((sum /vm.NumOfMonths)*100)/100 ;
                    var newvm = new PreviewRequestVM() { Total=debtregister.Total,ToBePaidFull = decimal.Truncate(vm.ToBePaidFull*100)/100, NumOfMonths = vm.NumOfMonths, ToBePaidInstallment = decimal.Truncate((debtregister.Total - vm.ToBePaidFull)*100)/100, ToBePaidEachMonth = tobepaideachmonth };
                    return View("CreateRequest", newvm); 
                }
                else
                {
                    return View(vm);
                }
            }
            else
            {
                var vmm = new ErrorViewModel() { ErrorMessage = "You tried to enter a page to which you are not allowed" };
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
            authorize = debtregister.Requests.Any(r => r.Status == "Accepted");
            authorize = debtregister.Total - vm.ToBePaidFull >= 0;
            if (authorize)
            {
                if (ModelState.IsValid)
                {
                    if (vm.Accept)
                    {
                        var newrequest = new REQUEST()
                        {
                            ToBePaidFull = vm.ToBePaidFull,
                            ToBePaidInstallment = debtregister.Total-vm.ToBePaidFull,
                            NumOfMonths = vm.NumOfMonths,
                            RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
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
            else
            {
                var vmm = new ErrorViewModel() { ErrorMessage = "You either tried to make a request when there's already one accepted or you typed the wrong data" };
                return RedirectToAction("Error", "Home", vmm);
            }
        }
    }
}
