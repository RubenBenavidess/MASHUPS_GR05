using System.Xml.Linq;

namespace ec.edu.monster.TicketPremium.Clients;

public class FifaSoapClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FifaSoapClient> _logger;
    private const string FifaEndpoint = "http://localhost:5001/FifaAsientoService.svc";

    public FifaSoapClient(IHttpClientFactory httpClientFactory, ILogger<FifaSoapClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FifaClient");
        _logger = logger;
    }

    public async Task<(bool Exitoso, string Mensaje)> ReservarAsientoAsync(string codigoAsiento)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] FIFA_CLIENT=ReservarAsiento | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ReservarAsiento xmlns=""http://tempuri.org/"">
      <codigoAsiento>{EscapeXml(codigoAsiento)}</codigoAsiento>
    </ReservarAsiento>
  </s:Body>
</s:Envelope>";

        return await SendSoapRequestAsync(soapEnvelope, "ReservarAsiento", ts);
    }

    public async Task<(bool Exitoso, string Mensaje)> ConfirmarVentaAsync(string codigoAsiento)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] FIFA_CLIENT=ConfirmarVenta | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ConfirmarVenta xmlns=""http://tempuri.org/"">
      <codigoAsiento>{EscapeXml(codigoAsiento)}</codigoAsiento>
    </ConfirmarVenta>
  </s:Body>
</s:Envelope>";

        return await SendSoapRequestAsync(soapEnvelope, "ConfirmarVenta", ts);
    }

    public async Task<(bool Exitoso, string Mensaje)> LiberarAsientoAsync(string codigoAsiento)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] FIFA_CLIENT=LiberarAsiento | codigoAsiento={CodigoAsiento}", ts, codigoAsiento);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <LiberarAsiento xmlns=""http://tempuri.org/"">
      <codigoAsiento>{EscapeXml(codigoAsiento)}</codigoAsiento>
    </LiberarAsiento>
  </s:Body>
</s:Envelope>";

        return await SendSoapRequestAsync(soapEnvelope, "LiberarAsiento", ts);
    }

    private async Task<(bool Exitoso, string Mensaje)> SendSoapRequestAsync(string soapEnvelope, string operation, string ts)
    {
        try
        {
            using var content = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", $"http://tempuri.org/IFifaAsientoService/{operation}");

            var response = await _httpClient.PostAsync(FifaEndpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("[{Timestamp}] FIFA_CLIENT={Operation} | HTTP_StatusCode={StatusCode}", ts, operation, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[{Timestamp}] FIFA_CLIENT={Operation} | Result=Fail | HTTP_Error={StatusCode}", ts, operation, (int)response.StatusCode);
                return (false, $"Error HTTP: {(int)response.StatusCode}");
            }

            var doc = XDocument.Parse(responseBody);
            XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

            var fault = doc.Descendants(s + "Fault").FirstOrDefault();
            if (fault != null)
            {
                var faultString = fault.Element(s + "faultstring")?.Value ?? "Fault desconocido";
                _logger.LogWarning("[{Timestamp}] FIFA_CLIENT={Operation} | Result=Fault | FaultString={FaultString}", ts, operation, faultString);
                return (false, faultString);
            }

            var exitoElement = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "Exito");
            var mensajeElement = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

            if (exitoElement == null)
            {
                _logger.LogWarning("[{Timestamp}] FIFA_CLIENT={Operation} | Result=Warning | Reason=NoExitoElement", ts, operation);
                return (false, "No se pudo determinar el resultado de la operación");
            }

            var exito = bool.Parse(exitoElement.Value);
            var mensaje = mensajeElement?.Value ?? "";

            _logger.LogInformation("[{Timestamp}] FIFA_CLIENT={Operation} | Result={Result} | Mensaje={Mensaje}", ts, operation, exito, mensaje);
            return (exito, mensaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Timestamp}] FIFA_CLIENT={Operation} | Result=Exception", ts, operation);
            return (false, $"Error de comunicación: {ex.Message}");
        }
    }

    private static string EscapeXml(string value)
    {
        return System.Security.SecurityElement.Escape(value) ?? value;
    }
}
