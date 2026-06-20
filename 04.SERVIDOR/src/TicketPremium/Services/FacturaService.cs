using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class FacturaService : IFacturaService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<FacturaService> _logger;

    public FacturaService(TicketPremiumDbContext db, ILogger<FacturaService> logger)
    { _db = db; _logger = logger; }

    public async Task<FacturaResponse> CalcularFactura(string sessionToken, List<string> codigosAsiento, bool esEfectivo, string clienteCedula)
    {
        RequerirAdminOCliente(sessionToken);
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{T}] Operation=CalcularFactura | codigos={C}, efectivo={E}, cedula={CL}", ts, string.Join(",", codigosAsiento), esEfectivo, clienteCedula);

        var clienteExiste = await _db.Clientes.AnyAsync(c => c.Cedula == clienteCedula);
        if (!clienteExiste)
        {
            throw new FaultException(new FaultReason($"El cliente con cedula {clienteCedula} no esta registrado en el sistema. Debe registrarse primero."), new FaultCode("ClienteNoEncontrado"));
        }

        var asientos = await _db.Asientos.Where(a => codigosAsiento.Contains(a.Codigo)).Include(a => a.Localidad).Include(a => a.Partido).ToListAsync();

        if (asientos.Count != codigosAsiento.Count)
        {
            var encontrados = asientos.Select(a => a.Codigo).ToHashSet();
            var faltantes = codigosAsiento.Where(c => !encontrados.Contains(c)).ToList();
            throw new FaultException(new FaultReason("Uno o mas asientos no existen: " + string.Join(", ", faltantes)), new FaultCode("AsientosNoEncontrados"));
        }

        var subtotal = asientos.Sum(a => a.Localidad.PrecioBase);
        var descuento = esEfectivo ? subtotal * 0.12m : 0m;
        var baseImponible = subtotal - descuento;
        var iva = baseImponible * 0.15m;
        var total = baseImponible + iva;
        var numero = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmmss");

        var factura = new Factura
        {
            Numero = numero, Fecha = DateTime.Now,
            Subtotal = Math.Round(subtotal, 2), Descuento = Math.Round(descuento, 2),
            Iva = Math.Round(iva, 2), Total = Math.Round(total, 2),
            MetodoPago = esEfectivo ? "EFECTIVO" : "CREDITO_DIRECTO", ClienteCedula = clienteCedula
        };
        _db.Facturas.Add(factura);
        foreach (var a in asientos)
            _db.DetallesFactura.Add(new DetalleFactura { FacturaNumero = numero, AsientoCodigo = a.Codigo, PrecioUnitario = a.Localidad.PrecioBase });
        await _db.SaveChangesAsync();

        var items = asientos.Select(a => new FacturaItemDto
        {
            CodigoAsiento = a.Codigo, Localidad = a.Localidad.Descripcion,
            Partido = $"{a.Partido.EquipoLocal} vs {a.Partido.EquipoVisitante}", PrecioUnitario = a.Localidad.PrecioBase
        }).ToList();

        return new FacturaResponse { Numero = numero, Fecha = factura.Fecha, Subtotal = factura.Subtotal, Descuento = factura.Descuento, Iva = factura.Iva, Total = factura.Total, MetodoPago = factura.MetodoPago, ClienteCedula = clienteCedula, Items = items };
    }

    private static void RequerirAdminOCliente(string st) { if (!AuthService.EsAdmin(st) && !AuthService.EsClienteValido(st, out _)) throw new FaultException(new FaultReason("Acceso denegado. Inicie sesion."), new FaultCode("AccesoDenegado")); }
}
