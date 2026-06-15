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
    { _db = db; _logger = logger; }

    public async Task<List<EstadioDto>> ListarEstadios(string sessionToken)
    {
        RequerirAdmin(sessionToken);
        var result = await _db.Estadios.OrderBy(e => e.Nombre)
            .Select(e => new EstadioDto { Codigo = e.Codigo, Nombre = e.Nombre, Ciudad = e.Ciudad, CapacidadTotal = e.CapacidadTotal, PaisCodigo = e.PaisCodigo }).ToListAsync();
        return result;
    }

    public async Task<EstadioDto> ObtenerEstadio(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var e = await _db.Estadios.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        return new EstadioDto { Codigo = e.Codigo, Nombre = e.Nombre, Ciudad = e.Ciudad, CapacidadTotal = e.CapacidadTotal, PaisCodigo = e.PaisCodigo };
    }

    public async Task CrearEstadio(string sessionToken, EstadioDto estadio)
    {
        RequerirAdmin(sessionToken);
        if (await _db.Estadios.AnyAsync(e => e.Codigo == estadio.Codigo))
            throw new FaultException(new FaultReason("Ya existe un estadio con ese codigo"), new FaultCode("Duplicate"));
        _db.Estadios.Add(new Estadio { Codigo = estadio.Codigo, Nombre = estadio.Nombre, Ciudad = estadio.Ciudad, CapacidadTotal = estadio.CapacidadTotal, PaisCodigo = estadio.PaisCodigo });
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarEstadio(string sessionToken, EstadioDto estadio)
    {
        RequerirAdmin(sessionToken);
        var e = await _db.Estadios.FindAsync(estadio.Codigo)
            ?? throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        e.Nombre = estadio.Nombre; e.Ciudad = estadio.Ciudad; e.CapacidadTotal = estadio.CapacidadTotal; e.PaisCodigo = estadio.PaisCodigo;
        await _db.SaveChangesAsync();
    }

    public async Task EliminarEstadio(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var e = await _db.Estadios.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Estadio no encontrado"), new FaultCode("NotFound"));
        _db.Estadios.Remove(e);
        await _db.SaveChangesAsync();
    }

    private static void RequerirAdmin(string st) { if (!AuthService.EsAdmin(st)) throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado")); }
}
