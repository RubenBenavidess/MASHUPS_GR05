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
    { _db = db; _logger = logger; }

    public async Task<List<LocalidadDto>> ListarLocalidades(string sessionToken)
    {
        RequerirAdmin(sessionToken);
        var result = await _db.Localidades.OrderBy(l => l.EstadioCodigo).ThenBy(l => l.Descripcion)
            .Select(l => new LocalidadDto { Codigo = l.Codigo, Descripcion = l.Descripcion, Capacidad = l.Capacidad, PrecioBase = l.PrecioBase, EstadioCodigo = l.EstadioCodigo }).ToListAsync();
        return result;
    }

    public async Task<LocalidadDto> ObtenerLocalidad(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var l = await _db.Localidades.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        return new LocalidadDto { Codigo = l.Codigo, Descripcion = l.Descripcion, Capacidad = l.Capacidad, PrecioBase = l.PrecioBase, EstadioCodigo = l.EstadioCodigo };
    }

    public async Task CrearLocalidad(string sessionToken, LocalidadDto localidad)
    {
        RequerirAdmin(sessionToken);
        if (await _db.Localidades.AnyAsync(l => l.Codigo == localidad.Codigo))
            throw new FaultException(new FaultReason("Ya existe una localidad con ese codigo"), new FaultCode("Duplicate"));
        _db.Localidades.Add(new Localidad { Codigo = localidad.Codigo, Descripcion = localidad.Descripcion, Capacidad = localidad.Capacidad, PrecioBase = localidad.PrecioBase, EstadioCodigo = localidad.EstadioCodigo });
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarLocalidad(string sessionToken, LocalidadDto localidad)
    {
        RequerirAdmin(sessionToken);
        var l = await _db.Localidades.FindAsync(localidad.Codigo)
            ?? throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        l.Descripcion = localidad.Descripcion; l.Capacidad = localidad.Capacidad; l.PrecioBase = localidad.PrecioBase; l.EstadioCodigo = localidad.EstadioCodigo;
        await _db.SaveChangesAsync();
    }

    public async Task EliminarLocalidad(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var l = await _db.Localidades.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Localidad no encontrada"), new FaultCode("NotFound"));
        _db.Localidades.Remove(l);
        await _db.SaveChangesAsync();
    }

    private static void RequerirAdmin(string st) { if (!AuthService.EsAdmin(st)) throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado")); }
}
