using PizzeriaNino.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PizzeriaNino.Models;
using PizzeriaNino.Data;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore.
builder.Services.AddDbContext<PizzeriaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurazione del servizio UserService
builder.Services.AddScoped<UserService>();

// Aggiunta dei servizi di autenticazione e autorizzazione
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Imposta la durata del cookie
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLoggedIn", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Esecuzione delle migrazioni e creazione dell'utente amministratore all'avvio
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PizzeriaContext>();
    context.Database.Migrate();

    // Creazione dell'utente amministratore
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        userService.RegisterUserAsync("admin", "admin@example.com", "AdminPassword123", "Admin").Wait();
    }
}

// Configurazione del middleware HTTP
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