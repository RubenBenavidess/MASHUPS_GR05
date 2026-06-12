using ec.edu.monster.AppFIFA.Contracts;
using ec.edu.monster.AppFIFA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ec.edu.monster.AppFIFA.Services;

public class PartidoService : IFifaPartidoService
{
    private readonly FifaDbContext _context;
    private readonly ILogger<PartidoService> _logger;

    public PartidoService(FifaDbContext context, ILogger<PartidoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PartidoDisponible>> ListarPartidosDisponibles()
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ListarPartidosDisponibles | Params=None",
            timestamp);

        var now = DateTime.UtcNow;

        var result = await _context.PartidosFutbol
            .Where(p => p.FechaHora >= now)
            .OrderBy(p => p.FechaHora)
            .Select(p => new PartidoDisponible
            {
                Codigo = p.Codigo,
                EquipoLocal = p.EquipoLocal,
                EquipoVisitante = p.EquipoVisitante,
                FechaHora = p.FechaHora,
                Lugar = p.EstadioCodigo
            })
            .ToListAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=ListarPartidosDisponibles | ResultCount={Count}",
            timestamp,
            result.Count);

        return result;
    }
}
