using ConsultorioMedico.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ConsultorioContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache(); // Necesario para usar sesiones en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de que la sesión expire
    options.Cookie.HttpOnly = true; // Seguridad adicional
    options.Cookie.IsEssential = true; // Necesario para compatibilidad con GDPR
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.UseSession();

app.Run();
