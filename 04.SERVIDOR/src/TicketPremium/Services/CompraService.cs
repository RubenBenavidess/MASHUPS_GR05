using CoreWCF;
using ec.edu.monster.TicketPremium.Clients;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class CompraService : ICompraService
{
    private readonly FifaSoapClient _fifaClient;
    private readonly BancoSoapClient _bancoClient;
    private readonly IFacturaService _facturaService;
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<CompraService> _logger;

    public CompraService(
        FifaSoapClient fifaClient,
        BancoSoapClient bancoClient,
        IFacturaService facturaService,
        TicketPremiumDbContext db,
        ILogger<CompraService> logger)
    {
        _fifaClient = fifaClient;
        _bancoClient = bancoClient;
        _facturaService = facturaService;
        _db = db;
        _logger = logger;
    }

    public async Task<CompraResponse> ComprarEnEfectivo(List<string> codigosAsiento, string clienteCedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ComprarEnEfectivo | Params=codigosAsiento={CodigosAsiento}, clienteCedula={ClienteCedula}",
            ts, string.Join(",", codigosAsiento), clienteCedula);

        var reservados = new List<string>();

        try
        {
            foreach (var codigoAsiento in codigosAsiento)
            {
                _logger.LogInformation("[{Timestamp}] STEP=ReservarAsiento | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);
                var (exitoso, mensaje) = await _fifaClient.ReservarAsientoAsync(codigoAsiento);
                if (!exitoso)
                {
                    _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ASIENTO_NO_DISPONIBLE | AsientoFallo={CodigoAsiento}", ts, codigoAsiento);
                    foreach (var r in reservados)
                        await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ASIENTO_NO_DISPONIBLE: {codigoAsiento}"), new FaultCode("ReservaFallida"));
                }
                reservados.Add(codigoAsiento);
            }

            _logger.LogInformation("[{Timestamp}] STEP=CalcularFactura | esEfectivo=True", ts);
            var facturaResponse = await _facturaService.CalcularFactura(codigosAsiento, true, clienteCedula);

            foreach (var codigoAsiento in codigosAsiento)
            {
                _logger.LogInformation("[{Timestamp}] STEP=ConfirmarVenta | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);
                var (exitoso, mensaje) = await _fifaClient.ConfirmarVentaAsync(codigoAsiento);
                if (!exitoso)
                {
                    _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ERROR_CONFIRMACION_VENTA | AsientoFallo={CodigoAsiento}", ts, codigoAsiento);
                    foreach (var r in reservados)
                        await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ERROR_CONFIRMACION_VENTA: {codigoAsiento}"), new FaultCode("ConfirmacionFallida"));
                }
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                var localAsiento = await _db.Asientos.FindAsync(codigoAsiento);
                if (localAsiento != null)
                    localAsiento.Estado = "VENDIDO";
            }
            await _db.SaveChangesAsync();
            _logger.LogInformation("[{Timestamp}] STEP=LocalSeatsUpdated | Estado=VENDIDO", ts);

            _logger.LogInformation(
                "[{Timestamp}] Operation=ComprarEnEfectivo | Result=Success | Factura={NumeroFactura}",
                ts, facturaResponse.Numero);

            return new CompraResponse
            {
                Exitoso = true,
                NumeroFactura = facturaResponse.Numero,
                Mensaje = "Compra en efectivo exitosa"
            };
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Timestamp}] Operation=ComprarEnEfectivo | Result=Error", ts);
            throw new FaultException(new FaultReason("Error inesperado en la compra en efectivo"), new FaultCode("ErrorInterno"));
        }
    }

    public async Task<CompraResponse> ComprarACredito(List<string> codigosAsiento, string clienteCedula, int plazoMeses)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation(
            "[{Timestamp}] Operation=ComprarACredito | Params=codigosAsiento={CodigosAsiento}, clienteCedula={ClienteCedula}, plazoMeses={PlazoMeses}",
            ts, string.Join(",", codigosAsiento), clienteCedula, plazoMeses);

        var reservados = new List<string>();

        try
        {
            foreach (var codigoAsiento in codigosAsiento)
            {
                _logger.LogInformation("[{Timestamp}] STEP=ReservarAsiento | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);
                var (exitoso, mensaje) = await _fifaClient.ReservarAsientoAsync(codigoAsiento);
                if (!exitoso)
                {
                    _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ASIENTO_NO_DISPONIBLE | AsientoFallo={CodigoAsiento}", ts, codigoAsiento);
                    foreach (var r in reservados)
                        await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ASIENTO_NO_DISPONIBLE: {codigoAsiento}"), new FaultCode("ReservaFallida"));
                }
                reservados.Add(codigoAsiento);
            }

            _logger.LogInformation("[{Timestamp}] STEP=VerificarSujetoCredito | clienteCedula={ClienteCedula}", ts, clienteCedula);
            var verificacion = await _bancoClient.VerificarSujetoCreditoAsync(clienteCedula);
            if (!verificacion.Aprobado)
            {
                _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=CREDITO_DENEGADO | Mensaje={Mensaje}", ts, verificacion.Mensaje);
                foreach (var r in reservados)
                    await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"CREDITO_DENEGADO: {verificacion.Mensaje}"), new FaultCode("CreditoDenegado"));
            }

            _logger.LogInformation("[{Timestamp}] STEP=ObtenerMontoMaximo | clienteCedula={ClienteCedula}", ts, clienteCedula);
            var montoMax = await _bancoClient.ObtenerMontoMaximoAsync(clienteCedula);
            if (!montoMax.Exitoso)
            {
                _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ERROR_MONTO_MAXIMO | Mensaje={Mensaje}", ts, montoMax.Mensaje);
                foreach (var r in reservados)
                    await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"ERROR_MONTO_MAXIMO: {montoMax.Mensaje}"), new FaultCode("MontoMaximoError"));
            }

            _logger.LogInformation("[{Timestamp}] STEP=CalcularFactura | esEfectivo=False", ts);
            var facturaResponse = await _facturaService.CalcularFactura(codigosAsiento, false, clienteCedula);

            if (facturaResponse.Total > montoMax.MontoMaximo)
            {
                _logger.LogWarning(
                    "[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=MONTO_EXCEDIDO | Total={Total} | MontoMaximo={MontoMaximo}",
                    ts, facturaResponse.Total, montoMax.MontoMaximo);
                foreach (var r in reservados)
                    await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(
                    new FaultReason($"MONTO_EXCEDIDO: Total ${facturaResponse.Total} > Monto Máximo ${montoMax.MontoMaximo}"),
                    new FaultCode("MontoExcedido"));
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                _logger.LogInformation("[{Timestamp}] STEP=ConfirmarVenta | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);
                var (exitoso, mensaje) = await _fifaClient.ConfirmarVentaAsync(codigoAsiento);
                if (!exitoso)
                {
                    _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ERROR_CONFIRMACION_VENTA | AsientoFallo={CodigoAsiento}", ts, codigoAsiento);
                    foreach (var r in reservados)
                        await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ERROR_CONFIRMACION_VENTA: {codigoAsiento}"), new FaultCode("ConfirmacionFallida"));
                }
            }

            _logger.LogInformation(
                "[{Timestamp}] STEP=RegistrarCredito | clienteCedula={ClienteCedula}, total={Total}, plazoMeses={PlazoMeses}",
                ts, clienteCedula, facturaResponse.Total, plazoMeses);
            var registroCredito = await _bancoClient.RegistrarCreditoAsync(clienteCedula, facturaResponse.Total, plazoMeses);
            if (!registroCredito.Exitoso)
            {
                _logger.LogWarning("[{Timestamp}] COMPENSATION=LIBERAR_RESERVAS | Reason=ERROR_REGISTRO_CREDITO | Mensaje={Mensaje}", ts, registroCredito.Mensaje);
                foreach (var r in reservados)
                    await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"ERROR_REGISTRO_CREDITO: {registroCredito.Mensaje}"), new FaultCode("RegistroCreditoError"));
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                var localAsiento = await _db.Asientos.FindAsync(codigoAsiento);
                if (localAsiento != null)
                    localAsiento.Estado = "VENDIDO";
            }
            await _db.SaveChangesAsync();
            _logger.LogInformation("[{Timestamp}] STEP=LocalSeatsUpdated | Estado=VENDIDO", ts);

            _logger.LogInformation(
                "[{Timestamp}] Operation=ComprarACredito | Result=Success | Factura={NumeroFactura} | CreditoCodigo={CreditoCodigo}",
                ts, facturaResponse.Numero, registroCredito.CreditoCodigo);

            return new CompraResponse
            {
                Exitoso = true,
                NumeroFactura = facturaResponse.Numero,
                Mensaje = "Compra a crédito exitosa. Crédito #" + registroCredito.CreditoCodigo
            };
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Timestamp}] Operation=ComprarACredito | Result=Error", ts);
            throw new FaultException(new FaultReason("Error inesperado en la compra a crédito"), new FaultCode("ErrorInterno"));
        }
    }
}
