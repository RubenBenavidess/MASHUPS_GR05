using CoreWCF;
using CoreWCF.Configuration;
using ec.edu.monster.TicketPremium.Clients;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Services;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseKestrel(options =>
        {
            options.ListenLocalhost(9099);
        });

        builder.Services.AddServiceModelServices();

        builder.Services.AddDbContext<TicketPremiumDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("TicketPremiumDB")));

        builder.Services.AddScoped<PaisService>();
        builder.Services.AddScoped<EstadioService>();
        builder.Services.AddScoped<LocalidadService>();
        builder.Services.AddScoped<PartidoService>();
        builder.Services.AddScoped<ClienteService>();
        builder.Services.AddScoped<FacturaService>();
        builder.Services.AddScoped<ReporteService>();
        builder.Services.AddScoped<CompraService>();
        builder.Services.AddScoped<AuthService>();

        builder.Services.AddSingleton<FifaSoapClient>();
        builder.Services.AddSingleton<BancoSoapClient>();

        builder.Services.AddHttpClient("FifaClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddHttpClient("BancoClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddScoped<IFacturaService>(sp => sp.GetRequiredService<FacturaService>());

        builder.Services.AddServiceModelMetadata();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TicketPremiumDbContext>();
            db.Database.EnsureCreated();
        }

        app.UseServiceModel(serviceBuilder =>
        {
            var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpGetEnabled = true;

            serviceBuilder.AddService<PaisService>();
            serviceBuilder.AddServiceEndpoint<PaisService, IPaisService>(
                new BasicHttpBinding(), "/PaisService.svc");

            serviceBuilder.AddService<EstadioService>();
            serviceBuilder.AddServiceEndpoint<EstadioService, IEstadioService>(
                new BasicHttpBinding(), "/EstadioService.svc");

            serviceBuilder.AddService<LocalidadService>();
            serviceBuilder.AddServiceEndpoint<LocalidadService, ILocalidadService>(
                new BasicHttpBinding(), "/LocalidadService.svc");

            serviceBuilder.AddService<PartidoService>();
            serviceBuilder.AddServiceEndpoint<PartidoService, IPartidoService>(
                new BasicHttpBinding(), "/PartidoService.svc");

            serviceBuilder.AddService<ClienteService>();
            serviceBuilder.AddServiceEndpoint<ClienteService, IClienteService>(
                new BasicHttpBinding(), "/ClienteService.svc");

            serviceBuilder.AddService<FacturaService>();
            serviceBuilder.AddServiceEndpoint<FacturaService, IFacturaService>(
                new BasicHttpBinding(), "/FacturaService.svc");

            serviceBuilder.AddService<ReporteService>();
            serviceBuilder.AddServiceEndpoint<ReporteService, IReporteService>(
                new BasicHttpBinding(), "/ReporteService.svc");

            serviceBuilder.AddService<CompraService>();
            serviceBuilder.AddServiceEndpoint<CompraService, ICompraService>(
                new BasicHttpBinding(), "/CompraService.svc");

            serviceBuilder.AddService<AuthService>();
            serviceBuilder.AddServiceEndpoint<AuthService, IAuthService>(
                new BasicHttpBinding(), "/AuthService.svc");
        });

        app.Run();
    }
}
