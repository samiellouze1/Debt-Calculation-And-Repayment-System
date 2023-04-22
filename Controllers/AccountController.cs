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
        private readonly IPROGRAMTYPEService _programTypeService;
        public AccountController(UserManager<USER> usermanager, SignInManager<USER> signinmanager, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService, IUSERService userService, AppDbContext context, IPROGRAMTYPEService programtypeService)
        {
            _userManager = usermanager;
            _signInManager = signinmanager;
            _staffmemberService = staffmemberService;
            _studentService = studentService;
            _userService = userService;
            _context = context;
            _programTypeService = programtypeService;
        }
        #region affectation
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> AffectStudentToStaffMember()
        {
            var allstudents = await _studentService.GetAllAsync(s=>s.StaffMember);
            var students = allstudents.Where(s => s.StaffMember == new STAFFMEMBER());
            var staffmembers = await _staffmemberService.GetAllAsync();
            var vm = new AffectVM() { Students=students.ToList(),StaffMembers=staffmembers.ToList()};
            return View(vm);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> AffectStudentToStaffMember(AffectVM vm)
        {
            if ( ModelState.IsValid)
            {
                var oldstaffmember = await _staffmemberService.GetByIdAsync(vm.StaffMemberId, sm => sm.Students);
                var student = await _studentService.GetByIdAsync(vm.StudentId, s => s.StaffMember, s => s.DebtRegister);
                oldstaffmember.Students.Add(student);
                await _context.SaveChangesAsync();
                var successMessage = "You successfully affected the student"+student.Email+"to the staff member"+oldstaffmember.Email;
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
                var staffmembers = await _staffmemberService.GetAllAsync();
                var students = await _studentService.GetAllAsync();
                var newvm = new AffectVM() {Students=students.ToList(), StaffMembers=staffmembers.ToList() };
                return View(newvm);
            }
        }
        #endregion

        #region normal
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
                    var result = await _signInManager.PasswordSignInAsync(user, loginvm.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (User.IsInRole("Student"))
                        {
                            var userid = User.FindFirstValue("Id");
                            var student = await _studentService.GetByIdAsync(userid);
                            student.Status = "Logged In";
                            await _context.SaveChangesAsync();
                        }
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
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            var successMessage = "you successfully deleted a user";
            return RedirectToAction("IndexParam", "Home", new {successMessage});
        }
        #endregion

        #region special
        public async Task<IActionResult> RegisterAStudent()
        {
            var programtypes =await  _programTypeService.GetAllAsync();
            var vm = new RegisterAStudentVM() { ProgramTypes=programtypes.ToList()};
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAStudent(RegisterAStudentVM registerVM)
        {

            var authorize = registerVM.InterestRate >= 0 && registerVM.InterestRate <= 1;
            if (!ModelState.IsValid)
            {
                var programtypes = await _programTypeService.GetAllAsync();
                registerVM.ProgramTypes = programtypes.ToList();
                ViewData["Error"] = "Wrong Data";
                return View(registerVM);
            }
            if (!authorize)
            {
                var programtypes = await _programTypeService.GetAllAsync();
                registerVM.ProgramTypes = programtypes.ToList();
                ViewData["Error"] = "Wrong data, interest must be between 0 and 1";
                return View(registerVM);
            }
            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                TempData["Error"] = "This Email Address has already been taken";
                return View(registerVM);
            }
            var staffmember = new STAFFMEMBER();
            var staffmemberstatus = false;
            if (User.IsInRole("StaffMember"))
            {
                var staffmemberId = User.FindFirstValue("Id");
                staffmember = await _staffmemberService.GetByIdAsync(staffmemberId);
                staffmemberstatus = true;
            }
            var newStudent = new STUDENT()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                SurName = registerVM.SurName,
                RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Address = registerVM.Address,
                PhoneNumber = registerVM.PhoneNumber,
                StaffMember = staffmember,
                DebtRegister = new DEBTREGISTER() { InterestRate = registerVM.InterestRate },
                StaffMemberAssigned = staffmemberstatus,
                ProgramID=registerVM.ProgramID,
                Status="New Recorded"
            };
            string password = GenerateRandomPassword(8);
            var newUserResponse = await _userManager.CreateAsync(newStudent, password);
            if (newUserResponse.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newStudent, UserRoles.Student);
                if (result.Succeeded)
                {
                    var successMessage = "You successfully added a student ";
                    return RedirectToAction("IndexParam", "Home", new { successMessage });
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
                    var successMessage = "You successfully added a new staff member";
                    return RedirectToAction("IndexParam", "Home", new { successMessage });
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
            message.IsBodyHtml = true;

            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                client.Send(message);
            }
            var student = await _studentService.GetByIdAsync(id);
            if (student!=null)
            {
                student.Status = "Notified";
                await _context.SaveChangesAsync();
            }
            var successMessage = "a reset password link has been sent ";
            return RedirectToAction("IndexParam", "Home", new { successMessage });
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string code)
        {
            // Check if there is an authenticated user
            if (User.Identity.IsAuthenticated)
            {
                var errorMessage = "There is already a user logged in";
                return RedirectToAction("Error", "Home", new {errorMessage});
            }

            // Verify that the email and code are valid
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                var errorMessage = "Your token expired ";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }

            // Create a view model for the password reset form
            var model = new ResetPasswordVM()
            {
                Email = email,
                Code = code
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            // Check if there is an authenticated user
            if (User.Identity.IsAuthenticated)
            {
                var errorMessage = "Your token expired ";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
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
                var successMessage = "Your password has been reset ";
                return RedirectToAction("IndexParam", "Home", new { successMessage });
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
            var successMessage = "a password reset has been sent to your mail ";
            return RedirectToAction("IndexParam", "Home", new { successMessage });
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

        #region SendNormalMail
        public async Task<IActionResult> SendMail()
        {
            var Users = await _userService.GetAllAsync();
            var vm = new SendEmailViewModel() { Users=Users.ToList()};
            return View("SendEmail", vm);
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(SendEmailViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Send the password reset email to the user
                var message = new MailMessage();
                message.From = new MailAddress("debtcalculation1@gmail.com", "Debt Calculation and repayment system");
                message.To.Add(new MailAddress(vm.Email));
                message.Subject = vm.Subject;
                message.Body = vm.Body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("debtcalculation1@gmail.com", "zdsjnyteligoddnd");
                    client.Send(message);
                }

                var successMessage = "You have sent the e-mail";
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
                var errorMessage = "There has been an error";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        #endregion

    }
}
