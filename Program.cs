using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Interfaces;
using SignalR.Models;
using SignalR.Repositories;
using SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SignalRDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SignalRConnection")));
builder.Services.AddSession();


builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.Configure<OpenRouterSettings>(
builder.Configuration.GetSection("OpenRouter"));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();
builder.Services.AddScoped<ICriptografiaService, CriptografiaService>();


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub"); 

app.Run();
