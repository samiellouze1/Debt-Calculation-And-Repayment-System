using Debt_Calculation_And_Repayment_System.Controllers;
using Debt_Calculation_And_Repayment_System.Data;
using Debt_Calculation_And_Repayment_System.Data.Extensions;
using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container
builder.Services.AddScoped<IUSERService, USERService>();
builder.Services.AddScoped<ISTAFFMEMBERService, STAFFMEMBERService>();
builder.Services.AddScoped<ISTUDENTService, STUDENTService>();
builder.Services.AddScoped<IPAYMENTService, PAYMENTService>();
builder.Services.AddScoped<IREQUESTService, REQUESTService>();
builder.Services.AddScoped<IDEBTREGISTERService, DEBTREGISTERService>();
builder.Services.AddScoped<IDEBTService, DEBTService>();
builder.Services.AddScoped<IDOCUMENTService, DOCUMENTService>();
builder.Services.AddScoped<IINSTALLMENTService, INSTALLMENTService>();
builder.Services.AddScoped<IPROGRAMTYPEService, PROGRAMTYPEService>();
builder.Services.AddScoped<ISTUDENTSTATUSTYPEService, STUDENTSTATUSTYPEService>();
builder.Services.AddScoped<IEMAILTEMPLATEService, EMAILTEMPLATEService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<USER>, ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddTransient<IUserTwoFactorTokenProvider<USER>, EmailTokenProvider<USER>>();


#region Services related to authentification and authorization
builder.Services.AddIdentity<USER, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
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
builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
//builder.Services.AddHttpsRedirection(options =>
//{
//    options.HttpsPort = 443;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Seeding
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();
AppDbInitializer.Seed(app);

//CultureInfo culture = new CultureInfo("tr-TR");
//Thread.CurrentThread.CurrentCulture = culture;
//Thread.CurrentThread.CurrentUICulture = culture;

var ci = new CultureInfo("en-US");
CultureInfo.CurrentCulture = ci;
CultureInfo.DefaultThreadCurrentCulture = ci;

//Running
app.Run();
