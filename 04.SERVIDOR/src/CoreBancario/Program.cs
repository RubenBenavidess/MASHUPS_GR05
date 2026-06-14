using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using ec.edu.monster.CoreBancario.Contracts;
using ec.edu.monster.CoreBancario.Data;
using ec.edu.monster.CoreBancario.Services;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.CoreBancario
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenLocalhost(5002);
            });*/

            builder.Services.AddServiceModelServices();
            builder.Services.AddServiceModelMetadata();
            builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            builder.Services.AddDbContext<BancoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BancoDB")));

            builder.Services.AddScoped<CreditoService>();

            var app = builder.Build();

            app.UseServiceModel(serviceBuilder =>
            {
                var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();

                serviceMetadataBehavior.HttpGetEnabled = true;

                serviceBuilder.AddService<CreditoService>();
                serviceBuilder.AddServiceEndpoint<CreditoService, ICreditoService>(
                    new BasicHttpBinding(),
                    "/CreditoService.svc");
            });

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BancoDbContext>();
                db.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
