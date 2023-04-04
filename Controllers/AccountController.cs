using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

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
                Console.WriteLine("c bon");
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginvm.Password);
                if (passwordCheck)
                {
                    Console.WriteLine("c bon");
                    var result =await _signInManager.PasswordSignInAsync(user, loginvm.Password,false,false);
                    if (result.Succeeded)
                    {
                        Console.WriteLine("c bon");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            TempData["Error"] = "Wrong! Try Again";
            return View(loginvm);
        }
        [Authorize(UserRoles.Admin)]
        [Authorize(UserRoles.StaffMember)]
        public async Task<IActionResult> RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                                                  .Select(s => s[random.Next(s.Length)])
                                                  .ToArray());
            return password;
        }
        [HttpPost]
        [Authorize(UserRoles.Admin)]
        [Authorize(UserRoles.StaffMember)]
        public async Task<IActionResult> RegisterAStudent(RegisterAStudentVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                TempData["Error"] = "This Email Address has already been taken";
                return View(registerVM);
            }
            var newStudent = new STUDENT()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                RegDate = DateTime.Now,
                Address = "unspecified",
                PhoneNumber = "unspecified",
            };
            string password = GenerateRandomPassword(8);
            var newUserResponse = await _userManager.CreateAsync(newStudent, password);
            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newStudent, UserRoles.StaffMember);
                var result = await _signInManager.PasswordSignInAsync(newStudent, password, false, false);
                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }
        [Authorize(UserRoles.Student)]
        public async Task<IActionResult> ModifyPassword()
        {
            var response = new ModifyPasswordVM();
            return View(response);
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var vm = new EditVM()
            {
                Name = user.Name,
                SurName=user.SurName,

            };
            return View(id, vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(string id,EditVM editStudentVM)
        {
            var dbuser = await _userManager.FindByIdAsync(id);

            if (dbuser != null)
            {
                dbuser.Name=editStudentVM.Name;
                dbuser.SurName=editStudentVM.SurName;
                dbuser.PhoneNumber=editStudentVM.PhoneNumber;
                dbuser.Address = editStudentVM.Address;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
            
    }
}
