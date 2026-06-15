using ec.edu.monster.AppFIFA.Contracts;
using ec.edu.monster.AppFIFA.Data;
using ec.edu.monster.AppFIFA.Services;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.EntityFrameworkCore;
// Necesitarás este using para el comportamiento de la metadata:
using CoreWCF.Description;

namespace ec.edu.monster.AppFIFA;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://localhost:9097");

        builder.Services.AddServiceModelServices();

        // Agregamos el servicio de metadata
        builder.Services.AddServiceModelMetadata();

        builder.Services.AddDbContext<FifaDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<PartidoService>();
        builder.Services.AddScoped<AsientoService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FifaDbContext>();
            db.Database.EnsureCreated();
        }

        app.UseServiceModel(serviceBuilder =>
        {
            var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();

            serviceMetadataBehavior.HttpGetEnabled = true;

            serviceBuilder.AddService<PartidoService>();
            serviceBuilder.AddServiceEndpoint<PartidoService, IFifaPartidoService>(
                new BasicHttpBinding(), "/FifaPartidoService.svc");

            // Configuración AsientoService (Ya no necesitamos ConfigureServiceHostBase)
            serviceBuilder.AddService<AsientoService>();
            serviceBuilder.AddServiceEndpoint<AsientoService, IFifaAsientoService>(
                new BasicHttpBinding(), "/FifaAsientoService.svc");
        });

        app.Run();
    }
}