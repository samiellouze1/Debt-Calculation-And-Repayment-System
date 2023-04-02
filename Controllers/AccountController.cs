using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<USER> _userManager;
        private readonly SignInManager<USER> _signInManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<USER> usermanager,SignInManager<USER> signinmanager,AppDbContext context)
        {
            _userManager = usermanager;
            _signInManager = signinmanager;
            _context = context;
        }
        public IActionResult Login()
        {
            var response = new LoginVM();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginvm)
        {
            if (!ModelState.IsValid)
            {
                return (View(loginvm));
            }
            var user = await _userManager.FindByEmailAsync(loginvm.Email);
            if (user!= null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginvm.Password);
                if (passwordCheck)
                {
                    var result=await _signInManager.PasswordSignInAsync(user, loginvm.Password,false,false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            TempData["Error"] = "Wrong! Try Again";
            return View(loginvm);
        }
        public IActionResult Register()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
        [HttpPost]
        //public async Task<IActionResult> Register(RegisterAStudentVM registerVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(registerVM);
        //    }

        //    var user = await _userManager.FindByEmailAsync(registerVM.Email);
        //    if (user!=null)
        //    {
        //        TempData["Error"] = "This EMail Address has already been taken";
        //        return View(registerVM);
        //    }
        //    var newUser = new USER()
        //    {
        //        UserName=registerVM.Email,
        //        Email=registerVM.Email,
        //        Name=registerVM.Name,
        //        SurName=registerVM.SurName,
        //    };
        //    var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
        //    if (newUserResponse.Succeeded)
        //    {
        //        await _userManager.AddToRoleAsync(newUser, UserRoles.StaffMember);
        //        var result = await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    return View(registerVM);
        //}
        //[HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }
        public async Task<IActionResult> RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
        //[HttpPost]
        public async Task<IActionResult> RegisterPassword()
        {
            var response = new RegisterPasswordVM();
            return View(response);
        }
        //[HttpPost]
    }
}
