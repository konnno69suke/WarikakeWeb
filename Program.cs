using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarikakeWeb.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WarikakeWebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WarikakeWebContext") ?? throw new InvalidOperationException("Connection string 'WarikakeWebContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".WarikakeWeb.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(1200);   //todo 20•ª
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// SeedData‚ð’Ç‰Á
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// log
app.Logger.LogInformation("Adding Routes");
//app.MapGet("/", () => "Hello World!");
app.Logger.LogInformation("Starting the app");
app.MapGet("/Home", async (ILogger<Program> logger, HttpResponse response) =>
{
    logger.LogInformation("Testing logging in Program.cs");
    await response.WriteAsync("Testing");
});


app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
