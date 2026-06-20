using System.ServiceModel;
using ec.edu.monster.CoreBancario.Contracts;
using ec.edu.monster.CoreBancario.Data;
using ec.edu.monster.CoreBancario.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.CoreBancario.Services
{
    public class CreditoService : ICreditoService
    {
        private readonly BancoDbContext _db;
        private readonly ILogger<CreditoService> _logger;

        public CreditoService(BancoDbContext db, ILogger<CreditoService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<VerificacionCreditoResponse> VerificarSujetoCredito(string cedula)
        {
            _logger.LogInformation("[{Timestamp}] VerificarSujetoCredito - Cedula: {Cedula}", DateTime.Now, cedula);

            var cliente = await _db.Clientes
                .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                .Include(c => c.Creditos)
                .FirstOrDefaultAsync(c => c.Cedula == cedula);

            if (cliente == null)
            {
                _logger.LogWarning("[{Timestamp}] VerificarSujetoCredito - Cliente no encontrado: {Cedula}", DateTime.Now, cedula);
                return new VerificacionCreditoResponse { Aprobado = false, Mensaje = "Cliente no encontrado." };
            }

            // Rule 1: Estado activo
            if (cliente.Estado != "ACTIVO")
            {
                _logger.LogWarning("[{Timestamp}] VerificarSujetoCredito - Cliente inactivo: {Cedula}", DateTime.Now, cedula);
                return new VerificacionCreditoResponse { Aprobado = false, Mensaje = "El cliente no se encuentra activo." };
            }

            // Rule 2: At least 1 Deposito in last 30 days
            var hace30Dias = DateTime.Now.AddDays(-30);
            var tieneDepositoReciente = cliente.Cuentas
                .SelectMany(cu => cu.Movimientos)
                .Any(m => m.Tipo == "DEPOSITO" && m.Fecha >= hace30Dias);

            if (!tieneDepositoReciente)
            {
                _logger.LogWarning("[{Timestamp}] VerificarSujetoCredito - Sin depósitos recientes: {Cedula}", DateTime.Now, cedula);
                return new VerificacionCreditoResponse { Aprobado = false, Mensaje = "El cliente no tiene depósitos en los últimos 30 días." };
            }

            // Rule 3: If MASCULINO → edad >= 25
            if (cliente.Genero == "MASCULINO")
            {
                var edad = DateTime.Now.Year - cliente.FechaNacimiento.Year;
                if (cliente.FechaNacimiento.Date > DateTime.Now.AddYears(-edad)) edad--;

                if (edad < 25)
                {
                    _logger.LogWarning("[{Timestamp}] VerificarSujetoCredito - Hombre menor de 25: {Cedula}, Edad: {Edad}", DateTime.Now, cedula, edad);
                    return new VerificacionCreditoResponse { Aprobado = false, Mensaje = "El cliente masculino debe tener al menos 25 años." };
                }
            }

            // Rule 4: No active credit
            var tieneCreditoActivo = cliente.Creditos.Any(cr => cr.Estado == "ACTIVO");
            if (tieneCreditoActivo)
            {
                _logger.LogWarning("[{Timestamp}] VerificarSujetoCredito - Crédito activo existente: {Cedula}", DateTime.Now, cedula);
                return new VerificacionCreditoResponse { Aprobado = false, Mensaje = "El cliente ya posee un crédito activo." };
            }

            _logger.LogInformation("[{Timestamp}] VerificarSujetoCredito - Aprobado: {Cedula}", DateTime.Now, cedula);
            return new VerificacionCreditoResponse { Aprobado = true, Mensaje = "El cliente es sujeto de crédito." };
        }

        public async Task<MontoMaximoResponse> ObtenerMontoMaximo(string cedula)
        {
            _logger.LogInformation("[{Timestamp}] ObtenerMontoMaximo - Cedula: {Cedula}", DateTime.Now, cedula);

            var cliente = await _db.Clientes
                .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                .FirstOrDefaultAsync(c => c.Cedula == cedula);

            if (cliente == null)
            {
                return new MontoMaximoResponse { Exitoso = false, MontoMaximo = 0, Mensaje = "Cliente no encontrado." };
            }

            var hace3Meses = DateTime.Now.AddMonths(-3);
            var movimientos = cliente.Cuentas
                .SelectMany(cu => cu.Movimientos)
                .Where(m => m.Fecha >= hace3Meses)
                .ToList();

            var depositos = movimientos.Where(m => m.Tipo == "DEPOSITO").ToList();
            var retiros = movimientos.Where(m => m.Tipo == "RETIRO").ToList();

            var promDep3Meses = depositos.Count > 0
                ? depositos.Average(m => m.Monto)
                : 0m;

            var promRet3Meses = retiros.Count > 0
                ? retiros.Average(m => m.Monto)
                : 0m;

            if (promDep3Meses == 0)
            {
                _logger.LogInformation("[{Timestamp}] ObtenerMontoMaximo - Sin depósitos en 3 meses: {Cedula}", DateTime.Now, cedula);
                return new MontoMaximoResponse { Exitoso = true, MontoMaximo = 0, Mensaje = "No se encontraron depósitos en los últimos 3 meses." };
            }

            var montoMaximo = ((promDep3Meses - promRet3Meses) * 0.35m) * 6;

            if (montoMaximo <= 0)
            {
                _logger.LogInformation("[{Timestamp}] ObtenerMontoMaximo - Monto máximo 0 o negativo: {Cedula}", DateTime.Now, cedula);
                return new MontoMaximoResponse { Exitoso = true, MontoMaximo = 0, Mensaje = "El promedio de retiros supera o iguala al de depósitos." };
            }

            _logger.LogInformation("[{Timestamp}] ObtenerMontoMaximo - Cedula: {Cedula}, MontoMaximo: {MontoMaximo}", DateTime.Now, cedula, montoMaximo);
            return new MontoMaximoResponse { Exitoso = true, MontoMaximo = Math.Round(montoMaximo, 2), Mensaje = "Cálculo exitoso." };
        }

        public async Task<RegistroCreditoResponse> RegistrarCredito(string cedula, decimal monto, int plazoMeses)
        {
            _logger.LogInformation("[{Timestamp}] RegistrarCredito - Cedula: {Cedula}, Monto: {Monto}, Plazo: {Plazo}", DateTime.Now, cedula, monto, plazoMeses);

            if (plazoMeses < 3 || plazoMeses > 18)
            {
                throw new FaultException("El plazo debe estar entre 3 y 18 meses.");
            }

            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);
            if (cliente == null)
            {
                throw new FaultException("Cliente no encontrado.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var tasaPeriodo = 0.165m / 12m;
                var cuota = monto / ((1m - (decimal)Math.Pow((double)(1m + tasaPeriodo), -plazoMeses)) / tasaPeriodo);
                cuota = Math.Round(cuota, 2);

                var credito = new Credito
                {
                    ClienteId = cliente.Id, 
                    Monto = monto,
                    PlazoMeses = plazoMeses,
                    TasaAnual = 16.5m,
                    FechaAprobacion = DateTime.Now,
                    Estado = "ACTIVO"
                };

                _db.Creditos.Add(credito);
                await _db.SaveChangesAsync();

                var amortizaciones = new List<Amortizacion>();
                var dtos = new List<AmortizacionDto>();
                decimal saldo = monto;

                for (int i = 1; i <= plazoMeses; i++)
                {
                    decimal interes = Math.Round(saldo * tasaPeriodo, 2);
                    decimal capital = Math.Round(cuota - interes, 2);
                    
                    if (i == plazoMeses)
                    {
                        capital = saldo;
                        cuota = capital + interes;
                    }

                    saldo -= capital;
                    if (saldo < 0) saldo = 0;

                    var amort = new Amortizacion
                    {
                        CreditoCodigo = credito.Codigo,
                        NumeroCuota = i,
                        ValorCuota = cuota,
                        FechaPago = DateTime.Now.AddMonths(i)
                    };
                    amortizaciones.Add(amort);

                    dtos.Add(new AmortizacionDto
                    {
                        NumeroCuota = amort.NumeroCuota,
                        ValorCuota = amort.ValorCuota,
                        InteresPagado = interes,
                        CapitalPagado = capital,
                        Saldo = saldo,
                        FechaPago = amort.FechaPago
                    });
                }

                _db.Amortizaciones.AddRange(amortizaciones);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("[{Timestamp}] RegistrarCredito - Éxito: Cedula={Cedula}, CreditoCodigo={Codigo}", DateTime.Now, cedula, credito.Codigo);
                return new RegistroCreditoResponse
                {
                    Exitoso = true,
                    CreditoCodigo = credito.Codigo,
                    Mensaje = $"Crédito registrado exitosamente. Código: {credito.Codigo}",
                    Amortizaciones = dtos
                };
            }
            catch (FaultException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "[{Timestamp}] RegistrarCredito - Error", DateTime.Now);
                throw new FaultException("Error al registrar el crédito.");
            }
        }
        public async Task<CrearClienteResponse> CrearClienteConCuenta(string cedula, string nombre, string apellido, DateTime fechaNacimiento, string genero, decimal depositoInicial)
        {
            try
            {
                var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _logger.LogInformation("[{Timestamp}] Operation=CrearClienteConCuenta | Cedula={Cedula}", ts, cedula);

                if (await _db.Clientes.AnyAsync(c => c.Cedula == cedula))
                {
                    return new CrearClienteResponse { Exitoso = false, Mensaje = "El cliente ya existe en el Banco." };
                }

                var clienteId = Guid.NewGuid();
                var cliente = new ClienteBanco
                {
                    Id = clienteId,
                    Cedula = cedula,
                    Nombre = nombre,
                    Apellido = apellido,
                    FechaNacimiento = fechaNacimiento,
                    Genero = genero,
                    Estado = "ACTIVO"
                };

                var cuenta = new Cuenta
                {
                    Numero = DateTime.Now.Ticks.ToString().Substring(0, 10),
                    Tipo = "AHORROS",
                    Saldo = depositoInicial,
                    ClienteId = clienteId,
                    Movimientos = new List<Movimiento>()
                };

                if (depositoInicial > 0)
                {
                    cuenta.Movimientos.Add(new Movimiento
                    {
                        Tipo = "DEPOSITO",
                        Monto = depositoInicial,
                        Fecha = DateTime.Now.AddDays(-15) // Hace 15 días para dar crédito
                    });
                }

                cliente.Cuentas.Add(cuenta);
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();

                _logger.LogInformation("[{Timestamp}] CrearClienteConCuenta=Success | Cedula={Cedula}", ts, cedula);
                return new CrearClienteResponse { Exitoso = true, Mensaje = "Cliente bancario creado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CrearClienteConCuenta para {Cedula}", cedula);
                return new CrearClienteResponse { Exitoso = false, Mensaje = $"Error interno: {ex.Message}" };
            }
        }

        public async Task<ListarClientesResponse> ListarClientes()
        {
            try
            {
                _logger.LogInformation("[{Timestamp}] Operation=ListarClientes", DateTime.Now);

                var clientes = await _db.Clientes
                    .Select(c => new ClienteBancoDto
                    {
                        Cedula = c.Cedula,
                        Nombre = c.Nombre,
                        Apellido = c.Apellido,
                        FechaNacimiento = c.FechaNacimiento,
                        Genero = c.Genero,
                        Estado = c.Estado
                    })
                    .ToListAsync();

                return new ListarClientesResponse
                {
                    Exitoso = true,
                    Mensaje = "Lista de clientes obtenida exitosamente.",
                    Clientes = clientes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ListarClientes");
                return new ListarClientesResponse { Exitoso = false, Mensaje = $"Error interno: {ex.Message}" };
            }
        }

        public async Task<ActualizarClienteResponse> ActualizarCliente(ClienteBancoDto clienteDto)
        {
            try
            {
                _logger.LogInformation("[{Timestamp}] Operation=ActualizarCliente | Cedula={Cedula}", DateTime.Now, clienteDto.Cedula);

                var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Cedula == clienteDto.Cedula);
                if (cliente == null)
                {
                    return new ActualizarClienteResponse { Exitoso = false, Mensaje = "Cliente no encontrado." };
                }

                cliente.Nombre = clienteDto.Nombre;
                cliente.Apellido = clienteDto.Apellido;
                cliente.FechaNacimiento = clienteDto.FechaNacimiento;
                cliente.Genero = clienteDto.Genero;
                cliente.Estado = string.IsNullOrWhiteSpace(clienteDto.Estado) ? cliente.Estado : clienteDto.Estado;

                await _db.SaveChangesAsync();

                return new ActualizarClienteResponse { Exitoso = true, Mensaje = "Cliente actualizado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ActualizarCliente para {Cedula}", clienteDto.Cedula);
                return new ActualizarClienteResponse { Exitoso = false, Mensaje = $"Error interno: {ex.Message}" };
            }
        }

        public async Task<EliminarClienteResponse> EliminarCliente(string cedula)
        {
            try
            {
                _logger.LogInformation("[{Timestamp}] Operation=EliminarCliente | Cedula={Cedula}", DateTime.Now, cedula);

                var cliente = await _db.Clientes
                    .Include(c => c.Cuentas)
                        .ThenInclude(cu => cu.Movimientos)
                    .Include(c => c.Creditos)
                        .ThenInclude(cr => cr.Amortizaciones)
                    .FirstOrDefaultAsync(c => c.Cedula == cedula);

                if (cliente == null)
                {
                    return new EliminarClienteResponse { Exitoso = false, Mensaje = "Cliente no encontrado." };
                }

                // Entity Framework Core eliminará en cascada si está configurado, o bien podemos remover manualmente si ya lo cargamos
                _db.Clientes.Remove(cliente);
                await _db.SaveChangesAsync();

                return new EliminarClienteResponse { Exitoso = true, Mensaje = "Cliente eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en EliminarCliente para {Cedula}", cedula);
                return new EliminarClienteResponse { Exitoso = false, Mensaje = $"Error al eliminar: {ex.Message}" };
            }
        }

        public async Task<ClienteDetalleResponse> ObtenerClienteDetalle(string cedula)
        {
            try
            {
                _logger.LogInformation("[{Timestamp}] Operation=ObtenerClienteDetalle | Cedula={Cedula}", DateTime.Now, cedula);

                var cliente = await _db.Clientes
                    .Include(c => c.Cuentas)
                    .Include(c => c.Creditos)
                    .FirstOrDefaultAsync(c => c.Cedula == cedula);

                if (cliente == null)
                {
                    return new ClienteDetalleResponse { Exitoso = false, Mensaje = "Cliente no encontrado." };
                }

                decimal saldoAhorros = cliente.Cuentas.Sum(c => c.Saldo);

                var creditosDto = cliente.Creditos.Select(cr => new CreditoDetalleDto
                {
                    Monto = cr.Monto,
                    PlazoMeses = cr.PlazoMeses,
                    FechaAprobacion = cr.FechaAprobacion,
                    Estado = cr.Estado
                }).ToList();

                return new ClienteDetalleResponse
                {
                    Exitoso = true,
                    Mensaje = "Detalle obtenido exitosamente.",
                    SaldoAhorros = saldoAhorros,
                    Creditos = creditosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerClienteDetalle para {Cedula}", cedula);
                return new ClienteDetalleResponse { Exitoso = false, Mensaje = $"Error interno: {ex.Message}" };
            }
        }
    }
}