using Microsoft.AspNetCore.Components.Web;
using TicketPremium.Web;
using ec.edu.monster.TicketClient.Servicios;
using ec.edu.monster.TicketClient.State;
using CompraService;
using PartidoService;
using LocalidadService;
using ReporteService;
using PaisService;
using ClienteService;
using EstadioService;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// State Services
builder.Services.AddScoped<CarritoState>();
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<TicketingService>();

// WCF Services
// Configurar IP base para los servicios WCF
// CAMBIA ESTA IP POR LA DE TU NUEVA COMPUTADORA:
string ipAddress = "192.168.0.137"; 
string wcfUrlBase = $"http://{ipAddress}:9099";

builder.Services.AddScoped(sp => { var c = new CompraServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/CompraService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new PartidoServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/PartidoService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new LocalidadServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/LocalidadService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new ReporteServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/ReporteService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new PaisServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/PaisService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new EstadioServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/EstadioService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new ClienteServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/ClienteService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new AuthService.AuthServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/AuthService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new BancoAdminService.BancoAdminServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/BancoAdminService.svc"); return c; });
builder.Services.AddScoped(sp => { var c = new AsientoService.AsientoServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{wcfUrlBase}/AsientoService.svc"); return c; });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
