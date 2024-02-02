using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
    options.IdleTimeout = TimeSpan.FromSeconds(1200);   //todo 20分
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// serilogを使用する宣言
builder.Host.UseSerilog();

// cookie認証を使用する
builder.Services.Configure<CookiePolicyOptions>(options => {
    options.CheckConsentNeeded = context => !context.User.Identity.IsAuthenticated;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Home/Login";
    options.AccessDeniedPath = "/Home/Denied";
});

var app = builder.Build();

// SeedDataを追加
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Denied");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

// cookie認証
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// serilogの設定は設定ファイルを参照する宣言
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(app.Configuration).CreateLogger();

app.Run();
