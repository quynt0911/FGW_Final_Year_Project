// using Blank.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Identity.UI;
// using Microsoft.AspNetCore.Identity.UI.Services;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllersWithViews();

// var connectionTestDbConnection = builder.Configuration.GetConnectionString("MyConnect");

// builder.Services.AddDbContext<FinalprojectContext>(options =>
//     options.UseSqlServer(connectionTestDbConnection));

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<FinalprojectContext>();

// builder.Services.AddDbContext<FinalprojectContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
// {
//     options.SignIn.RequireConfirmedAccount = true;
//     options.Lockout.AllowedForNewUsers = false;  // Tắt khóa tài khoản cho người dùng mới
//     options.Lockout.MaxFailedAccessAttempts = 5; // Nếu cần, tăng số lần thất bại trước khi khóa
//     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Thời gian khóa tài khoản
// });

// builder.Services.Configure<IdentityOptions>(options =>
// {
//     options.User.RequireUniqueEmail = true; // Đảm bảo rằng Email là duy nhất
//     options.SignIn.RequireConfirmedEmail = false; // Tùy chọn nếu bạn chưa muốn xác nhận email
// });

// builder.Services.AddTransient<IEmailSender, EmailSender>();

// builder.Services.AddControllersWithViews();

// var app = builder.Build();


// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();


// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// app.Run();


// using Blank.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Identity.UI;
// using Microsoft.AspNetCore.Identity.UI.Services;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages();

// var connectionTestDbConnection = builder.Configuration.GetConnectionString("MyConnect");

// builder.Services.AddDbContext<FinalprojectContext>(options =>
//     options.UseSqlServer(connectionTestDbConnection));


// builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
// {
//     options.SignIn.RequireConfirmedAccount = true;
//     options.Lockout.AllowedForNewUsers = false;  // Tắt khóa tài khoản cho người dùng mới
//     options.Lockout.MaxFailedAccessAttempts = 5; // Nếu cần, tăng số lần thất bại trước khi khóa
//     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Thời gian khóa tài khoản
// })
// .AddEntityFrameworkStores<FinalprojectContext>()
// .AddDefaultTokenProviders();

// builder.Services.Configure<IdentityOptions>(options =>
// {
//     options.User.RequireUniqueEmail = true; // Đảm bảo rằng Email là duy nhất
//     options.SignIn.RequireConfirmedEmail = false; // Tùy chọn nếu bạn chưa muốn xác nhận email
// });



// // Đăng ký IEmailSender với dịch vụ giả
// builder.Services.AddTransient<IEmailSender, EmailSender>();

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();


// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// app.Run();

using Blank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ cho Controllers và Razor Pages
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddRazorPages();

// Kết nối cơ sở dữ liệu
var connectionTestDbConnection = builder.Configuration.GetConnectionString("MyConnect");

builder.Services.AddDbContext<FinalprojectContext>(options =>
    options.UseSqlServer(connectionTestDbConnection));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.AllowedForNewUsers = false;  // Tắt khóa tài khoản cho người dùng mới
    options.Lockout.MaxFailedAccessAttempts = 5; // Nếu cần, tăng số lần thất bại trước khi khóa
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Thời gian khóa tài khoản
})
.AddEntityFrameworkStores<FinalprojectContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true; // Đảm bảo rằng Email là duy nhất
    options.SignIn.RequireConfirmedEmail = false; // Tùy chọn nếu bạn chưa muốn xác nhận email
});


// Đăng ký IEmailSender với dịch vụ giả
// Cấu hình EmailSettings từ appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();


var app = builder.Build();


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

app.MapControllers();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
