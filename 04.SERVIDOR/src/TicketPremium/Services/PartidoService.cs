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
    { _db = db; _logger = logger; }

    public async Task<List<PartidoDto>> ListarPartidos(string sessionToken)
    {
        var result = await _db.Partidos.OrderBy(p => p.FechaHora)
            .Select(p => new PartidoDto { Codigo = p.Codigo, EquipoLocal = p.EquipoLocal, EquipoVisitante = p.EquipoVisitante, FechaHora = p.FechaHora, EstadioCodigo = p.EstadioCodigo }).ToListAsync();
        return result;
    }

    public async Task<PartidoDto> ObtenerPartido(string sessionToken, string codigo)
    {
        RequerirAdminOCliente(sessionToken);
        var p = await _db.Partidos.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        return new PartidoDto { Codigo = p.Codigo, EquipoLocal = p.EquipoLocal, EquipoVisitante = p.EquipoVisitante, FechaHora = p.FechaHora, EstadioCodigo = p.EstadioCodigo };
    }

    public async Task CrearPartido(string sessionToken, PartidoDto partido)
    {
        RequerirAdmin(sessionToken);
        if (await _db.Partidos.AnyAsync(p => p.Codigo == partido.Codigo))
            throw new FaultException(new FaultReason("Ya existe un partido con ese codigo"), new FaultCode("Duplicate"));
        _db.Partidos.Add(new Partido { Codigo = partido.Codigo, EquipoLocal = partido.EquipoLocal, EquipoVisitante = partido.EquipoVisitante, FechaHora = partido.FechaHora, EstadioCodigo = partido.EstadioCodigo });
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarPartido(string sessionToken, PartidoDto partido)
    {
        RequerirAdmin(sessionToken);
        var p = await _db.Partidos.FindAsync(partido.Codigo)
            ?? throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        p.EquipoLocal = partido.EquipoLocal; p.EquipoVisitante = partido.EquipoVisitante; p.FechaHora = partido.FechaHora; p.EstadioCodigo = partido.EstadioCodigo;
        await _db.SaveChangesAsync();
    }

    public async Task EliminarPartido(string sessionToken, string codigo)
    {
        RequerirAdmin(sessionToken);
        var p = await _db.Partidos.FindAsync(codigo)
            ?? throw new FaultException(new FaultReason("Partido no encontrado"), new FaultCode("NotFound"));
        _db.Partidos.Remove(p);
        await _db.SaveChangesAsync();
    }

    private static void RequerirAdmin(string st) { if (!AuthService.EsAdmin(st)) throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado")); }
    private static void RequerirAdminOCliente(string st) { if (!AuthService.EsAdmin(st) && !AuthService.EsClienteValido(st, out _)) throw new FaultException(new FaultReason("Acceso denegado. Inicie sesion."), new FaultCode("AccesoDenegado")); }
}
