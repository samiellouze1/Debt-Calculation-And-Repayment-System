using Debt_Calculation_And_Repayment_System.Data.Static;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;

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
                        Name = "Admin",
                        SurName = "User",
                        RegDate = DateTime.Now
                    };
                    await userManager.CreateAsync(newUser,"Adminuser123@");
                    await userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                }
                #endregion

                #region staffmember
                #endregion

                #region student
                #endregion
                #endregion

            }
        }
    }
}
