using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Drawing2D;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope=applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();

                #region scolarship debts
                if (!context.DEBTs.Any())
                {
                    context.DEBTs.AddRange(new List<DEBT>()
                    {
                        new DEBT() 
                        { 
                            Id="1",
                            InitialAmount=3000, 
                            StartDate= new DateTime(2021,1,1),
                            InterestRate=9m,
                            RegDate=DateTime.Now,
                            StudentId="3"
                        },
                        new DEBT() 
                        { 
                            Id="2",
                            InitialAmount=6000,
                            StartDate= new DateTime(2020,1,1),
                            InterestRate=9m,
                            RegDate=DateTime.Now,
                            StudentId="3"
                        },
                    });
                    context.SaveChanges();
                }
                #endregion
            }
        }
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationbuilder)
        {
            using (var serviceScope = applicationbuilder.ApplicationServices.CreateScope())
            {

                #region roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.StaffMember))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.StaffMember));
                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                #endregion


                #region users

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<USER>>();

                #region admin
                string useradminemail = "admin@debt.com";
                var useradmin = await userManager.FindByEmailAsync(useradminemail);
                if (useradmin==null)
                {
                    var newUser = new USER()
                    {
                        Id = "1",
                        FirstName = "Admin",
                        SurName = "User",
                        RegDate = DateTime.Now,
                        UserName = useradminemail,
                        Address = "unknown",
                        PhoneNumber = "12345678",
                        Email=useradminemail
                    };
                    await userManager.CreateAsync(newUser,"Adminuser123@");
                    await userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                }
                #endregion

                #region staffmember
                string userstaffemail = "staff@debt.com";
                var userstaff = await userManager.FindByEmailAsync(userstaffemail);
                if (userstaff==null)
                {
                    var newUser1 = new STAFFMEMBER()
                    {
                        Id = "2",
                        FirstName = "Staff",
                        SurName = "Member",
                        RegDate = DateTime.Now,
                        UserName = userstaffemail,
                        Address = "Turkey",
                        PhoneNumber = "12345678",
                        Email = userstaffemail
                    };
                    await userManager.CreateAsync(newUser1, "Staffmember123@");
                    await userManager.AddToRoleAsync(newUser1, UserRoles.StaffMember);
                }
                #endregion

                #region student
                string userstudentemail = "student@debt.com";
                var userstudent = await userManager.FindByEmailAsync(userstudentemail);
                if (userstudent == null)
                {
                    var newUser2 = new STUDENT()
                    {
                        Id = "3",
                        FirstName = "Student",
                        SurName = "",
                        RegDate = DateTime.Now,
                        UserName = userstudentemail,
                        Address = "Turkey",
                        PhoneNumber = "12345678",
                        Email = userstudentemail,
                        StaffMemberId="2"
                    };
                    await userManager.CreateAsync(newUser2, "Student123@");
                    await userManager.AddToRoleAsync(newUser2, UserRoles.Student);
                }
                #endregion

                #endregion
            }
        }
    }
}
