using TechBricks.Helper;
using TechBricks.Models;
using TechBricks.Background;

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

// existing registrations
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IBulkEmailSender, BulkEmailSender>();

// background queue + hosted service (assumes these files already exist)
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

// Register job store (in-memory). Replace with persistent store later if needed.
builder.Services.AddSingleton<IEmailJobStore, EmailJobStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
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