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

                #region debts
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
                #region PaymentPlan
                if (!context.PAYMENTPLANs.Any())
                {
                    context.PAYMENTPLANFULLs.AddRange(new List<PAYMENTPLANFULL>()
                    {
                        new PAYMENTPLANFULL()
                        {
                            Id = "1",
                            Type= "F",
                            Amount=1500,
                            Paid=true,
                            DebtId="1"
                        }
                    });
                    context.SaveChanges();
                    context.PAYMENTPLANINSTALLMENTs.AddRange(new List<PAYMENTPLANINSTALLMENT>() 
                    { 
                        new PAYMENTPLANINSTALLMENT()
                        {
                            Id = "2",
                            Type= "I",
                            Amount=1500,
                            Paid=false,
                            NumOfInstallments=2,
                            DebtId="1"
                        }
                    });
                    context.SaveChanges();
                }
                #endregion
                #region Installment
                if (!context.INSTALLMENTs.Any())
                {
                    context.INSTALLMENTs.AddRange(new List<INSTALLMENT>()
                    {
                        new INSTALLMENT()
                        {
                            Id = "1",
                            Amount=1000,
                            SupposedPaymentDate=new DateTime(2021,1,1),
                            ActualPaymentDate=new DateTime(2021,1,1),
                            PaymentPlanInstallmentId="2"
                        }
                    });
                    context.SaveChanges();
                    context.INSTALLMENTs.AddRange(new List<INSTALLMENT>()
                    {
                        new INSTALLMENT()
                        {
                            Id = "2",
                            Amount=1000,
                            SupposedPaymentDate=new DateTime(2021,2,1),
                            ActualPaymentDate=new DateTime(2021,2,1),
                            PaymentPlanInstallmentId="2"
                        }
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
                if (useradmin == null)
                {
                    var newUserAdmin = new USER()
                    {
                        Id = "1",
                        FirstName = "Admin",
                        SurName = "User",
                        RegDate = DateTime.Now,
                        UserName = useradminemail,
                        PhoneNumber = "12345678",
                        Email = useradminemail,
                        Address="Turkey"
                    };
                    await userManager.CreateAsync(newUserAdmin, "Adminuser123@");
                    await userManager.AddToRoleAsync(newUserAdmin, UserRoles.Admin);
                }
                #endregion

                #region staffmember
                string userstaffemail = "staff@debt.com";
                var userstaff = await userManager.FindByEmailAsync(userstaffemail);
                if (userstaff==null)
                {
                    var newUserStaff = new STAFFMEMBER()
                    {
                        Id = "2",
                        FirstName = "Staff",
                        SurName = "Member",
                        RegDate = DateTime.Now,
                        UserName = userstaffemail,
                        PhoneNumber = "12345678",
                        Email = userstaffemail,
                        Address = "Turkey"
                    };
                    await userManager.CreateAsync(newUserStaff, "Staffmember123@");
                    await userManager.AddToRoleAsync(newUserStaff, UserRoles.StaffMember);
                }
                #endregion

                #region student
                string userstudentemail = "student@debt.com";
                var userstudent = await userManager.FindByEmailAsync(userstudentemail);
                if (userstudent == null)
                {
                    var newUserStudent = new STUDENT()
                    {
                        Id = "3",
                        FirstName = "Student",
                        SurName = "User",
                        RegDate = DateTime.Now,
                        UserName = userstudentemail,
                        PhoneNumber = "12345678",
                        Email = userstudentemail,
                        StaffMemberId="2",
                        Address = "Turkey"
                    };
                    await userManager.CreateAsync(newUserStudent, "Student123@");
                    await userManager.AddToRoleAsync(newUserStudent, UserRoles.Student);
                }
                #endregion

                #endregion
            }
        }
    }
}
