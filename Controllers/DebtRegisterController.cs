using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class DebtRegisterController : Controller
    {
        private readonly ISTUDENTService _studentService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly IDEBTREGISTERService _debtregisterService;
        public DebtRegisterController(IDEBTREGISTERService debtregisterService,ISTAFFMEMBERService staffmemberService,ISTUDENTService studentService)
        {
            _staffmemberService = staffmemberService;
            _debtregisterService = debtregisterService;
            _studentService = studentService;
        }
        public async Task<IActionResult> DebtRegisterByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var debtregister = student.DebtRegister;
            return View("DebtRegister", debtregister);
        }
        //public List<INSTALLMENT> GenerateInstallments(string debtid, string requestid)
        //{
        //    var debt = _debtregisterService.GetByIdAsync(debtid).Result;
        //    var request = _requestService.GetByIdAsync(requestid).Result;
        //    var installments = new List<INSTALLMENT>();
        //    var today = DateTime.Now;
        //    for (int i = 0; i < request.NumOfMonths; i++)
        //    {
        //        var ia = debt.InitialAmount / request.NumOfMonths;
        //        var pd = today.AddMonths(i);
        //        var aai = ia * request.InterestRate * (pd - today).Days / 36500;
        //        var installment = new INSTALLMENT()
        //        {
        //            InitialAmount = ia,
        //            AmountAfterInterest = aai,
        //            PaymentDate = pd,
        //        };
        //        installments.Add(installment);
        //    }
        //    return installments;
        //}
    }
}
