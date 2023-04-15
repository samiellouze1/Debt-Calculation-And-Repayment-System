using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();
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
                        FirstName = "Admin",
                        SurName = "User",
                        RegDate = DateTime.Now,
                        UserName = useradminemail,
                        PhoneNumber = "12345678",
                        Email = useradminemail,
                        Address = "Turkey"
                    };
                    await userManager.CreateAsync(newUserAdmin, "Adminuser123@");
                    await userManager.AddToRoleAsync(newUserAdmin, UserRoles.Admin);
                }
                #endregion

                #region staffmember
                string userstaffemail = "staff@debt.com";
                var userstaff = await userManager.FindByEmailAsync(userstaffemail);
                if (userstaff == null)
                {
                    var newUserStaff = new STAFFMEMBER()
                    {
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
                        Email = userstudentemail,
                        UserName = userstudentemail,
                        FirstName = "Student",
                        SurName = "User",
                        RegDate = DateTime.Now,
                        PhoneNumber = "12345678",
                        Address = "Turkey",
                        DebtRegister = new DEBTREGISTER()
                        {
                            Total=300m,
                            TotalAfterInterest=300m,
                            TotalAfterRequest=300m,
                            TotalCash=300m,
                            PaidCash=30m,
                            NotPaidCash=300m,
                            TotalInstallment=300m,
                            TotalInstallmentAfterRequest=300m,
                            PaidInstallment=90m,
                            NotPaidInstallment=50m,
                            InterestRate=0.9m,
                            RegDate=DateTime.Now,
                            Payments = new List<PAYMENT>()
                            {
                                new PAYMENT(){Sum=3000m,Paid=true,Type="Full",PaymentDate=new DateTime(2021,2,1)},
                                new PAYMENT(){Sum=3000m,Paid=true,Type="Installment",PaymentDate=new DateTime(2021,2,1)},
                                new PAYMENT(){Sum=3000m,Paid=true,Type="Full",PaymentDate=new DateTime(2021,2,1)}
                            },
                            Requests=new List<REQUEST>() 
                            { 
                                new REQUEST()
                                {
                                    Status="Declined",
                                    ToBePaidFull=1500m,
                                    ToBePaidInstallment=1500m,
                                    NumOfMonths=30,
                                    InterestRate=0.31m,
                                    RegDate=DateTime.Now
                                } 
                            },
                            Debts = new List<DEBT>()
                            {
                                new DEBT()
                                {
                                    Amount=300m,
                                    StartDate=new DateTime(2021,1,2),
                                    RegDate=DateTime.Now
                                },
                                new DEBT()
                                {
                                    Amount=300m,
                                    StartDate=new DateTime(2021,1,2),
                                    RegDate=DateTime.Now
                                }
                            },
                            Installments = new List<INSTALLMENT>()
                            {
                                new INSTALLMENT()
                                {
                                    InitialAmount=1500m,
                                    AmountAfterInterest=1600m,
                                    PaymentDate=new DateTime(2021,1,1),
                                },
                                new INSTALLMENT()
                                {
                                    InitialAmount=1500m,
                                    AmountAfterInterest=1600m,
                                    PaymentDate=new DateTime(2021,1,1),
                                }
                            }
                        }
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
