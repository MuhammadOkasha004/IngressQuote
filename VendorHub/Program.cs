using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using VendorHub.Data;
using VendorHub.Models.Repositories;
using VendorHub.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AppAuthenticationStateProvider>());

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<VendorRepository>();
builder.Services.AddScoped<QuotationRepository>();
builder.Services.AddScoped<AdminDashboardRepository>();
builder.Services.AddScoped<VendorDashboardRepository>();
builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<ActivityLogRepository>();
builder.Services.AddScoped<VendorResponseRepository>();
builder.Services.AddScoped<InvitationRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        string? connStr = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connStr))
        {
            var masked = System.Text.RegularExpressions.Regex.Replace(connStr, @"Password=([^;]+)", "Password=***");
            Console.WriteLine($"[DB] Connecting using: {masked}");
        }
        else
        {
            Console.WriteLine("[DB] WARNING: ConnectionStrings:DefaultConnection is null or empty in appsettings.json");
        }

        db.Database.EnsureCreated();

        try
        {
            db.Database.ExecuteSqlRaw(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Users' AND column_name='Role') THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""Role"" TEXT NOT NULL DEFAULT '';
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Users' AND column_name='CreatedAt') THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""CreatedAt"" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW();
                    END IF;
                END $$;
            ");
        }
        catch { }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] FAILED: Could not connect to the database.");
        Console.WriteLine($"[DB] Reason: {ex.GetType().Name}: {ex.Message}");
        Console.WriteLine($"[DB] Check ConnectionStrings:DefaultConnection in appsettings.json and verify your Neon database is active.");
        Console.WriteLine($"[DB] The application will stop now. Fix the connection string and restart.");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapStaticAssets();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
