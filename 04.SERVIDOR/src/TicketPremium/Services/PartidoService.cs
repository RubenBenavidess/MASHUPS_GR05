using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class PartidoService : IPartidoService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<PartidoService> _logger;

    public PartidoService(TicketPremiumDbContext db, ILogger<PartidoService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PartidoDto>> ListarPartidos()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ListarPartidos | Params=None", ts);

        var result = await _db.Partidos
            .OrderBy(p => p.FechaHora)
            .Select(p => new PartidoDto
            {
                Codigo = p.Codigo,
                EquipoLocal = p.EquipoLocal,
                EquipoVisitante = p.EquipoVisitante,
                FechaHora = p.FechaHora,
                EstadioCodigo = p.EstadioCodigo
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ListarPartidos | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<PartidoDto> ObtenerPartido(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ObtenerPartido | Params=codigo={Codigo}", ts, codigo);

        var partido = await _db.Partidos.FindAsync(codigo);
        if (partido == null)
        {
            throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        }

        return new PartidoDto
        {
            Codigo = partido.Codigo,
            EquipoLocal = partido.EquipoLocal,
            EquipoVisitante = partido.EquipoVisitante,
            FechaHora = partido.FechaHora,
            EstadioCodigo = partido.EstadioCodigo
        };
    }

    public async Task CrearPartido(PartidoDto partido)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=CrearPartido | Params=codigo={Codigo}", ts, partido.Codigo);

        if (await _db.Partidos.AnyAsync(p => p.Codigo == partido.Codigo))
        {
            throw new FaultException(new FaultReason("Ya existe un partido con ese código"), new FaultCode("Duplicate"));
        }

        _db.Partidos.Add(new Partido
        {
            Codigo = partido.Codigo,
            EquipoLocal = partido.EquipoLocal,
            EquipoVisitante = partido.EquipoVisitante,
            FechaHora = partido.FechaHora,
            EstadioCodigo = partido.EstadioCodigo
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=CrearPartido | Result=Success", ts);
    }

    public async Task ActualizarPartido(PartidoDto partido)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarPartido | Params=codigo={Codigo}", ts, partido.Codigo);

        var existing = await _db.Partidos.FindAsync(partido.Codigo);
        if (existing == null)
        {
            throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        }

        existing.EquipoLocal = partido.EquipoLocal;
        existing.EquipoVisitante = partido.EquipoVisitante;
        existing.FechaHora = partido.FechaHora;
        existing.EstadioCodigo = partido.EstadioCodigo;
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarPartido | Result=Success", ts);
    }

    public async Task EliminarPartido(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=EliminarPartido | Params=codigo={Codigo}", ts, codigo);

        var partido = await _db.Partidos.FindAsync(codigo);
        if (partido == null)
        {
            throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        }

        _db.Partidos.Remove(partido);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=EliminarPartido | Result=Success", ts);
    }
}
