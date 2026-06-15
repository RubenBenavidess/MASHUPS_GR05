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
    { _db = db; _logger = logger; }

    public async Task<List<PaisDto>> ListarPaises(string sessionToken)
    {
        RequerirAdmin(sessionToken);
        var ts = Now();
        _logger.LogInformation("[{T}] Operation=ListarPaises", ts);
        var result = await _db.Paises.OrderBy(p => p.Nombre)
            .Select(p => new PaisDto { Codigo = p.Codigo, Nombre = p.Nombre, Continente = p.Continente }).ToListAsync();
        _logger.LogInformation("[{T}] ListarPaises=OK | Count={C}", ts, result.Count);
        return result;
    }

    public async Task<PaisDto> ObtenerPais(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var pais = await _db.Paises.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Pais no encontrado"), new FaultCode("NotFound"));
        return new PaisDto { Codigo = pais.Codigo, Nombre = pais.Nombre, Continente = pais.Continente };
    }

    public async Task CrearPais(string sessionToken, PaisDto pais)
    {
        RequerirAdmin(sessionToken);
        if (await _db.Paises.AnyAsync(p => p.Codigo == pais.Codigo))
            throw new FaultException(new FaultReason("Ya existe un pais con ese codigo"), new FaultCode("Duplicate"));
        _db.Paises.Add(new Pais { Codigo = pais.Codigo, Nombre = pais.Nombre, Continente = pais.Continente });
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarPais(string sessionToken, PaisDto pais)
    {
        RequerirAdmin(sessionToken);
        var existing = await _db.Paises.FindAsync(pais.Codigo)
            ?? throw new FaultException(new FaultReason("Pais no encontrado"), new FaultCode("NotFound"));
        existing.Nombre = pais.Nombre; existing.Continente = pais.Continente;
        await _db.SaveChangesAsync();
    }

    public async Task EliminarPais(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var pais = await _db.Paises.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Pais no encontrado"), new FaultCode("NotFound"));
        _db.Paises.Remove(pais);
        await _db.SaveChangesAsync();
    }

    private static void RequerirAdmin(string st) { if (!AuthService.EsAdmin(st)) throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado")); }
    private static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
}
