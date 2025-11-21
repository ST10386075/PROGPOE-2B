using ContractMontlyClaims.Data;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use an in-memory database so the app runs without any external database setup.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ContractMonthlyClaimsDb"));

builder.Services.AddScoped<IContractMonthlyClaimService, EfContractMonthlyClaimService>();
builder.Services.AddScoped<IUserService, EfUserService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Ensure database is created on startup (simple demo approach).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    // Seed default users if none exist yet, so the sample login still works.
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new ApplicationUser
            {
                Id = "LEC001",
                FullName = "Dr. John Smith",
                Email = "john.smith@university.edu",
                Phone = "+27 11 000 0001",
                Department = "Computer Science",
                Role = UserRole.Lecturer,
                Password = "Password123!"
            },
            new ApplicationUser
            {
                Id = "PC001",
                FullName = "Ms. Jane Doe",
                Email = "jane.doe@university.edu",
                Phone = "+27 11 000 0002",
                Department = "Information Technology",
                Role = UserRole.ProgrammeCoordinator,
                Password = "Password123!"
            }
        );

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
