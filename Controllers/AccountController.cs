using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
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
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly AppDbContext _context;
        public AccountController(UserManager<USER> usermanager, SignInManager<USER> signinmanager, AppDbContext context, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService)
        {
            _userManager = usermanager;
            _signInManager = signinmanager;
            _staffmemberService = staffmemberService;
            _context = context;
            _studentService = studentService;
        }

        #region common
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
                Console.WriteLine("c bon");
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
        public async Task<IActionResult> EditUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var vm = new EditVM()
            {
                FirstName = user.FirstName,
                SurName = user.SurName,
            };
            return View(id, vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(string id, EditVM editStudentVM)
        {
            var dbuser = await _userManager.FindByIdAsync(id);

            if (dbuser != null)
            {
                dbuser.FirstName = editStudentVM.FirstName;
                dbuser.SurName = editStudentVM.SurName;
                dbuser.PhoneNumber = editStudentVM.PhoneNumber;
                dbuser.Address = editStudentVM.Address;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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
        #region generatepassword
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
        #endregion

        #region adminstaff
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }
        [Authorize(Roles = "Admin, StaffMember")]
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
                FirstName = registerVM.FirstName,
                SurName = registerVM.SurName,
                RegDate = DateTime.Now,
                Address = "unspecified",
                PhoneNumber = "unspecified",
            };
            string password = GenerateRandomPassword(8);
            var newUserResponse = await _userManager.CreateAsync(newStudent, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStudent, UserRoles.Student);
                if (result.Succeeded)
                {
                    await SendPasswordResetEmail(newStudent.Email);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task SendPasswordResetEmail(string email)
        {
            // Look up the user by email address
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Show an error message to the user
                TempData["ErrorMessage"] = "Invalid email address";
                Console.WriteLine("We couldn't find your account");
                //return RedirectToAction("ForgotPassword", "Account");
            }

            // Generate the password reset link
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new ResetPasswordVM() { Email = user.Email, Code = code }, Request.Scheme);

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
            //return RedirectToAction("Index", "Home");
        }
        #endregion

        #region admin

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAStaffMember()
        {
            var response = new RegisterAStaffMemberVM();
            return View(response);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllStaffMembers(string id)
        {
            var staffmembers = _staffmemberService.GetAllAsync();
            return View(staffmembers);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllStudents(string id)
        {
            var students = _studentService.GetAllAsync();
            return View(students);
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
                RegDate = DateTime.Now,
                Address = registerVM.Address,
                PhoneNumber = registerVM.PhoneNumber,
            };
            string password = registerVM.Password;
            var newUserResponse = await _userManager.CreateAsync(newStaffMember, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStaffMember, UserRoles.StaffMember);
                if (result.Succeeded)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(registerVM.Email);
                    mail.From = new MailAddress("debtcalculation1@gmail.com");
                    mail.Subject = "Password Reset Link";
                    mail.Body = "Dear Student, please click on the following link to reset your password: http://example.com/reset-password?token=abc123"; ;
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

        #endregion

        #region staff
        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyStudents(string id)
        {
            var staffmemberId = User.FindFirstValue("Id");
            var staffmember = _staffmemberService.GetByIdAsync(staffmemberId).Result;
            var students = staffmember.Students.ToList();
            return View(students);
        }

        #endregion

        #region student
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string code)
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
            Console.WriteLine(model.Password);
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

        #endregion

    }
}
