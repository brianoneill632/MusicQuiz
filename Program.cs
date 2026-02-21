using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicQuiz.Application.Services;
using MusicQuiz.Application.Interfaces;
using MusicQuiz.Core.Data;
using MusicQuiz.Core.Entities;
using MusicQuiz.Core.Migrations;
using System;

var builder = WebApplication.CreateBuilder(args);

// Determine the environment
var environment = builder.Environment;

string msbuildPath = string.Empty;

// Use default MSBuild paths based on environment if the variable is not set
if (environment.IsDevelopment())
{
    msbuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register ApplicationDbContext with environment-specific connection string
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found.");
}

// Check if we are in the Production environment
if (environment.IsProduction())
{
    // Add Key Vault integration only for Production environment
    string keyVaultUrl = builder.Configuration["KeyVault:Url"];
    string secretName = builder.Configuration["KeyVault:SecretName"];

    // Set up Key Vault client
    var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

    // Retrieve the MySQL password from Azure Key Vault
    KeyVaultSecret mysqlPasswordSecret = secretClient.GetSecret(secretName);
    string mysqlPassword = mysqlPasswordSecret.Value;

    // Replace the password in the connection string with the value from Key Vault
    connectionString = connectionString.Replace("{MySQLDbPassword}", mysqlPassword);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21))));

// Add Identity services with roles
builder.Services.AddDefaultIdentity<UserData>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Register services
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<IResultsService, ResultsService>();
builder.Services.AddScoped<UserExpService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddTransient<MusicQuiz.Application.Interfaces.IEmailSender, EmailSender>();

var app = builder.Build();

// Apply migrations and seed data before starting the application
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Apply pending migrations
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // Seed roles and data
        await InitializeRoles(services);
        await SeedAccountData(services);
        await SeedData(services);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred during migration or seeding: {ex.Message}");
        throw; // Rethrow the exception to ensure visibility during development
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Enable session middleware
app.UseSession();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Method to create roles
static async Task InitializeRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = ["Admin", "User"];
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Method to seed Question data
static async Task SeedData(IServiceProvider serviceProvider)
{
    try
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Seeding data...");

        if (!dbContext.QuizQuestions.Any())
        {
            var seedData = QuizQuestionSeedData.GenerateSeedData();
            await dbContext.QuizQuestions.AddRangeAsync(seedData);
            await dbContext.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Method to seed Account data
static async Task SeedAccountData(IServiceProvider serviceProvider)
{
    try
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserData>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        Console.WriteLine("Seeding account data...");

        await AccountSeedData.SeedUserData(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding account data: {ex.Message}");
    }
}
