using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<USER> _userManager;
        private readonly SignInManager<USER> _signInManager;
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly AppDbContext _context;
        public AccountController(UserManager<USER> usermanager,SignInManager<USER> signinmanager, AppDbContext context, ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService)
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
        public async Task<IActionResult> EditUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var vm = new EditVM()
            {
                FirstName = user.FirstName,
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
                dbuser.FirstName=editStudentVM.FirstName;
                dbuser.SurName=editStudentVM.SurName;
                dbuser.PhoneNumber=editStudentVM.PhoneNumber;
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
        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                                                  .Select(s => s[random.Next(s.Length)])
                                                  .ToArray());
            return password;
        }
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> RegisterAStudent()
        {
            var response = new RegisterAStudentVM();
            return View(response);
        }

        #endregion

        #region adminstaff
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

        #endregion

        #region staffmember

        [Authorize(Roles = "StaffMember")]
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
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentInitializePassword()
        {
            var response = new StudentInitializePasswordVM();
            return View(response);
        }

        #endregion

    }
}
