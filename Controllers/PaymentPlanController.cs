using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class PaymentPlanController : Controller
    {
        private readonly IDEBTService _debtService;
        private readonly IPAYMENTPLANService _paymentplanService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IPAYMENTPLANFULLService _paymentPlanFullService;
        private readonly IPAYMENTPLANINSTALLMENTService _paymentPlanInstallmentService;
        private readonly IINSTALLMENTService _installmentService;
        private readonly ISTUDENTService _studentService;
        public PaymentPlanController(IPAYMENTPLANService paymentplanService, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService, IDEBTService debtService, IPAYMENTPLANFULLService paymentplanfullService, IPAYMENTPLANINSTALLMENTService paymentPlanInstallmentService, IINSTALLMENTService installmentService)
        {
            _paymentplanService = paymentplanService;
            _staffmemberService = staffmemberService;
            _studentService = studentService;
            _debtService = debtService;
            _paymentPlanFullService = paymentplanfullService;
            _paymentPlanInstallmentService = paymentPlanInstallmentService;
            _installmentService = installmentService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task GeneratePaymentPlans(GeneratePaymentPlansVM gppvm)
        {
            var debt = _debtService.GetByIdAsync(gppvm.DebtId).Result;
            var NumOfMonths = gppvm.NumOfMonths;
            var startdate = debt.StartDate;
            var interest = debt.InterestRate;
            var initialamount = debt.InitialAmount;
            var amoundpaidfull = debt.PaymentPlans.Where(pp => pp.Type == "F").ToList().Sum(pp=>pp.Amount);
            var amounttopayinstallment = debt.InitialAmount - amoundpaidfull;
            var insterestcalculs = new List<InterestCalculVM>();
            var installments = new List<INSTALLMENT>();
            for (var monthnum = 1; monthnum<=NumOfMonths; monthnum++)
            {
                insterestcalculs.Add(new InterestCalculVM()
                {
                    PaymentDate= startdate.AddMonths(monthnum),
                    InitialAmount=initialamount/NumOfMonths,
                    AmountafterInstallments=initialamount*interest*(startdate.AddMonths(monthnum)-startdate.AddMonths(monthnum-1)).Days/36500
                });
            }
            var paymentinstallment = new PAYMENTPLANINSTALLMENT()
            {
                Amount = debt.InitialAmount,
                Type = "I",
                Paid = false,
                DebtId = gppvm.DebtId,
                NumOfInstallments = NumOfMonths,
                AmountAfterInstallments = insterestcalculs.Sum(ic => ic.AmountafterInstallments),
            };
            await _paymentPlanInstallmentService.AddAsync(paymentinstallment);
            foreach (var ic in insterestcalculs)
            {
                await _installmentService.AddAsync(new INSTALLMENT()
                {
                    Amount=paymentinstallment.AmountAfterInstallments/NumOfMonths,
                    Paid=false,
                    SupposedPaymentDate=ic.PaymentDate,
                    PaymentPlanInstallmentId=paymentinstallment.Id
                });
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllPaymentPlans()
        {
            var paymentplans = _paymentplanService.GetAllAsync().Result;
            return View(paymentplans);
        }

        #region getters
        public async Task<IActionResult> MyPaymentPlansStudent()
        {
            var studentId = User.FindFirstValue("Id");
            var student = _studentService.GetByIdAsync(studentId).Result;
            var myDebts = student.Debts.ToList();
            var mypaymentplans = new List<PAYMENTPLAN>();
            foreach (var sd in myDebts)
            {
                mypaymentplans.AddRange(sd.PaymentPlans.ToList());
            }
            return View(mypaymentplans);
        }
        public async Task<IActionResult> MyPaymentPlansStaffMember()
        {
            var staffId = User.FindFirstValue("Id");
            var students = _staffmemberService.GetByIdAsync(staffId).Result.Students;
            var paymentplans = new List<PAYMENTPLAN>();
            foreach (var std in students)
            {
                foreach (var sd in std.Debts)
                {
                    paymentplans.AddRange(sd.PaymentPlans);
                }
            }
            return View(paymentplans);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PaymentPlansByDebtAdmin(string id)
        {
            var Debt = _debtService.GetByIdAsync(id).Result;
            var paymentplans = Debt.PaymentPlans.ToList();
            return View(paymentplans);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> PaymentPlansByDebtStaffMember(string id)
        {
            var Debt = _debtService.GetByIdAsync(id).Result;
            var paymentplans = Debt.PaymentPlans;
            return View(paymentplans);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> PaymentPlansByDebtStudent(string id)
        {
            var Debt = _debtService.GetByIdAsync(id).Result;
            var paymentplans = Debt.PaymentPlans;
            return View(paymentplans);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PaymentPlansByStaffMemberAdmin(string id)
        {
            var staffmember = _staffmemberService.GetByIdAsync(id).Result;
            var students = staffmember.Students.ToList();
            var paymentplans = new List<PAYMENTPLAN>();
            foreach (var std in students)
            {
                foreach (var debt in std.Debts.ToList())
                {
                    paymentplans.AddRange(debt.PaymentPlans.ToList());
                }
            }
            return View(paymentplans);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PaymentPlansByStudentAdmin(string id)
        {
            var student = _studentService.GetByIdAsync(id).Result;
            var debts = student.Debts.ToList();
            var paymentplans = new List<PAYMENTPLAN>();
            foreach (var debt in debts)
            {
                paymentplans.AddRange(debt.PaymentPlans.ToList());
            }
            return View(paymentplans);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> PaymentPlansByStudentStaffMember(string id)
        {
            var student = _studentService.GetByIdAsync(id).Result;
            var debts = student.Debts.ToList();
            var paymentplans = new List<PAYMENTPLAN>();
            foreach (var debt in debts)
            {
                paymentplans.AddRange(debt.PaymentPlans.ToList());
            }
            return View(paymentplans);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsByPaymentPlanAdmin(string id)
        {
            var paymentplanfull=_paymentPlanFullService.GetByIdAsync(id).Result;
            return View(paymentplanfull);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> DetailsByPaymentPlanStaffMember(string id)
        {
            var paymentplanfull = _paymentPlanFullService.GetByIdAsync(id).Result;
            return View(paymentplanfull);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DetailsByPaymentPlanStudent(string id)
        {
            var paymentplanfull = _paymentPlanFullService.GetByIdAsync(id).Result;
            return View(paymentplanfull);
        }
        #endregion
    }
}
