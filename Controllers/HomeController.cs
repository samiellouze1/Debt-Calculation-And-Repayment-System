using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISTUDENTService _studentService;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, ISTUDENTService studentService, AppDbContext context)
        {
            _logger = logger;
            _studentService = studentService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
            {
                var userid = User.FindFirstValue("Id");
                var student = await _studentService.GetByIdAsync(userid);
                if (student.Status == "Bildirim Gönderildi")
                {
                    student.Status = "Giriş Yapıldı";
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("DebtRegisterById", "DebtRegister", new { id = userid });
            }
            if (User.IsInRole("StaffMember") || User.IsInRole("Admin"))
            {
                return RedirectToAction("MyProfile", "StaffMember", new { type =0 });
            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorMessage)
        {
            var vm = new ErrorViewModel() { ErrorMessage = errorMessage };
            return View(vm);
        }
        public IActionResult IndexParam(string successMessage)
        {
            var vm = new SuccessVM() { successMessage = successMessage };
            return View(vm);
        }
    }
}