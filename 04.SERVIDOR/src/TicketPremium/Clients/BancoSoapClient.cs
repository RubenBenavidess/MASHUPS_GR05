using System.Xml.Linq;

namespace ec.edu.monster.TicketPremium.Clients;

public class BancoSoapClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BancoSoapClient> _logger;
    private const string BancoEndpoint = "http://localhost:9098/CreditoService.svc";

    public BancoSoapClient(IHttpClientFactory httpClientFactory, ILogger<BancoSoapClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("BancoClient");
        _logger = logger;
    }

    public async Task<(bool Aprobado, string Mensaje)> VerificarSujetoCreditoAsync(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=VerificarSujetoCredito | cedula={Cedula}", ts, cedula);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <VerificarSujetoCredito xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
    </VerificarSujetoCredito>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "VerificarSujetoCredito", ts);
        if (!success)
        {
            return (false, responseBody);
        }

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null)
        {
            var faultString = fault.Element(s + "faultstring")?.Value ?? "Fault desconocido";
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=VerificarSujetoCredito | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, faultString);
        }

        var aprobadoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Aprobado");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (aprobadoElement == null)
        {
            return (false, "No se pudo determinar la verificación de crédito");
        }

        var aprobado = bool.Parse(aprobadoElement.Value);
        var mensaje = mensajeElement?.Value ?? "";

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=VerificarSujetoCredito | Aprobado={Aprobado} | Mensaje={Mensaje}", ts, aprobado, mensaje);
        return (aprobado, mensaje);
    }

    public async Task<(bool Exitoso, decimal MontoMaximo, string Mensaje)> ObtenerMontoMaximoAsync(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ObtenerMontoMaximo | cedula={Cedula}", ts, cedula);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ObtenerMontoMaximo xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
    </ObtenerMontoMaximo>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "ObtenerMontoMaximo", ts);
        if (!success)
        {
            return (false, 0, responseBody);
        }

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null)
        {
            var faultString = fault.Element(s + "faultstring")?.Value ?? "Fault desconocido";
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=ObtenerMontoMaximo | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, 0, faultString);
        }

        var exitosoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Exitoso");
        var montoMaximoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "MontoMaximo");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (exitosoElement == null)
        {
            return (false, 0, "No se pudo determinar el monto máximo");
        }

        var exitoso = bool.Parse(exitosoElement.Value);
        var montoMaximo = montoMaximoElement != null ? decimal.Parse(montoMaximoElement.Value) : 0m;
        var mensaje = mensajeElement?.Value ?? "";

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ObtenerMontoMaximo | Exitoso={Exitoso} | MontoMaximo={MontoMaximo} | Mensaje={Mensaje}", ts, exitoso, montoMaximo, mensaje);
        return (exitoso, montoMaximo, mensaje);
    }

    public async Task<(bool Exitoso, int CreditoCodigo, string Mensaje)> RegistrarCreditoAsync(string cedula, decimal monto, int plazoMeses)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | cedula={Cedula}, monto={Monto}, plazoMeses={PlazoMeses}", ts, cedula, monto, plazoMeses);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <RegistrarCredito xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
      <monto>{monto}</monto>
      <plazoMeses>{plazoMeses}</plazoMeses>
    </RegistrarCredito>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "RegistrarCredito", ts);
        if (!success)
        {
            return (false, 0, responseBody);
        }

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null)
        {
            var faultString = fault.Element(s + "faultstring")?.Value ?? "Fault desconocido";
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, 0, faultString);
        }

        var exitosoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Exitoso");
        var creditoCodigoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "CreditoCodigo");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (exitosoElement == null)
        {
            return (false, 0, "No se pudo determinar el registro del crédito");
        }

        var exitoso = bool.Parse(exitosoElement.Value);
        var creditoCodigo = creditoCodigoElement != null ? int.Parse(creditoCodigoElement.Value) : 0;
        var mensaje = mensajeElement?.Value ?? "";

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | Exitoso={Exitoso} | CreditoCodigo={CreditoCodigo} | Mensaje={Mensaje}", ts, exitoso, creditoCodigo, mensaje);
        return (exitoso, creditoCodigo, mensaje);
    }

    private async Task<(bool Success, string Body)> SendRawRequestAsync(string soapEnvelope, string operation, string ts)
    {
        try
        {
            using var content = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", $"http://tempuri.org/ICreditoService/{operation}");

            var response = await _httpClient.PostAsync(BancoEndpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("[{Timestamp}] BANCO_CLIENT={Operation} | HTTP_StatusCode={StatusCode}", ts, operation, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[{Timestamp}] BANCO_CLIENT={Operation} | Result=Fail | HTTP_Error={StatusCode}", ts, operation, (int)response.StatusCode);
                return (false, $"Error HTTP: {(int)response.StatusCode}");
            }

            return (true, responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Timestamp}] BANCO_CLIENT={Operation} | Result=Exception", ts, operation);
            return (false, $"Error de comunicación: {ex.Message}");
        }
    }

    private static string EscapeXml(string value)
    {
        return System.Security.SecurityElement.Escape(value) ?? value;
    }
}
