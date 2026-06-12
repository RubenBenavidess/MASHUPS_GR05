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
    {
        _db = db;
        _logger = logger;
    }

    public async Task<FacturaResponse> CalcularFactura(List<string> codigosAsiento, bool esEfectivo, string clienteCedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=CalcularFactura | Params=codigosAsiento={CodigosAsiento}, esEfectivo={EsEfectivo}, clienteCedula={ClienteCedula}",
            ts, string.Join(",", codigosAsiento), esEfectivo, clienteCedula);

        var asientos = await _db.Asientos
            .Where(a => codigosAsiento.Contains(a.Codigo))
            .Include(a => a.Localidad)
            .Include(a => a.Partido)
            .ToListAsync();

        if (asientos.Count != codigosAsiento.Count)
        {
            var encontrados = asientos.Select(a => a.Codigo).ToHashSet();
            var faltantes = codigosAsiento.Where(c => !encontrados.Contains(c)).ToList();
            _logger.LogWarning("[{Timestamp}] Operation=CalcularFactura | Result=Fail | MissingAsientos={Faltantes}",
                ts, string.Join(",", faltantes));
            throw new FaultException(new FaultReason("Uno o más asientos no existen: " + string.Join(", ", faltantes)),
                new FaultCode("AsientosNoEncontrados"));
        }

        var subtotal = asientos.Sum(a => a.Localidad.PrecioBase);
        var descuento = esEfectivo ? subtotal * 0.12m : 0m;
        var baseImponible = subtotal - descuento;
        var iva = baseImponible * 0.15m;
        var total = baseImponible + iva;

        var numero = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmmss");

        var factura = new Factura
        {
            Numero = numero,
            Fecha = DateTime.Now,
            Subtotal = Math.Round(subtotal, 2),
            Descuento = Math.Round(descuento, 2),
            Iva = Math.Round(iva, 2),
            Total = Math.Round(total, 2),
            MetodoPago = esEfectivo ? "EFECTIVO" : "CREDITO_DIRECTO",
            ClienteCedula = clienteCedula
        };

        _db.Facturas.Add(factura);

        foreach (var asiento in asientos)
        {
            _db.DetallesFactura.Add(new DetalleFactura
            {
                FacturaNumero = numero,
                AsientoCodigo = asiento.Codigo,
                PrecioUnitario = asiento.Localidad.PrecioBase
            });
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "[{Timestamp}] Operation=CalcularFactura | Result=Success | FacturaNumero={Numero} | Total={Total}",
            ts, numero, total);

        var items = asientos.Select(a => new FacturaItemDto
        {
            CodigoAsiento = a.Codigo,
            Localidad = a.Localidad.Descripcion,
            Partido = $"{a.Partido.EquipoLocal} vs {a.Partido.EquipoVisitante}",
            PrecioUnitario = a.Localidad.PrecioBase
        }).ToList();

        return new FacturaResponse
        {
            Numero = numero,
            Fecha = factura.Fecha,
            Subtotal = factura.Subtotal,
            Descuento = factura.Descuento,
            Iva = factura.Iva,
            Total = factura.Total,
            MetodoPago = factura.MetodoPago,
            ClienteCedula = clienteCedula,
            Items = items
        };
    }
}
