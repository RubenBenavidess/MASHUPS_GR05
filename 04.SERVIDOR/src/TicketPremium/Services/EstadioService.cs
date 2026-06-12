using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class EstadioService : IEstadioService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<EstadioService> _logger;

    public EstadioService(TicketPremiumDbContext db, ILogger<EstadioService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<EstadioDto>> ListarEstadios()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ListarEstadios | Params=None", ts);

        var result = await _db.Estadios
            .OrderBy(e => e.Nombre)
            .Select(e => new EstadioDto
            {
                Codigo = e.Codigo,
                Nombre = e.Nombre,
                Ciudad = e.Ciudad,
                CapacidadTotal = e.CapacidadTotal,
                PaisCodigo = e.PaisCodigo
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ListarEstadios | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<EstadioDto> ObtenerEstadio(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ObtenerEstadio | Params=codigo={Codigo}", ts, codigo);

        var estadio = await _db.Estadios.FindAsync(codigo);
        if (estadio == null)
        {
            throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        }

        return new EstadioDto
        {
            Codigo = estadio.Codigo,
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            CapacidadTotal = estadio.CapacidadTotal,
            PaisCodigo = estadio.PaisCodigo
        };
    }

    public async Task CrearEstadio(EstadioDto estadio)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=CrearEstadio | Params=codigo={Codigo}", ts, estadio.Codigo);

        if (await _db.Estadios.AnyAsync(e => e.Codigo == estadio.Codigo))
        {
            throw new FaultException(new FaultReason("Ya existe un estadio con ese código"), new FaultCode("Duplicate"));
        }

        _db.Estadios.Add(new Estadio
        {
            Codigo = estadio.Codigo,
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            CapacidadTotal = estadio.CapacidadTotal,
            PaisCodigo = estadio.PaisCodigo
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=CrearEstadio | Result=Success", ts);
    }

    public async Task ActualizarEstadio(EstadioDto estadio)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarEstadio | Params=codigo={Codigo}", ts, estadio.Codigo);

        var existing = await _db.Estadios.FindAsync(estadio.Codigo);
        if (existing == null)
        {
            throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        }

        existing.Nombre = estadio.Nombre;
        existing.Ciudad = estadio.Ciudad;
        existing.CapacidadTotal = estadio.CapacidadTotal;
        existing.PaisCodigo = estadio.PaisCodigo;
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarEstadio | Result=Success", ts);
    }

    public async Task EliminarEstadio(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=EliminarEstadio | Params=codigo={Codigo}", ts, codigo);

        var estadio = await _db.Estadios.FindAsync(codigo);
        if (estadio == null)
        {
            throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        }

        _db.Estadios.Remove(estadio);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=EliminarEstadio | Result=Success", ts);
    }
}
