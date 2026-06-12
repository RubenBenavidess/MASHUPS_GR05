using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class ReporteService : IReporteService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<ReporteService> _logger;

    public ReporteService(TicketPremiumDbContext db, ILogger<ReporteService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<ResumenLocalidadDto>> ResumenVentasPorPartido(string codigoPartido)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ResumenVentasPorPartido | Params=codigoPartido={CodigoPartido}", ts, codigoPartido);

        var result = await _db.DetallesFactura
            .Where(d => d.Asiento.PartidoCodigo == codigoPartido && d.Asiento.Estado == "VENDIDO")
            .GroupBy(d => new { d.Asiento.LocalidadCodigo, d.Asiento.Localidad.Descripcion })
            .Select(g => new ResumenLocalidadDto
            {
                CodigoLocalidad = g.Key.LocalidadCodigo,
                DescripcionLocalidad = g.Key.Descripcion,
                BoletosVendidos = g.Count(),
                TotalRecaudado = g.Sum(d => d.PrecioUnitario)
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ResumenVentasPorPartido | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<List<ResumenClienteDto>> ResumenVentasPorCliente(string cedulaCliente)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ResumenVentasPorCliente | Params=cedulaCliente={CedulaCliente}", ts, cedulaCliente);

        var result = await _db.DetallesFactura
            .Where(d => d.Factura.ClienteCedula == cedulaCliente)
            .Include(d => d.Factura)
            .Include(d => d.Asiento)
                .ThenInclude(a => a.Partido)
            .Include(d => d.Asiento)
                .ThenInclude(a => a.Localidad)
            .Select(d => new ResumenClienteDto
            {
                NumeroFactura = d.FacturaNumero,
                Fecha = d.Factura.Fecha,
                Partido = d.Asiento.Partido.EquipoLocal + " vs " + d.Asiento.Partido.EquipoVisitante,
                Asiento = d.AsientoCodigo,
                Localidad = d.Asiento.Localidad.Descripcion,
                PrecioUnitario = d.PrecioUnitario,
                MetodoPago = d.Factura.MetodoPago
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ResumenVentasPorCliente | ResultCount={Count}", ts, result.Count);
        return result;
    }
}
