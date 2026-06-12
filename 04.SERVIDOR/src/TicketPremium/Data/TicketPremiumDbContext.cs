using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Data;

public class TicketPremiumDbContext : DbContext
{
    public TicketPremiumDbContext(DbContextOptions<TicketPremiumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pais> Paises { get; set; } = null!;
    public DbSet<Estadio> Estadios { get; set; } = null!;
    public DbSet<Localidad> Localidades { get; set; } = null!;
    public DbSet<Partido> Partidos { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Asiento> Asientos { get; set; } = null!;
    public DbSet<Factura> Facturas { get; set; } = null!;
    public DbSet<DetalleFactura> DetallesFactura { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Continente).HasMaxLength(50);
        });

        modelBuilder.Entity<Estadio>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.HasOne(e => e.Pais)
                  .WithMany(p => p.Estadios)
                  .HasForeignKey(e => e.PaisCodigo)
                  .IsRequired();
        });

        modelBuilder.Entity<Localidad>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Estadio)
                  .WithMany(es => es.Localidades)
                  .HasForeignKey(e => e.EstadioCodigo)
                  .IsRequired();
        });

        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.EquipoLocal).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EquipoVisitante).HasMaxLength(100).IsRequired();
            entity.HasOne(e => e.Estadio)
                  .WithMany(es => es.Partidos)
                  .HasForeignKey(e => e.EstadioCodigo)
                  .IsRequired();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Cedula);
            entity.Property(e => e.Cedula).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellido).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Genero).HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<Asiento>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Fila).HasMaxLength(5);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.HasOne(e => e.Localidad)
                  .WithMany(l => l.Asientos)
                  .HasForeignKey(e => e.LocalidadCodigo)
                  .IsRequired();
            entity.HasOne(e => e.Partido)
                  .WithMany(p => p.Asientos)
                  .HasForeignKey(e => e.PartidoCodigo)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Numero);
            entity.Property(e => e.Numero).HasMaxLength(30);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Descuento).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Iva).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MetodoPago).HasMaxLength(30);
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Facturas)
                  .HasForeignKey(e => e.ClienteCedula)
                  .IsRequired();
            entity.HasMany(e => e.Detalles)
                  .WithOne(d => d.Factura)
                  .HasForeignKey(d => d.FacturaNumero)
                  .IsRequired();
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacturaNumero).HasMaxLength(30).IsRequired();
            entity.Property(e => e.AsientoCodigo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Asiento)
                  .WithMany()
                  .HasForeignKey(e => e.AsientoCodigo)
                  .IsRequired();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pais>().HasData(
            new Pais { Codigo = "USA", Nombre = "Estados Unidos", Continente = "América" },
            new Pais { Codigo = "MEX", Nombre = "México", Continente = "América" },
            new Pais { Codigo = "CAN", Nombre = "Canadá", Continente = "América" }
        );

        modelBuilder.Entity<Estadio>().HasData(
            new Estadio { Codigo = "EST-001", Nombre = "Estadio Azteca", Ciudad = "Ciudad de México", CapacidadTotal = 87523, PaisCodigo = "MEX" },
            new Estadio { Codigo = "EST-002", Nombre = "MetLife Stadium", Ciudad = "East Rutherford", CapacidadTotal = 82500, PaisCodigo = "USA" },
            new Estadio { Codigo = "EST-003", Nombre = "SoFi Stadium", Ciudad = "Los Ángeles", CapacidadTotal = 70240, PaisCodigo = "USA" }
        );

        modelBuilder.Entity<Localidad>().HasData(
            new Localidad { Codigo = "LOC-A1", Descripcion = "PALCO", Capacidad = 20, PrecioBase = 500m, EstadioCodigo = "EST-001" },
            new Localidad { Codigo = "LOC-A2", Descripcion = "TRIBUNA", Capacidad = 30, PrecioBase = 250m, EstadioCodigo = "EST-001" },
            new Localidad { Codigo = "LOC-A3", Descripcion = "GENERAL", Capacidad = 50, PrecioBase = 100m, EstadioCodigo = "EST-001" },
            new Localidad { Codigo = "LOC-B1", Descripcion = "PALCO", Capacidad = 20, PrecioBase = 500m, EstadioCodigo = "EST-002" },
            new Localidad { Codigo = "LOC-B2", Descripcion = "TRIBUNA", Capacidad = 30, PrecioBase = 250m, EstadioCodigo = "EST-002" },
            new Localidad { Codigo = "LOC-B3", Descripcion = "GENERAL", Capacidad = 50, PrecioBase = 100m, EstadioCodigo = "EST-002" },
            new Localidad { Codigo = "LOC-C1", Descripcion = "PALCO", Capacidad = 20, PrecioBase = 500m, EstadioCodigo = "EST-003" },
            new Localidad { Codigo = "LOC-C2", Descripcion = "TRIBUNA", Capacidad = 30, PrecioBase = 250m, EstadioCodigo = "EST-003" },
            new Localidad { Codigo = "LOC-C3", Descripcion = "GENERAL", Capacidad = 50, PrecioBase = 100m, EstadioCodigo = "EST-003" }
        );

        modelBuilder.Entity<Partido>().HasData(
            new Partido { Codigo = "PAR-001", EquipoLocal = "México", EquipoVisitante = "Estados Unidos", FechaHora = new DateTime(2026, 6, 11, 20, 0, 0), EstadioCodigo = "EST-001" },
            new Partido { Codigo = "PAR-002", EquipoLocal = "Argentina", EquipoVisitante = "Brasil", FechaHora = new DateTime(2026, 6, 18, 20, 0, 0), EstadioCodigo = "EST-002" },
            new Partido { Codigo = "PAR-003", EquipoLocal = "Francia", EquipoVisitante = "Inglaterra", FechaHora = new DateTime(2026, 7, 5, 20, 0, 0), EstadioCodigo = "EST-003" }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Cedula = "1712345678", Nombre = "Juan", Apellido = "Pérez", FechaNacimiento = new DateTime(1990, 5, 15), Genero = "MASCULINO", Telefono = "0991234567", Email = "juan.perez@email.com" },
            new Cliente { Cedula = "1718765432", Nombre = "María", Apellido = "García", FechaNacimiento = new DateTime(1985, 10, 20), Genero = "FEMENINO", Telefono = "0997654321", Email = "maria.garcia@email.com" },
            new Cliente { Cedula = "1709876543", Nombre = "Carlos", Apellido = "López", FechaNacimiento = new DateTime(1995, 3, 8), Genero = "MASCULINO", Telefono = "0981239876", Email = "carlos.lopez@email.com" }
        );

        var asientos = new List<Asiento>();

        var localidades = new[]
        {
            new { Codigo = "LOC-A1", Partido = "PAR-001", AppFifaLocalidad = "L001", Capacidad = 10 },
            new { Codigo = "LOC-A2", Partido = "PAR-001", AppFifaLocalidad = "L002", Capacidad = 12 },
            new { Codigo = "LOC-A3", Partido = "PAR-001", AppFifaLocalidad = "L003", Capacidad = 15 },
            new { Codigo = "LOC-B1", Partido = "PAR-002", AppFifaLocalidad = "L004", Capacidad = 10 },
            new { Codigo = "LOC-B2", Partido = "PAR-002", AppFifaLocalidad = "L005", Capacidad = 12 },
            new { Codigo = "LOC-B3", Partido = "PAR-002", AppFifaLocalidad = "L006", Capacidad = 15 },
            new { Codigo = "LOC-C1", Partido = "PAR-003", AppFifaLocalidad = "L007", Capacidad = 10 },
            new { Codigo = "LOC-C2", Partido = "PAR-003", AppFifaLocalidad = "L008", Capacidad = 12 },
            new { Codigo = "LOC-C3", Partido = "PAR-003", AppFifaLocalidad = "L009", Capacidad = 15 }
        };

        foreach (var loc in localidades)
        {
            for (int i = 1; i <= loc.Capacidad; i++)
            {
                var fila = (char)('A' + (i - 1) / 5);
                asientos.Add(new Asiento
                {
                    Codigo = $"AS-{loc.AppFifaLocalidad}-{i:D2}",
                    Fila = fila.ToString(),
                    Numero = i,
                    Estado = "LIBRE",
                    LocalidadCodigo = loc.Codigo,
                    PartidoCodigo = loc.Partido
                });
            }
        }

        modelBuilder.Entity<Asiento>().HasData(asientos.ToArray());
    }
}
