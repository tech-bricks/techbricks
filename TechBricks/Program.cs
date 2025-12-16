using TechBricks.Helper;
using TechBricks.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    // Add Razor Runtime Compilation only in Development
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddControllersWithViews();
}

// 1. Register EmailSettings configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 2. Register the IEmailSender service
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
