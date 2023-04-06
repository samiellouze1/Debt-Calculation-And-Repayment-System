using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Data.Extensions
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<USER, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<USER> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options): base(userManager,roleManager,options)
        {

        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(USER user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("Id", user.Id ?? ""));
            return identity;
        }
    }
}
