using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class PaisService : IPaisService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<PaisService> _logger;

    public PaisService(TicketPremiumDbContext db, ILogger<PaisService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PaisDto>> ListarPaises()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ListarPaises | Params=None", ts);

        var result = await _db.Paises
            .OrderBy(p => p.Nombre)
            .Select(p => new PaisDto
            {
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Continente = p.Continente
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ListarPaises | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<PaisDto> ObtenerPais(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ObtenerPais | Params=codigo={Codigo}", ts, codigo);

        var pais = await _db.Paises.FindAsync(codigo);
        if (pais == null)
        {
            _logger.LogWarning("[{Timestamp}] Operation=ObtenerPais | Result=NotFound | codigo={Codigo}", ts, codigo);
            throw new FaultException(new FaultReason("País no encontrado"), new FaultCode("NotFound"));
        }

        return new PaisDto { Codigo = pais.Codigo, Nombre = pais.Nombre, Continente = pais.Continente };
    }

    public async Task CrearPais(PaisDto pais)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=CrearPais | Params=codigo={Codigo}", ts, pais.Codigo);

        if (await _db.Paises.AnyAsync(p => p.Codigo == pais.Codigo))
        {
            throw new FaultException(new FaultReason("Ya existe un país con ese código"), new FaultCode("Duplicate"));
        }

        _db.Paises.Add(new Pais { Codigo = pais.Codigo, Nombre = pais.Nombre, Continente = pais.Continente });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=CrearPais | Result=Success | codigo={Codigo}", ts, pais.Codigo);
    }

    public async Task ActualizarPais(PaisDto pais)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarPais | Params=codigo={Codigo}", ts, pais.Codigo);

        var existing = await _db.Paises.FindAsync(pais.Codigo);
        if (existing == null)
        {
            throw new FaultException(new FaultReason("País no encontrado"), new FaultCode("NotFound"));
        }

        existing.Nombre = pais.Nombre;
        existing.Continente = pais.Continente;
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarPais | Result=Success | codigo={Codigo}", ts, pais.Codigo);
    }

    public async Task EliminarPais(string codigo)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=EliminarPais | Params=codigo={Codigo}", ts, codigo);

        var pais = await _db.Paises.FindAsync(codigo);
        if (pais == null)
        {
            throw new FaultException(new FaultReason("País no encontrado"), new FaultCode("NotFound"));
        }

        _db.Paises.Remove(pais);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=EliminarPais | Result=Success | codigo={Codigo}", ts, codigo);
    }
}
