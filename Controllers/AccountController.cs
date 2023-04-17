using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<USER> _userManager;
        private readonly SignInManager<USER> _signInManager;
        private readonly IUSERService _userService;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly AppDbContext _context;
        public AccountController(UserManager<USER> usermanager, SignInManager<USER> signinmanager, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService, IUSERService userService, AppDbContext context)
        {
            _userManager = usermanager;
            _signInManager = signinmanager;
            _staffmemberService = staffmemberService;
            _studentService = studentService;
            _userService = userService;
            _context = context;
        }
        #region affectation
        public async Task<IActionResult> AffectStudentToStaffMember()
        {
            var students = await _studentService.GetAllAsync();
            var staffmembers = await _staffmemberService.GetAllAsync();
            var vm = new AffectVM() { Students=students.ToList(),StaffMembers=staffmembers.ToList()};
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AffectStudentToStaffMember(AffectVM vm)
        {
            try
            {
                var oldstaffmember = await _staffmemberService.GetByIdAsync(vm.StaffMemberId, sm => sm.Students);
                var student = await _studentService.GetByIdAsync(vm.StudentId, s => s.StaffMember, s => s.DebtRegister);
                //var newstaffmember = new STAFFMEMBER() {Id=oldstaffmember.Id, FirstName = oldstaffmember.FirstName, SurName = oldstaffmember.SurName, RegDate=oldstaffmember.RegDate,PhoneNumber=oldstaffmember.PhoneNumber,Address=oldstaffmember.Address,Students=oldstaffmember.Students.Concat(new List<STUDENT>() { student}).ToList()};
                //await _staffmemberService.UpdateAsync(vm.StaffMemberId, newstaffmember);
                oldstaffmember.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }
        #endregion

        #region normal registration
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
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginvm.Password);
                if (passwordCheck)
                {
                    Console.WriteLine("c bon");
                    var result = await _signInManager.PasswordSignInAsync(user, loginvm.Password, false, false);
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
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region specialregistration

        public IActionResult RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
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
            var staffmember = new STAFFMEMBER();
            if (User.IsInRole("StaffMember"))
            {
                var staffmemberId = User.FindFirstValue("Id");
                staffmember = await _staffmemberService.GetByIdAsync(staffmemberId);
            }
            var newStudent = new STUDENT()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                SurName = registerVM.SurName,
                RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                Address = registerVM.Address,
                PhoneNumber = registerVM.PhoneNumber,
                StaffMember=staffmember,
                DebtRegister=new DEBTREGISTER() { InterestRate = registerVM.InterestRate }
            };
            string password = GenerateRandomPassword(8);
            var newUserResponse = await _userManager.CreateAsync(newStudent, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStudent, UserRoles.Student);
                if (result.Succeeded)
                {
                    RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAStaffMember()
        {
            var response = new RegisterAStaffMemberVM();
            return View(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterAStaffMember(RegisterAStaffMemberVM registerVM)
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
            var newStaffMember = new STAFFMEMBER()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                SurName = registerVM.SurName,
                RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                Address = registerVM.Address,
                PhoneNumber = registerVM.PhoneNumber,
            };
            string password = GenerateRandomPassword(8);
            var newUserResponse = await _userManager.CreateAsync(newStaffMember, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStaffMember, UserRoles.StaffMember);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SendPasswordResetEmail(string id)
        {
            // Look up the user by email address
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                // Show an error message to the user
                TempData["ErrorMessage"] = "Invalid email address";
                Console.WriteLine("We couldn't find your account");
                return RedirectToAction("ForgotPassword", "Account");
            }

            // Generate the password reset link
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { user.Email, code }, Request.Scheme);

            // Send the password reset email to the user
            var message = new MailMessage();
            message.From = new MailAddress("debtcalculation1@gmail.com", "Debt Calculation and repayment system");
            message.To.Add(new MailAddress(user.Email, user.UserName));
            message.Subject = "Reset Your Password";
            message.Body = $"Please reset your password by clicking <a href=\"{callbackUrl}\">here</a>.";
            Console.WriteLine(message.Body);
            message.IsBodyHtml = true;

            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                client.Send(message);
            }

            // Show a success message to the user
            TempData["SuccessMessage"] = "A password reset email has been sent to your email address";
            Console.WriteLine("A password reset email has been sent to your email address");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string code)
        {
            // Verify that the email and code are valid
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                TempData["SuccessMessage"] = "Your Token Expired";
                return RedirectToAction("Index", "Home");
            }

            // Create a view model for the password reset form
            var model = new ResetPasswordVM()
            {
                Email = email,
                Code = code
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Look up the user by email address
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email address");
                return View(model);
            }

            // Reset the user's password
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Look up the user by email address
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Show an error message to the user
                TempData["ErrorMessage"] = "Invalid email address";
                return View();
            }

            // Generate the password reset link
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, code = code }, protocol: Request.Scheme);

            // Send the password reset email to the user
            // (code for sending email omitted)

            // Show a success message to the user
            TempData["SuccessMessage"] = "A password reset email has been sent to your email address";
            return RedirectToAction("Index", "Home");
        }
        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            var random = new Random();

            // Ensure that the password meets the conventional requirements
            string password = "";
            do
            {
                password = new string(
                    Enumerable.Repeat(validChars, length)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray()
                );
            } while (!MeetsPasswordRequirements(password));

            return password;
        }
        private static bool MeetsPasswordRequirements(string password)
        {
            const int minLength = 8;
            const int minLowercase = 1;
            const int minUppercase = 1;
            const int minNumeric = 1;
            const int minSpecial = 1;

            // Check the length of the password
            if (password.Length < minLength)
            {
                return false;
            }

            // Check for the presence of lowercase letters
            if (password.Count(c => char.IsLower(c)) < minLowercase)
            {
                return false;
            }

            // Check for the presence of uppercase letters
            if (password.Count(c => char.IsUpper(c)) < minUppercase)
            {
                return false;
            }

            // Check for the presence of numeric characters
            if (password.Count(c => char.IsDigit(c)) < minNumeric)
            {
                return false;
            }

            // Check for the presence of special characters
            if (password.Count(c => !char.IsLetterOrDigit(c)) < minSpecial)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
