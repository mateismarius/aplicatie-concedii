using AutoMapper;
using Utilitar.AppContext;
using Utilitar.Helpers.EmployeeHelper;
using Utilitar.Helpers.FreeDayHelper;
using Utilitar.Helpers.ScheduleHelper;
using Utilitar.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utilitar.Areas.Identity.Data;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using FluentValidation;
using Utilitar.Models;
using Utilitar.CustomValidation;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

// Add services to the container.
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new EmployeeProfile());
    cfg.AddProfile(new FreeDayProfile());
});



//builder.Services.AddRazorPages();

var mapper = config.CreateMapper();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UtilitiesContext>(options =>
                    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
builder.Services.AddScoped<IValidator<Employee>, EmployeeValidator>();
builder.Services.AddScoped<IValidator<FreeDay>, FreeDayValidatior>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(mapper);

builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IFreeDayRepository, FreeDayRepository>();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddRazorPages();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<UtilitiesContext>();
var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

try
{
    context.Database.Migrate();
}
catch (Exception ex)
{

    logger.LogError(ex, "Problem migrating data");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

var defaultDateCulture = "ro-RO";
var ci = new CultureInfo(defaultDateCulture);
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";

// Configure the Localization middleware
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo>
    {
        ci,
    },
    SupportedUICultures = new List<CultureInfo>
    {
        ci,
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
