using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NuGet.Protocol;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StudentController : Controller
    {
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly ISTUDENTSTATUSTYPEService _statusService;
        private readonly IPROGRAMTYPEService _programTypeService;
        private readonly IDEBTREGISTERService _deptRegService;
        private readonly AppDbContext _context;

        public StudentController(ISTUDENTService studentService,ISTAFFMEMBERService staffmemberService,IUSERService userService,ISTUDENTSTATUSTYPEService statusService,IPROGRAMTYPEService programTypeService, IDEBTREGISTERService deptRegService, AppDbContext context)
        {
            _studentService = studentService;
            _staffmemberService = staffmemberService;
            _statusService = statusService;
            _programTypeService = programTypeService;
            _deptRegService = deptRegService;
            _context = context;
        }
       
        [Authorize(Roles="Student")]
        public async Task<IActionResult> MyProfile ()
        {
            var id = User.FindFirstValue("Id");
            var student = await _studentService.GetByIdAsync(id);
            return View("Student",student);
        }
        public async Task<IActionResult> StudentById(string id)
        {
            var student = await _studentService.GetByIdAsync(id, s => s.StaffMember, s => s.DebtRegister);
            bool authorize=true;
            if (User.IsInRole("StaffMember"))
            {
                var staffid = User.FindFirstValue("Id");
                var staff = await _staffmemberService.GetByIdAsync(staffid, sm=>sm.Students);
                var staffstudentids = staff.Students.Select(s => s.Id).ToList();
                authorize = staffstudentids.Contains(student.Id);
            }
            if ( User.IsInRole("Student"))
            {
                var studentid = User.FindFirstValue("Id");
                authorize = studentid==id;
            }
            if (authorize)
            {
                return View("Student", student);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız" });
            }
        }
        [Authorize(Roles ="StaffMember,Admin")]
        public async Task<IActionResult> MyStudents(string? message)
        {
            var id = User.FindFirstValue("Id");

            var lstStudents = new List<STUDENT>();


            if (User.IsInRole("StaffMember"))
            {
                var staffMember = await _staffmemberService.GetByIdAsync(id, s => s.Students);

                lstStudents = staffMember.Students;
            }
            if (User.IsInRole("Admin"))
            {
                var students = (await _studentService.GetAllAsync()).ToList();
                lstStudents = students;
            }

            
            foreach (var s in lstStudents)
            {
                var ent = (await _studentService.GetByIdAsync(s.Id, sm => sm.DebtRegister,sm=>sm.ProgramType));
                s.DebtRegister = ent.DebtRegister;
                s.ProgramType = ent.ProgramType;
            }
            if (!string.IsNullOrEmpty(message))
                ViewBag.Message = message;
            await ViewBagSet();
            return View("Students", lstStudents.OrderByDescending(s=>s.RegDate).ToList());
        }
        [HttpPost]
        [Authorize(Roles = "Admin,StaffMember")]
        public async Task<IActionResult> MyStudents(string? filterProgramId,string? filterStatus,string? filterName,string? filterTc, string? filterEmail)
        {
            var id = User.FindFirstValue("Id");
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            var students = staffmember.Students;
            foreach (var s in students)
            {
                var ent = (await _studentService.GetByIdAsync(s.Id, sm => sm.DebtRegister, sm => sm.ProgramType));
                s.DebtRegister = ent.DebtRegister;
                s.ProgramType = ent.ProgramType;
            }
            if (!string.IsNullOrEmpty(filterProgramId))
                students = students.Where(k => k.ProgramType.Id == filterProgramId).ToList();
            if (!string.IsNullOrEmpty(filterStatus))
                students = students.Where(k => k.Status == filterStatus).ToList();
            if (!string.IsNullOrEmpty(filterName))
            {
                filterName = filterName.ToUpper();
                students = students.Where(k => k.FirstName.ToUpper().Contains(filterName) || k.SurName.ToUpper().Contains(filterName)).ToList();
            }
            if (!string.IsNullOrEmpty(filterTc))
                students = students.Where(k => k.IdentityNumber.Contains(filterTc) ).ToList();
            if (!string.IsNullOrEmpty(filterEmail))
                students = students.Where(k => k.Email.Contains(filterEmail)).ToList();
            await ViewBagSet();
            return View("Students", students.OrderByDescending(s => s.RegDate).ToList());
        }


        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> StudentsByStaffMember(string id)
        {
            bool authorize = true;
            if (User.IsInRole("StaffMember"))
            {
                authorize = id == User.FindFirstValue("Id");
            }
            if (authorize)
            {
                var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
                var students = staffmember.Students;
                foreach(var s in students)
                {
                    s.DebtRegister = (await _studentService.GetByIdAsync(s.Id, sm => sm.DebtRegister)).DebtRegister;
                }
                await ViewBagSet();
                return View("Students", students.OrderBy(s=>s.FirstName).ToList());
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "İzin verilmeyen bir sayfaya girmeye çalıştınız" });
            }
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student != null)
            {
                var vm = new DeleteStudentVM() { Id = id };
                return View(vm);
            }
            else
            {
                var errorMessage = "Var olmayan bir bursiyer silmeye çalıştınız";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult DeleteStudent (DeleteStudentVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Delete)
                {
                    return RedirectToAction("DeleteAccount", "Account", new { vm.Id });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(vm);
            }
        }
        [Authorize(Roles ="Admin, StaffMember")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var s = await _studentService.GetByIdAsync(id,s=>s.DebtRegister);
            if (s==null)
            {
                var errorMessage = "Bursiyer bulunamadı";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
            var statuses = await _statusService.GetAllAsync();
            var programs = await _programTypeService.GetAllAsync();
            var vm = new ChangeStatusVM()
            {
                Id = id,
                student = s,
                FirstInstallmentDate = s.DebtRegister != null ? s.DebtRegister.FirstInstallmentDate : null,
                statuses = statuses.ToList(),
                programes = programs.ToList()
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin, StaffMember")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus (ChangeStatusVM vm)
        {

            var student = await _studentService.GetByIdAsync(vm.Id, sm => sm.ProgramType, sm => sm.DebtRegister);
            var program = await _programTypeService.GetByIdAsync(vm.student.ProgramType.Id);
            var deptreg = await _deptRegService.GetByIdAsync(student.DebtRegister.Id);
            student.Status = vm.Type;
            student.PhoneNumber = vm.student.PhoneNumber;
            student.FirstName= vm.student.FirstName;
            student.SurName = vm.student.SurName;
            student.Email=vm.student.Email;
            student.Address=vm.student.Address;
            student.IdentityNumber=vm.student.IdentityNumber;
            student.PhoneNumber = vm.student.PhoneNumber;
            student.Address= vm.student.Address;
            student.GuarantorName= vm.student.GuarantorName;
            student.GuarantorMobile=vm.student.GuarantorMobile;
            student.GuarantorIdentityNumber=vm.student.GuarantorIdentityNumber;
            student.GuarantorAddress=vm.student.GuarantorAddress;

                student.ProgramType = program;
            if ( vm.student.ProgramFinishDate!=DateTime.MinValue)
            {
                student.ProgramFinishDate = vm.student.ProgramFinishDate;
            }
            if (deptreg != null)
            {
                deptreg.InterestRate = program.InterestRate;
                deptreg.InterestRateDelay = program.InterestRateDelay;
                deptreg.InterestRateInstallment = program.InterestRateInstallment;
                if (vm.student.ProgramFinishDate != DateTime.MinValue)
                {
                    deptreg.ProgramFinishDate = vm.student.ProgramFinishDate;
                }
                if (vm.FirstInstallmentDate!=null && vm.FirstInstallmentDate!= DateTime.MinValue)
                {
                    deptreg.FirstInstallmentDate = vm.FirstInstallmentDate;
                }
                student.DebtRegister = deptreg;
            }
               
                await _context.SaveChangesAsync();
                var successMessage = "bursiyerin durumunu başarıyla değiştirdiniz: " + student.FirstName+" "+student.SurName;
                return RedirectToAction("IndexParam", "Home", new { successMessage });
             
             
        }

        public async Task ViewBagSet()
        {
            var programType = (await _programTypeService.GetAllAsync()).ToList();
            
            var status = await _statusService.GetAllAsync();
            //programType.FirstOrDefault().Id

            //var lst = programType.Add(new Models.PROGRAMTYPE() {Type="dsa" });
            ViewBag.ProgramTypes = programType ;
            ViewBag.Statuses = status ;
        }
        
    }
}
