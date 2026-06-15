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

    public CompraService(FifaSoapClient fifaClient, BancoSoapClient bancoClient, IFacturaService facturaService, TicketPremiumDbContext db, ILogger<CompraService> logger)
    {
        _fifaClient = fifaClient; _bancoClient = bancoClient; _facturaService = facturaService; _db = db; _logger = logger;
    }

    public async Task<CompraResponse> ComprarEnEfectivo(string sessionToken, List<string> codigosAsiento, string clienteCedula)
    {
        RequerirAutenticado(sessionToken);
        var ts = Now();
        _logger.LogInformation("[{T}] ComprarEnEfectivo | codigos={C}, cedula={CL}", ts, string.Join(",", codigosAsiento), clienteCedula);
        var reservados = new List<string>();

        try
        {
            foreach (var codigoAsiento in codigosAsiento)
            {
                var (exitoso, mensaje) = await _fifaClient.ReservarAsientoAsync(codigoAsiento);
                if (!exitoso)
                {
                    foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ASIENTO_NO_DISPONIBLE: {codigoAsiento}"), new FaultCode("ReservaFallida"));
                }
                reservados.Add(codigoAsiento);
            }

            var facturaResponse = await _facturaService.CalcularFactura(sessionToken, codigosAsiento, true, clienteCedula);

            foreach (var codigoAsiento in codigosAsiento)
            {
                var (exitoso, mensaje) = await _fifaClient.ConfirmarVentaAsync(codigoAsiento);
                if (!exitoso)
                {
                    foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ERROR_CONFIRMACION_VENTA: {codigoAsiento}"), new FaultCode("ConfirmacionFallida"));
                }
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                var localAsiento = await _db.Asientos.FindAsync(codigoAsiento);
                if (localAsiento != null) localAsiento.Estado = "VENDIDO";
            }
            await _db.SaveChangesAsync();

            _logger.LogInformation("[{T}] ComprarEnEfectivo=OK | Factura={F}", ts, facturaResponse.Numero);
            return new CompraResponse { Exitoso = true, NumeroFactura = facturaResponse.Numero, Mensaje = "Compra en efectivo exitosa" };
        }
        catch (FaultException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{T}] ComprarEnEfectivo=Error", ts);
            throw new FaultException(new FaultReason("Error inesperado en la compra en efectivo"), new FaultCode("ErrorInterno"));
        }
    }

    public async Task<CompraResponse> ComprarACredito(string sessionToken, List<string> codigosAsiento, string clienteCedula, int plazoMeses)
    {
        RequerirAutenticado(sessionToken);
        var ts = Now();
        _logger.LogInformation("[{T}] ComprarACredito | codigos={C}, cedula={CL}, plazo={P}", ts, string.Join(",", codigosAsiento), clienteCedula, plazoMeses);
        var reservados = new List<string>();

        try
        {
            foreach (var codigoAsiento in codigosAsiento)
            {
                var (exitoso, mensaje) = await _fifaClient.ReservarAsientoAsync(codigoAsiento);
                if (!exitoso)
                {
                    foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ASIENTO_NO_DISPONIBLE: {codigoAsiento}"), new FaultCode("ReservaFallida"));
                }
                reservados.Add(codigoAsiento);
            }

            var verificacion = await _bancoClient.VerificarSujetoCreditoAsync(clienteCedula);
            if (!verificacion.Aprobado)
            {
                foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"CREDITO_DENEGADO: {verificacion.Mensaje}"), new FaultCode("CreditoDenegado"));
            }

            var montoMax = await _bancoClient.ObtenerMontoMaximoAsync(clienteCedula);
            if (!montoMax.Exitoso)
            {
                foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"ERROR_MONTO_MAXIMO: {montoMax.Mensaje}"), new FaultCode("MontoMaximoError"));
            }

            var facturaResponse = await _facturaService.CalcularFactura(sessionToken, codigosAsiento, false, clienteCedula);

            if (facturaResponse.Total > montoMax.MontoMaximo)
            {
                foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"MONTO_EXCEDIDO: Total ${facturaResponse.Total} > Max ${montoMax.MontoMaximo}"), new FaultCode("MontoExcedido"));
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                var (exitoso, mensaje) = await _fifaClient.ConfirmarVentaAsync(codigoAsiento);
                if (!exitoso)
                {
                    foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                    throw new FaultException(new FaultReason($"ERROR_CONFIRMACION_VENTA: {codigoAsiento}"), new FaultCode("ConfirmacionFallida"));
                }
            }

            var registroCredito = await _bancoClient.RegistrarCreditoAsync(clienteCedula, facturaResponse.Total, plazoMeses);
            if (!registroCredito.Exitoso)
            {
                foreach (var r in reservados) await _fifaClient.LiberarAsientoAsync(r);
                throw new FaultException(new FaultReason($"ERROR_REGISTRO_CREDITO: {registroCredito.Mensaje}"), new FaultCode("RegistroCreditoError"));
            }

            foreach (var codigoAsiento in codigosAsiento)
            {
                var localAsiento = await _db.Asientos.FindAsync(codigoAsiento);
                if (localAsiento != null) localAsiento.Estado = "VENDIDO";
            }
            await _db.SaveChangesAsync();

            _logger.LogInformation("[{T}] ComprarACredito=OK | Credito={C}", ts, registroCredito.CreditoCodigo);
            return new CompraResponse { Exitoso = true, NumeroFactura = facturaResponse.Numero, Mensaje = "Compra a credito exitosa. Credito #" + registroCredito.CreditoCodigo };
        }
        catch (FaultException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{T}] ComprarACredito=Error", ts);
            throw new FaultException(new FaultReason("Error inesperado en la compra a credito"), new FaultCode("ErrorInterno"));
        }
    }

    private static void RequerirAutenticado(string st) { if (!AuthService.EsAdmin(st) && !AuthService.EsClienteValido(st, out _)) throw new FaultException(new FaultReason("Acceso denegado. Inicie sesion."), new FaultCode("AccesoDenegado")); }
    private static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
}
