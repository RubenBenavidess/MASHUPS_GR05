using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class AsientoService : IAsientoService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<AsientoService> _logger;

    public AsientoService(TicketPremiumDbContext db, ILogger<AsientoService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<AsientoDto>> ListarAsientosPorPartido(string sessionToken, string partidoCodigo)
    {
        if (!AuthService.EsAdmin(sessionToken) && !AuthService.EsClienteValido(sessionToken, out _))
        {
            throw new FaultException(new FaultReason("Acceso denegado. Inicie sesion."), new FaultCode("AccesoDenegado"));
        }

        _logger.LogInformation("ListarAsientosPorPartido llamado para el partido: {Partido}", partidoCodigo);

        var asientos = await _db.Asientos
            .Where(a => a.PartidoCodigo == partidoCodigo)
            .ToListAsync();

        return asientos.Select(a => new AsientoDto
        {
            Codigo = a.Codigo,
            Fila = a.Fila,
            Numero = a.Numero,
            Estado = a.Estado,
            LocalidadCodigo = a.LocalidadCodigo,
            PartidoCodigo = a.PartidoCodigo
        }).ToList();
    }
}
