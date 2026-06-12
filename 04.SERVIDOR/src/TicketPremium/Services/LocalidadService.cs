using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class LocalidadService : ILocalidadService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<LocalidadService> _logger;

    public LocalidadService(TicketPremiumDbContext db, ILogger<LocalidadService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<LocalidadDto>> ListarLocalidades()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ListarLocalidades | Params=None", ts);

        var result = await _db.Localidades
            .OrderBy(l => l.EstadioCodigo)
            .ThenBy(l => l.Descripcion)
            .Select(l => new LocalidadDto
            {
                Codigo = l.Codigo,
                Descripcion = l.Descripcion,
                Capacidad = l.Capacidad,
                PrecioBase = l.PrecioBase,
                EstadioCodigo = l.EstadioCodigo
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ListarLocalidades | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<LocalidadDto> ObtenerLocalidad(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ObtenerLocalidad | Params=codigo={Codigo}", ts, codigo);

        var localidad = await _db.Localidades.FindAsync(codigo);
        if (localidad == null)
        {
            throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        }

        return new LocalidadDto
        {
            Codigo = localidad.Codigo,
            Descripcion = localidad.Descripcion,
            Capacidad = localidad.Capacidad,
            PrecioBase = localidad.PrecioBase,
            EstadioCodigo = localidad.EstadioCodigo
        };
    }

    public async Task CrearLocalidad(LocalidadDto localidad)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=CrearLocalidad | Params=codigo={Codigo}", ts, localidad.Codigo);

        if (await _db.Localidades.AnyAsync(l => l.Codigo == localidad.Codigo))
        {
            throw new FaultException(new FaultReason("Ya existe una localidad con ese código"), new FaultCode("Duplicate"));
        }

        _db.Localidades.Add(new Localidad
        {
            Codigo = localidad.Codigo,
            Descripcion = localidad.Descripcion,
            Capacidad = localidad.Capacidad,
            PrecioBase = localidad.PrecioBase,
            EstadioCodigo = localidad.EstadioCodigo
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=CrearLocalidad | Result=Success", ts);
    }

    public async Task ActualizarLocalidad(LocalidadDto localidad)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarLocalidad | Params=codigo={Codigo}", ts, localidad.Codigo);

        var existing = await _db.Localidades.FindAsync(localidad.Codigo);
        if (existing == null)
        {
            throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        }

        existing.Descripcion = localidad.Descripcion;
        existing.Capacidad = localidad.Capacidad;
        existing.PrecioBase = localidad.PrecioBase;
        existing.EstadioCodigo = localidad.EstadioCodigo;
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarLocalidad | Result=Success", ts);
    }

    public async Task EliminarLocalidad(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=EliminarLocalidad | Params=codigo={Codigo}", ts, codigo);

        var localidad = await _db.Localidades.FindAsync(codigo);
        if (localidad == null)
        {
            throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        }

        _db.Localidades.Remove(localidad);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=EliminarLocalidad | Result=Success", ts);
    }
}
