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
        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                                                  .Select(s => s[random.Next(s.Length)])
                                                  .ToArray());
            return password;
        }
        [Authorize(Roles = "Admin, StaffMember" )]
        public async Task<IActionResult> RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
        [Authorize(Roles = "Admin,StaffMember")]
        [HttpPost]
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
            string password = "Unspecified123@";
            var newUserResponse = await _userManager.CreateAsync(newStudent, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStudent, UserRoles.Student);
                if (result.Succeeded)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(registerVM.Email);
                    mail.From = new MailAddress("debtcalculation1@gmail.com");
                    mail.Subject = "tsiwtsiw";
                    mail.Body = "niwniw";
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerVM);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var passwordValidator = new PasswordValidator<USER>();
            var result = await passwordValidator.ValidateAsync(_userManager, user, changePasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                //errors(result)
                return View(changePasswordVM);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPassword, changePasswordVM.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                //AddErrors(changePasswordResult);
                return View(changePasswordVM);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
