using ec.edu.monster.AppFIFA.Contracts;
using ec.edu.monster.AppFIFA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CoreWCF;

namespace ec.edu.monster.AppFIFA.Services;

public class AsientoService : IFifaAsientoService
{
    private readonly FifaDbContext _context;
    private readonly ILogger<AsientoService> _logger;

    public AsientoService(FifaDbContext context, ILogger<AsientoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<LocalidadDetalle>> ListarLocalidadesPorPartido(string codigoPartido)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ListarLocalidadesPorPartido | Params=codigoPartido={CodigoPartido}",
            timestamp, codigoPartido);

        var result = await _context.LocalidadesPartido
            .Where(l => l.PartidoCodigo == codigoPartido)
            .Include(l => l.Asientos)
            .OrderBy(l => l.Descripcion)
            .Select(l => new LocalidadDetalle
            {
                Codigo = l.Codigo,
                Descripcion = l.Descripcion,
                PrecioBase = l.PrecioBase,
                Capacidad = l.Capacidad,
                AsientosDisponibles = l.Asientos.Count(a => a.Estado == "LIBRE"),
                Asientos = l.Asientos.Select(a => new AsientoDetalle
                {
                    CodigoAsiento = a.CodigoAsiento,
                    Fila = a.Fila,
                    Numero = a.Numero,
                    Estado = a.Estado
                }).ToList()
            })
            .ToListAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=ListarLocalidadesPorPartido | ResultCount={Count}",
            timestamp, result.Count);

        return result;
    }

    public async Task<ReservaResponse> ReservarAsiento(string codigoAsiento)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ReservarAsiento | Params=codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        var now = DateTime.UtcNow;

        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Asientos SET Estado = {"RESERVADO"}, TimestampReserva = {now} WHERE CodigoAsiento = {codigoAsiento} AND Estado = {"LIBRE"}");

        if (rowsAffected == 0)
        {
            _logger.LogWarning(
                "[{Timestamp}] Operation=ReservarAsiento | Result=Fail | Reason=ASIENTO_NO_DISPONIBLE | codigoAsiento={CodigoAsiento}",
                timestamp, codigoAsiento);

            throw new FaultException("ASIENTO_NO_DISPONIBLE");
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=ReservarAsiento | Result=Success | codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        return new ReservaResponse
        {
            CodigoAsiento = codigoAsiento,
            Exito = true,
            Mensaje = "Asiento reservado exitosamente"
        };
    }

    public async Task<VentaResponse> ConfirmarVenta(string codigoAsiento)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ConfirmarVenta | Params=codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Asientos SET Estado = {"VENDIDO"} WHERE CodigoAsiento = {codigoAsiento} AND Estado = {"RESERVADO"}");

        if (rowsAffected == 0)
        {
            _logger.LogWarning(
                "[{Timestamp}] Operation=ConfirmarVenta | Result=Fail | Reason=ASIENTO_NO_RESERVADO | codigoAsiento={CodigoAsiento}",
                timestamp, codigoAsiento);

            throw new FaultException("ASIENTO_NO_RESERVADO");
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=ConfirmarVenta | Result=Success | codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        return new VentaResponse
        {
            CodigoAsiento = codigoAsiento,
            Exito = true,
            Mensaje = "Venta confirmada exitosamente"
        };
    }

    public async Task<LiberacionResponse> LiberarAsiento(string codigoAsiento)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=LiberarAsiento | Params=codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        var asiento = await _context.Asientos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CodigoAsiento == codigoAsiento);

        if (asiento == null)
        {
            _logger.LogWarning(
                "[{Timestamp}] Operation=LiberarAsiento | Result=Fail | Reason=ASIENTO_NO_ENCONTRADO | codigoAsiento={CodigoAsiento}",
                timestamp, codigoAsiento);

            throw new FaultException("ASIENTO_NO_ENCONTRADO");
        }

        if (asiento.Estado == "LIBRE")
        {
            _logger.LogInformation(
                "[{Timestamp}] Operation=LiberarAsiento | Result=AlreadyFree | codigoAsiento={CodigoAsiento}",
                timestamp, codigoAsiento);

            return new LiberacionResponse
            {
                CodigoAsiento = codigoAsiento,
                Exito = true,
                Mensaje = "El asiento ya se encontraba libre"
            };
        }

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Asientos SET Estado = {"LIBRE"}, TimestampReserva = {null} WHERE CodigoAsiento = {codigoAsiento}");

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=LiberarAsiento | Result=Success | codigoAsiento={CodigoAsiento}",
            timestamp, codigoAsiento);

        return new LiberacionResponse
        {
            CodigoAsiento = codigoAsiento,
            Exito = true,
            Mensaje = "Asiento liberado exitosamente"
        };
    }

    public async Task<ExpirarResponse> ExpirarReservasVencidas()
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ExpirarReservasVencidas | Params=None",
            timestamp);

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Asientos SET Estado = 'LIBRE', TimestampReserva = NULL WHERE Estado = 'RESERVADO' AND DATEDIFF(MINUTE, TimestampReserva, GETDATE()) > 10");

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=ExpirarReservasVencidas | Result=Success | AsientosExpirados={Count}",
            timestamp, rowsAffected);

        return new ExpirarResponse
        {
            AsientosExpirados = rowsAffected,
            Exito = true,
            Mensaje = $"Se expiraron {rowsAffected} reservas vencidas"
        };
    }
}
