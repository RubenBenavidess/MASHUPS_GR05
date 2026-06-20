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
    { _db = db; _logger = logger; }

    public async Task<List<ResumenLocalidadDto>> ResumenVentasPorPartido(string sessionToken, string codigoPartido)
    {
        RequerirAdmin(sessionToken);
        var result = await _db.DetallesFactura
            .Where(d => d.Asiento.PartidoCodigo == codigoPartido && d.Asiento.Estado == "VENDIDO")
            .GroupBy(d => new { d.Asiento.LocalidadCodigo, d.Asiento.Localidad.Descripcion })
            .Select(g => new ResumenLocalidadDto { CodigoLocalidad = g.Key.LocalidadCodigo, DescripcionLocalidad = g.Key.Descripcion, BoletosVendidos = g.Count(), TotalRecaudado = g.Sum(d => d.PrecioUnitario) })
            .ToListAsync();
        return result;
    }

    public async Task<List<ResumenClienteDto>> ResumenVentasPorCliente(string sessionToken, string cedulaCliente)
    {
        if (!AuthService.EsClienteValido(sessionToken, out var cedulaToken))
            throw new FaultException(new FaultReason("Acceso denegado. Sesión inválida."), new FaultCode("AccesoDenegado"));
            
        if (!AuthService.EsAdmin(sessionToken) && cedulaToken != cedulaCliente)
            throw new FaultException(new FaultReason("Acceso denegado. No puedes ver compras de otro usuario."), new FaultCode("AccesoDenegado"));

        var result = await _db.DetallesFactura
            .Where(d => d.Factura.ClienteCedula == cedulaCliente)
            .Include(d => d.Factura).Include(d => d.Asiento).ThenInclude(a => a.Partido).Include(d => d.Asiento).ThenInclude(a => a.Localidad)
            .Select(d => new ResumenClienteDto { NumeroFactura = d.FacturaNumero, Fecha = d.Factura.Fecha, Partido = d.Asiento.Partido.EquipoLocal + " vs " + d.Asiento.Partido.EquipoVisitante, Asiento = d.AsientoCodigo, Localidad = d.Asiento.Localidad.Descripcion, PrecioUnitario = d.PrecioUnitario, MetodoPago = d.Factura.MetodoPago })
            .ToListAsync();
        return result;
    }

    public async Task<List<DetalleVentaAsientoDto>> DetallesVentasPorPartido(string sessionToken, string codigoPartido)
    {
        RequerirAdmin(sessionToken);
        var result = await _db.DetallesFactura
            .Where(d => d.Asiento.PartidoCodigo == codigoPartido && d.Asiento.Estado == "VENDIDO")
            .Include(d => d.Asiento)
            .Include(d => d.Factura).ThenInclude(f => f.Cliente)
            .Select(d => new DetalleVentaAsientoDto
            {
                NumeroFactura = d.FacturaNumero,
                CodigoAsiento = d.AsientoCodigo,
                Fila = d.Asiento.Fila,
                Numero = d.Asiento.Numero,
                CedulaCliente = d.Factura.ClienteCedula,
                NombreCliente = d.Factura.Cliente.Nombre,
                ApellidoCliente = d.Factura.Cliente.Apellido,
                FechaCompra = d.Factura.Fecha
            })
            .ToListAsync();
        return result;
    }

    private static void RequerirAdmin(string st) { if (!AuthService.EsAdmin(st)) throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado")); }
}
