﻿using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Execution;

namespace Debt_Calculation_And_Repayment_System.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (!context.PROGRAMTYPESs.Any())
                {
                    context.PROGRAMTYPESs.AddRange(new List<PROGRAMTYPE>()
                    {
                        new PROGRAMTYPE(){Type="Type1"},
                        new PROGRAMTYPE(){Type="Type2"}

                    });
                    context.SaveChanges();
                }
                if (!context.STUDENTSTATUSTYPEs.Any())
                {
                    context.STUDENTSTATUSTYPEs.AddRange(new List<STUDENTSTATUSTYPE>()
                    {
                        new STUDENTSTATUSTYPE(){Type="Logged in"},
                        new STUDENTSTATUSTYPE(){Type="Waiting"},
                        new STUDENTSTATUSTYPE(){Type="Notified"},
                        new STUDENTSTATUSTYPE(){Type="New Recorded"},
                        new STUDENTSTATUSTYPE(){Type="In Accounting"},
                        new STUDENTSTATUSTYPE(){Type="Refund Decision Canceled"},
                        new STUDENTSTATUSTYPE(){Type="Refund Process Suspended"},
                        new STUDENTSTATUSTYPE(){Type="Referred to Law"}
                    }) ;
                    context.SaveChanges();
                }
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
                        RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
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
                var newUserStaff = new STAFFMEMBER()
                {
                    FirstName = "Staff",
                    SurName = "Member",
                    RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                    UserName = userstaffemail,
                    PhoneNumber = "12345678",
                    Email = userstaffemail,
                    Address = "Turkey"
                };
                if (userstaff == null)
                {

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
                        RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                        PhoneNumber = "12345678",
                        Address = "Turkey",
                        Status="Logged In",
                        DebtRegister = new DEBTREGISTER()
                        {
                            Amount = 3600m,
                            InterestAmount=3262.97m,
                            Total = 4862.97m,
                            InterestRate = 0.09m,
                            ToBePaidCash=4862.97m,
                            RegDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,12,0,0),
                            Debts = new List<DEBT>()
                            {
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2012,12,22,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2012,12,23,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                            },
                        },
                        StaffMember=newUserStaff,
                        StaffMemberAssigned = true,
                        ProgramID="Unspecified"
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
