﻿using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class RequestController : Controller
    {
        private readonly IREQUESTService _requestService;
        private readonly ISTUDENTService _studentService;
        private readonly IDEBTService _debtService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        public RequestController(IREQUESTService requestService,ISTUDENTService studentService, ISTAFFMEMBERService staffmemberService, IDEBTService debtService)
        {
            _requestService = requestService;
            _studentService = studentService;
            _staffmemberService = staffmemberService;
            _debtService = debtService;
        }
        [Authorize(Roles ="Student")]
        public IActionResult SendRequest()
        {
            var vm = new SendRequestVM();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SendRequest(SendRequestVM srvm)
        {
            var request = new REQUEST()
            {
                NumOfMonths= srvm.NumOfMonths,
                DebtId=srvm.DebtId,
                Status=false
            };
            await _requestService.AddAsync(request);
            return RedirectToAction("Index", "Home");
        }
        #region getters
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RequestsByDebtAdmin(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles ="StaffMember")]
        public async Task<IActionResult> RequestsByDebtStaffMember(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> RequestsByDebtStudent(string debtid)
        {
            var debt = await _debtService.GetByIdAsync(debtid);
            var requests = debt.Requests;
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RequestsByStudentAdmin(string studentid)
        {
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> RequestsByStudentStaffMember(string studentid)
        {
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RequestsByStaffMemberAdmin(string staffid)
        {
            var staff = await _staffmemberService.GetByIdAsync(staffid);
            var requests = staff.Students.SelectMany(s => s.Debts).Select(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllRequests()
        {
            var requests = await _requestService.GetAllAsync();
            return View(requests);
        }
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyRequestsStaffMember()
        {
            var staffid = User.FindFirstValue("Id");
            var staff = await _staffmemberService.GetByIdAsync(staffid);
            var requests = staff.Students.SelectMany(s => s.Debts).Select(d => d.Requests);
            return View(requests);
        }
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> MyRequestsStudent()
        {
            var studentid = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(studentid);
            var requests = student.Debts.SelectMany(d => d.Requests);
            return View(requests);
        }
        #endregion
    }
}