using ec.edu.monster.CoreBancario.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.CoreBancario.Data
{
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options) { }

        public DbSet<ClienteBanco> Clientes { get; set; } = null!;
        public DbSet<Cuenta> Cuentas { get; set; } = null!;
        public DbSet<Movimiento> Movimientos { get; set; } = null!;
        public DbSet<Credito> Creditos { get; set; } = null!;
        public DbSet<Amortizacion> Amortizaciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClienteBanco>(entity =>
            {
                entity.HasKey(e => e.Cedula);
                entity.HasMany(e => e.Cuentas)
                      .WithOne(c => c.ClienteBanco)
                      .HasForeignKey(c => c.ClienteCedula)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Creditos)
                      .WithOne(cr => cr.ClienteBanco)
                      .HasForeignKey(cr => cr.ClienteCedula)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Numero);
                entity.HasMany(e => e.Movimientos)
                      .WithOne(m => m.Cuenta)
                      .HasForeignKey(m => m.CuentaNumero)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasKey(e => e.Codigo);
            });

            modelBuilder.Entity<Credito>(entity =>
            {
                entity.HasKey(e => e.Codigo);
                entity.HasMany(e => e.Amortizaciones)
                      .WithOne(a => a.Credito)
                      .HasForeignKey(a => a.CreditoCodigo)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Amortizacion>(entity =>
            {
                entity.HasKey(e => e.Codigo);
            });

            var now = new DateTime(2026, 6, 10);

            // Cliente 1: Juan Perez - MASCULINO, 45, ACTIVO (ELIGIBLE)
            var c1 = new ClienteBanco
            {
                Cedula = "1712345678",
                Nombre = "Juan",
                Apellido = "Perez",
                FechaNacimiento = new DateTime(1981, 3, 15),
                Genero = "MASCULINO",
                Estado = "ACTIVO"
            };

            // Cliente 2: Maria Gomez - FEMENINO, 30, ACTIVO (ELIGIBLE)
            var c2 = new ClienteBanco
            {
                Cedula = "1712345679",
                Nombre = "Maria",
                Apellido = "Gomez",
                FechaNacimiento = new DateTime(1996, 5, 20),
                Genero = "FEMENINO",
                Estado = "ACTIVO"
            };

            // Cliente 3: Pedro Ramirez - MASCULINO, 22, ACTIVO (NOT ELIGIBLE - male under 25)
            var c3 = new ClienteBanco
            {
                Cedula = "1712345680",
                Nombre = "Pedro",
                Apellido = "Ramirez",
                FechaNacimiento = new DateTime(2004, 1, 10),
                Genero = "MASCULINO",
                Estado = "ACTIVO"
            };

            // Cliente 4: Ana Lopez - FEMENINO, 28, INACTIVO (NOT ELIGIBLE - inactive)
            var c4 = new ClienteBanco
            {
                Cedula = "1712345681",
                Nombre = "Ana",
                Apellido = "Lopez",
                FechaNacimiento = new DateTime(1998, 7, 22),
                Genero = "FEMENINO",
                Estado = "INACTIVO"
            };

            // Cliente 5: Carlos Ruiz - MASCULINO, 35, ACTIVO, has active credit (NOT ELIGIBLE)
            var c5 = new ClienteBanco
            {
                Cedula = "1712345682",
                Nombre = "Carlos",
                Apellido = "Ruiz",
                FechaNacimiento = new DateTime(1991, 8, 5),
                Genero = "MASCULINO",
                Estado = "ACTIVO"
            };

            modelBuilder.Entity<ClienteBanco>().HasData(c1, c2, c3, c4, c5);

            // Cuentas
            var cuenta1 = new Cuenta { Numero = "1001-001", ClienteCedula = "1712345678", Tipo = "AHORROS", Saldo = 2800m };
            var cuenta2 = new Cuenta { Numero = "1002-001", ClienteCedula = "1712345679", Tipo = "CORRIENTE", Saldo = 4500m };
            var cuenta3 = new Cuenta { Numero = "1003-001", ClienteCedula = "1712345680", Tipo = "AHORROS", Saldo = 600m };
            var cuenta4 = new Cuenta { Numero = "1004-001", ClienteCedula = "1712345681", Tipo = "AHORROS", Saldo = 1200m };
            var cuenta5 = new Cuenta { Numero = "1005-001", ClienteCedula = "1712345682", Tipo = "CORRIENTE", Saldo = 3500m };

            modelBuilder.Entity<Cuenta>().HasData(cuenta1, cuenta2, cuenta3, cuenta4, cuenta5);

            // Movimientos Juan - last 30 days and older than 3 months
            var movs = new List<Movimiento>
            {
                // Juan (eligible) - cuenta 1001-001
                new Movimiento { Codigo = 1, CuentaNumero = "1001-001", Tipo = "DEPOSITO", Monto = 1000m, Fecha = now.AddDays(-5) },
                new Movimiento { Codigo = 2, CuentaNumero = "1001-001", Tipo = "DEPOSITO", Monto = 2000m, Fecha = now.AddDays(-50) },
                new Movimiento { Codigo = 3, CuentaNumero = "1001-001", Tipo = "DEPOSITO", Monto = 1500m, Fecha = now.AddDays(-120) },
                new Movimiento { Codigo = 4, CuentaNumero = "1001-001", Tipo = "RETIRO", Monto = 500m, Fecha = now.AddDays(-10) },
                new Movimiento { Codigo = 5, CuentaNumero = "1001-001", Tipo = "RETIRO", Monto = 300m, Fecha = now.AddDays(-95) },

                // Maria (eligible) - cuenta 1002-001
                new Movimiento { Codigo = 6, CuentaNumero = "1002-001", Tipo = "DEPOSITO", Monto = 1200m, Fecha = now.AddDays(-3) },
                new Movimiento { Codigo = 7, CuentaNumero = "1002-001", Tipo = "DEPOSITO", Monto = 2500m, Fecha = now.AddDays(-45) },
                new Movimiento { Codigo = 8, CuentaNumero = "1002-001", Tipo = "DEPOSITO", Monto = 1800m, Fecha = now.AddDays(-150) },
                new Movimiento { Codigo = 9, CuentaNumero = "1002-001", Tipo = "RETIRO", Monto = 400m, Fecha = now.AddDays(-8) },
                new Movimiento { Codigo = 10, CuentaNumero = "1002-001", Tipo = "RETIRO", Monto = 600m, Fecha = now.AddDays(-60) },

                // Pedro (NOT eligible - under 25) - cuenta 1003-001
                new Movimiento { Codigo = 11, CuentaNumero = "1003-001", Tipo = "DEPOSITO", Monto = 800m, Fecha = now.AddDays(-7) },
                new Movimiento { Codigo = 12, CuentaNumero = "1003-001", Tipo = "RETIRO", Monto = 200m, Fecha = now.AddDays(-20) },

                // Ana (NOT eligible - inactive) - cuenta 1004-001
                new Movimiento { Codigo = 13, CuentaNumero = "1004-001", Tipo = "DEPOSITO", Monto = 600m, Fecha = now.AddDays(-15) },
                new Movimiento { Codigo = 14, CuentaNumero = "1004-001", Tipo = "RETIRO", Monto = 300m, Fecha = now.AddDays(-40) },

                // Carlos (NOT eligible - active credit) - cuenta 1005-001
                new Movimiento { Codigo = 15, CuentaNumero = "1005-001", Tipo = "DEPOSITO", Monto = 1500m, Fecha = now.AddDays(-2) },
                new Movimiento { Codigo = 16, CuentaNumero = "1005-001", Tipo = "DEPOSITO", Monto = 3000m, Fecha = now.AddDays(-55) },
                new Movimiento { Codigo = 17, CuentaNumero = "1005-001", Tipo = "RETIRO", Monto = 700m, Fecha = now.AddDays(-12) },
            };

            modelBuilder.Entity<Movimiento>().HasData(movs.ToArray());

            // Carlos has active credit
            modelBuilder.Entity<Credito>().HasData(new Credito
            {
                Codigo = 1,
                ClienteCedula = "1712345682",
                Monto = 10000m,
                PlazoMeses = 12,
                TasaAnual = 16.5m,
                FechaAprobacion = now.AddMonths(-3),
                Estado = "ACTIVO"
            });

            // Amortizaciones for Carlos
            modelBuilder.Entity<Amortizacion>().HasData(
                new Amortizacion { Codigo = 1, CreditoCodigo = 1, NumeroCuota = 1, ValorCuota = 912.85m, FechaPago = now.AddMonths(-2) },
                new Amortizacion { Codigo = 2, CreditoCodigo = 1, NumeroCuota = 2, ValorCuota = 912.85m, FechaPago = now.AddMonths(-1) },
                new Amortizacion { Codigo = 3, CreditoCodigo = 1, NumeroCuota = 3, ValorCuota = 912.85m, FechaPago = now.AddMonths(0) },
                new Amortizacion { Codigo = 4, CreditoCodigo = 1, NumeroCuota = 4, ValorCuota = 912.85m, FechaPago = now.AddMonths(1) }
            );
        }
    }
}
