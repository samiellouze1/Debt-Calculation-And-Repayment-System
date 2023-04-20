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
                return RedirectToAction("Error", "Home", new { errorMessage = "You tried to enter a page to which you are not allowed" });
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
                    var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var amounttopay = debtregister.Amount;
                    var resttopayinstallment = vm.ToBePaidInstallment-debtregister.InterestAmount;
                    var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, vm.NumOfMonths);
                    decimal interestamount = 0m;
                    var whatstays = debtregister.Total-vm.ToBePaidFull;
                    decimal tobepaideachmonth;
                    if (vm.ToBePaidFull < debtregister.Amount)
                    {
                        whatstays = 0;
                        for (int i = 1; i <= vm.NumOfMonths; i++)
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
                            interestamount +=  nod * resttopayinstallment * debtregister.InterestRate / 365;
                            resttopayinstallment -= iatable[i - 1];
                        }
                        tobepaideachmonth = decimal.Truncate((amounttopay/vm.NumOfMonths)*100)/100 + decimal.Truncate((interestamount+debtregister.InterestAmount) / vm.NumOfMonths);
                    }
                    else
                    {
                        tobepaideachmonth = decimal.Truncate(whatstays / vm.NumOfMonths);
                    }
                    var newvm = new PreviewRequestVM() 
                    {
                        Total = debtregister.Total,
                        ToBePaidFull = vm.ToBePaidFull,
                        NumOfMonths = vm.NumOfMonths,
                        ToBePaidInstallment = tobepaideachmonth*vm.NumOfMonths,
                        ToBePaidEachMonth = tobepaideachmonth
                    };
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
                        var successMessage = "You successfully added a new request ";
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
                var vmm = new ErrorViewModel() { ErrorMessage = "You either tried to make a request when there's already one accepted or you typed the wrong data" };
                return RedirectToAction("Error", "Home", vmm);
            }
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
    }
}
