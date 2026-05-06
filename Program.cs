using Microsoft.EntityFrameworkCore;
using TaskManageApp.DAL;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
