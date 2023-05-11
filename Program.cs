using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Data;
using ExtensioProcuratio.Helper;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using ExtensioProcuratio.Repositories.Interface;
using ExtensioProcuratio.Repositories.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Dev") ?? throw new InvalidOperationException("Connection string 'Dev' not found.");

//Database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<EpDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<EpDbContext>();

//Interfaces
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserHelper, UserHelper>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
//Settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();