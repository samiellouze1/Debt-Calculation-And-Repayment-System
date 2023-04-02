using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<AppDbContext>(option => option.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<IKEYVALUEService, KEYVALUEService>();
builder.Services.AddScoped<IPAYMENTPLANService, PAYMENTPLANService>();
builder.Services.AddScoped<IPAYMENTService, PAYMENTService>();
builder.Services.AddScoped<ISCOLARSHIPDEBTService, SCOLARSHIPDEBTService>();
builder.Services.AddScoped<IUSERService, USERService>();
builder.Services.AddControllersWithViews();

#region Services related to authentification and authorization
builder.Services.AddIdentity<USER, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("Admin",
        authBuilder =>
        {
            authBuilder.RequireRole("Admin");
        });

});
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("StaffMember",
        authBuilder =>
        {
            authBuilder.RequireRole("StaffMember");
        });

});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Student",
        authBuilder =>
        {
            authBuilder.RequireRole("Student");
        });
});
#endregion  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
