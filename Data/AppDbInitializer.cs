using Debt_Calculation_And_Repayment_System.Data.Static;
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
                        new PROGRAMTYPE(){Type="2211 Yurt içi Doktora"},
                        new PROGRAMTYPE(){Type="2210 Yurt İçi Yüksek Lisans"}

                    });
                    context.SaveChanges();
                }
                if (!context.STUDENTSTATUSTYPEs.Any())
                {
                    context.STUDENTSTATUSTYPEs.AddRange(new List<STUDENTSTATUSTYPE>()
                    {
                        new STUDENTSTATUSTYPE(){Type="Yeni Kayıt"},
                        new STUDENTSTATUSTYPE(){Type="Borcu Girildi"},
                        new STUDENTSTATUSTYPE(){Type="Bildirim Gönderildi"},
                        new STUDENTSTATUSTYPE(){Type="Giriş Yapıldı"},
                        new STUDENTSTATUSTYPE(){Type="Talep Aşamasında"},
                        new STUDENTSTATUSTYPE(){Type="Muhasebede"},
                        new STUDENTSTATUSTYPE(){Type="Tamamlandı"},

                        new STUDENTSTATUSTYPE(){Type="İade Kararı İptal Edildi"},
                        new STUDENTSTATUSTYPE(){Type="İade İşlemi Askıya Alındı"},
                        new STUDENTSTATUSTYPE(){Type="Mahkemede"}
                    }) ;
                    context.SaveChanges();
                }
                if (!context.EMAILTEMPLATEs.Any())
                {
                    context.EMAILTEMPLATEs.AddRange(new List<EMAILTEMPLATE>()
                    {
                        new EMAILTEMPLATE(){Name="Şifre Sıfırlama", Content="Şifre Sıfırlama Talebi: ",NeedToLogin=false},
                        new EMAILTEMPLATE(){Name="Borç Eklendi Bildirimi", Content="There has been a new debt added. See your debt register ",NeedToLogin=true},
                        new EMAILTEMPLATE(){Name="Ödeme Hatırlatma.Son 3 Gün", Content="Borç Ödemeniz için son 3 gün. Tüm ödemeleleriniz için:",NeedToLogin=true},
                        new EMAILTEMPLATE(){Name="Ödeme Hatırlatma.Son Gün", Content="Borç Ödemeniz için son gün. Tüm ödemeleleriniz için:", NeedToLogin=true}
                    });
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
                string useradminemail = "admin@bideb.gov.tr";
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
                string userstaffemail = "staff@bideb.gov.tr";
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
                string userstudentemail = "student@bideb.gov.tr";
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
                        ProgramFinishDate=new DateTime(2023,1,16),
                        PhoneNumber = "12345678",
                        Address = "Turkey",
                        Status= "Giriş Yapıldı",
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
                                    StartDate=new DateTime(2012,12,6,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2012,12,6,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,2,22,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,2,22,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,3,6,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=400m,
                                    StartDate=new DateTime(2013,4,3,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,4,3,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,5,6,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,6,5,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,7,3,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,8,2,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=400m,
                                    StartDate=new DateTime(2013,8,2,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,9,6,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=1800m,
                                    StartDate=new DateTime(2013,10,4,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=283m,
                                    StartDate=new DateTime(2013,11,7,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2013,12,5,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                },
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2013,12,23,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                                ,
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2014,2,3,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                                ,
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2014,3,5,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                                ,
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2014,4,9,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                                ,
                                new DEBT()
                                {
                                    Amount=400m,
                                    StartDate=new DateTime(2014,4,9,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                                ,
                                new DEBT()
                                {
                                    Amount=500m,
                                    StartDate=new DateTime(2014,5,7,12,0,0),
                                    EndDate=new DateTime(2023,1,16,12,0,0),
                                    RegDate=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                                }
                            },
                        },
                        StaffMember=newUserStaff,
                        StaffMemberAssigned = true,
                        ProgramType=new PROGRAMTYPE {Currency="TL",InterestRate=0.9M,InterestRateDelay=0.9M,InterestRateInstallment=0.9m,Type="2209 Burs" }
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
