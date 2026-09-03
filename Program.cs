using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TaskManageApp.DAL;
using TaskManageApp.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "Web/wwwroot"
});

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Insert(0, "/Web/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Insert(0, "/Web/Views/{1}/{0}.cshtml");
    });

// Register EF-backed repository for database access
builder.Services.AddScoped<TaskManageApp.Repositories.ITaskRepository, TaskManageApp.Repositories.EFTaskRepository>();

// Register ApplicationDbContext (update connection string in appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TaskManageApp")));

builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/access-denied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

await NormalizeLegacyIdentityUsersAsync(app);
await EnsureSeededRolesAsync(app);

var supportedCultures = new[]
{
    new CultureInfo("hr-HR"),
    new CultureInfo("hr"),
    new CultureInfo("en-US")
    ,
    new CultureInfo("en")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("hr-HR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
localizationOptions.RequestCultureProviders.Insert(1, new QueryStringRequestCultureProvider());
localizationOptions.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static async Task NormalizeLegacyIdentityUsersAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var seededUserIds = new[] { 1, 2, 3 };
    var seededUsers = await userManager.Users
        .Where(user => seededUserIds.Contains(user.Id))
        .ToListAsync();

    foreach (var user in seededUsers)
    {
        if (string.IsNullOrWhiteSpace(user.PasswordHash) || !user.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "Password123!");
            user.EmailConfirmed = true;
            user.NormalizedUserName = user.UserName?.ToUpperInvariant();
            user.NormalizedEmail = user.Email?.ToUpperInvariant();
            await userManager.UpdateAsync(user);
        }
    }
}

static async Task EnsureSeededRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var roleNames = new[] { "Admin", "Manager" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }

    var adminUser = await userManager.FindByIdAsync("1");
    var managerUser = await userManager.FindByIdAsync("2");

    if (adminUser != null)
    {
        var currentRoles = await userManager.GetRolesAsync(adminUser);
        if (currentRoles.Any(role => role != "Admin"))
        {
            await userManager.RemoveFromRolesAsync(adminUser, currentRoles.Where(role => role != "Admin").ToArray());
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    if (managerUser != null)
    {
        var currentRoles = await userManager.GetRolesAsync(managerUser);
        if (currentRoles.Any(role => role != "Manager"))
        {
            await userManager.RemoveFromRolesAsync(managerUser, currentRoles.Where(role => role != "Manager").ToArray());
        }

        if (!await userManager.IsInRoleAsync(managerUser, "Manager"))
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");
        }
    }
}
