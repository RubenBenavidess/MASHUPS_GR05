using ec.edu.monster.AppFIFA.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.AppFIFA.Data;

public class FifaDbContext : DbContext
{
    public FifaDbContext(DbContextOptions<FifaDbContext> options) : base(options) { }

    public DbSet<PartidoFutbol> PartidosFutbol { get; set; } = null!;
    public DbSet<LocalidadPartido> LocalidadesPartido { get; set; } = null!;
    public DbSet<Asiento> Asientos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=FifaDB;User Id=sa;Password=TicketPremium2026!;TrustServerCertificate=true;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PartidoFutbol>(entity =>
        {
            entity.ToTable("PartidosFutbol");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.EquipoLocal).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EquipoVisitante).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EstadioCodigo).HasMaxLength(50).IsRequired();
            entity.HasMany(e => e.Localidades)
                  .WithOne(l => l.Partido)
                  .HasForeignKey(l => l.PartidoCodigo)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LocalidadPartido>(entity =>
        {
            entity.ToTable("LocalidadesPartido");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PartidoCodigo).HasMaxLength(50).IsRequired();
            entity.HasMany(e => e.Asientos)
                  .WithOne(a => a.LocalidadPartido)
                  .HasForeignKey(a => a.LocalidadPartidoCodigo)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Asiento>(entity =>
        {
            entity.ToTable("Asientos");
            entity.HasKey(e => e.CodigoAsiento);
            entity.Property(e => e.CodigoAsiento).HasMaxLength(50);
            entity.Property(e => e.Fila).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();
            entity.Property(e => e.LocalidadPartidoCodigo).HasMaxLength(50).IsRequired();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartidoFutbol>().HasData(
            new PartidoFutbol
            {
                Codigo = "P001",
                EquipoLocal = "USA",
                EquipoVisitante = "MEX",
                FechaHora = new DateTime(2026, 6, 11, 14, 0, 0, DateTimeKind.Utc),
                EstadioCodigo = "Estadio Azteca"
            },
            new PartidoFutbol
            {
                Codigo = "P002",
                EquipoLocal = "ARG",
                EquipoVisitante = "BRA",
                FechaHora = new DateTime(2026, 6, 13, 16, 0, 0, DateTimeKind.Utc),
                EstadioCodigo = "MetLife Stadium"
            },
            new PartidoFutbol
            {
                Codigo = "P003",
                EquipoLocal = "FRA",
                EquipoVisitante = "ENG",
                FechaHora = new DateTime(2026, 6, 15, 18, 0, 0, DateTimeKind.Utc),
                EstadioCodigo = "SoFi Stadium"
            }
        );

        modelBuilder.Entity<LocalidadPartido>().HasData(
            new LocalidadPartido { Codigo = "L001", Descripcion = "PALCO", PartidoCodigo = "P001", Capacidad = 10, PrecioBase = 200.00m },
            new LocalidadPartido { Codigo = "L002", Descripcion = "TRIBUNA", PartidoCodigo = "P001", Capacidad = 12, PrecioBase = 100.00m },
            new LocalidadPartido { Codigo = "L003", Descripcion = "GENERAL", PartidoCodigo = "P001", Capacidad = 15, PrecioBase = 50.00m },
            new LocalidadPartido { Codigo = "L004", Descripcion = "PALCO", PartidoCodigo = "P002", Capacidad = 10, PrecioBase = 250.00m },
            new LocalidadPartido { Codigo = "L005", Descripcion = "TRIBUNA", PartidoCodigo = "P002", Capacidad = 12, PrecioBase = 120.00m },
            new LocalidadPartido { Codigo = "L006", Descripcion = "GENERAL", PartidoCodigo = "P002", Capacidad = 15, PrecioBase = 60.00m },
            new LocalidadPartido { Codigo = "L007", Descripcion = "PALCO", PartidoCodigo = "P003", Capacidad = 10, PrecioBase = 300.00m },
            new LocalidadPartido { Codigo = "L008", Descripcion = "TRIBUNA", PartidoCodigo = "P003", Capacidad = 12, PrecioBase = 150.00m },
            new LocalidadPartido { Codigo = "L009", Descripcion = "GENERAL", PartidoCodigo = "P003", Capacidad = 15, PrecioBase = 75.00m }
        );

        var asientos = new List<Asiento>();
        var localidadesAsientos = new (string localidad, int capacidad)[]
        {
            ("L001", 10), ("L002", 12), ("L003", 15),
            ("L004", 10), ("L005", 12), ("L006", 15),
            ("L007", 10), ("L008", 12), ("L009", 15)
        };

        foreach (var (localidad, capacidad) in localidadesAsientos)
        {
            for (int i = 1; i <= capacidad; i++)
            {
                var fila = (char)('A' + (i - 1) / 5);
                asientos.Add(new Asiento
                {
                    CodigoAsiento = $"AS-{localidad}-{i:D2}",
                    Fila = fila.ToString(),
                    Numero = i,
                    Estado = "LIBRE",
                    TimestampReserva = null,
                    LocalidadPartidoCodigo = localidad
                });
            }
        }

        modelBuilder.Entity<Asiento>().HasData(asientos.ToArray());
    }
}
