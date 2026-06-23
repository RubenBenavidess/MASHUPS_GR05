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

    public class AmortizacionDto
    {
        public int NumeroCuota { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal InteresPagado { get; set; }
        public decimal CapitalPagado { get; set; }
        public decimal Saldo { get; set; }
        public DateTime FechaPago { get; set; }
    }

    public async Task<(bool Exitoso, int CreditoCodigo, string Mensaje, List<AmortizacionDto> Amortizaciones)> RegistrarCreditoAsync(string cedula, decimal monto, int plazoMeses)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | cedula={Cedula}, monto={Monto}, plazoMeses={PlazoMeses}", ts, cedula, monto, plazoMeses);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <RegistrarCredito xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
      <monto>{monto.ToString(System.Globalization.CultureInfo.InvariantCulture)}</monto>
      <plazoMeses>{plazoMeses}</plazoMeses>
    </RegistrarCredito>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "RegistrarCredito", ts);
        if (!success)
        {
            return (false, 0, responseBody, new List<AmortizacionDto>());
        }

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null)
        {
            var faultString = fault.Descendants().FirstOrDefault(e => e.Name.LocalName == "faultstring")?.Value ?? responseBody;
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, 0, faultString, new List<AmortizacionDto>());
        }

        var exitosoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Exitoso");
        var creditoCodigoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "CreditoCodigo");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (exitosoElement == null)
        {
            return (false, 0, "No se pudo determinar el registro del crédito", new List<AmortizacionDto>());
        }

        var exitoso = bool.Parse(exitosoElement.Value);
        var creditoCodigo = creditoCodigoElement != null ? int.Parse(creditoCodigoElement.Value) : 0;
        var mensaje = mensajeElement?.Value ?? "";

        var amortizaciones = new List<AmortizacionDto>();
        var amortizacionesNodes = doc.Descendants().Where(e => e.Name.LocalName == "AmortizacionDto");
        foreach (var node in amortizacionesNodes)
        {
            var num = node.Elements().FirstOrDefault(e => e.Name.LocalName == "NumeroCuota")?.Value;
            var val = node.Elements().FirstOrDefault(e => e.Name.LocalName == "ValorCuota")?.Value;
            var inter = node.Elements().FirstOrDefault(e => e.Name.LocalName == "InteresPagado")?.Value;
            var cap = node.Elements().FirstOrDefault(e => e.Name.LocalName == "CapitalPagado")?.Value;
            var sal = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Saldo")?.Value;
            var fec = node.Elements().FirstOrDefault(e => e.Name.LocalName == "FechaPago")?.Value;

            if (num != null && val != null)
            {
                amortizaciones.Add(new AmortizacionDto
                {
                    NumeroCuota = int.Parse(num),
                    ValorCuota = decimal.Parse(val, System.Globalization.CultureInfo.InvariantCulture),
                    InteresPagado = inter != null ? decimal.Parse(inter, System.Globalization.CultureInfo.InvariantCulture) : 0,
                    CapitalPagado = cap != null ? decimal.Parse(cap, System.Globalization.CultureInfo.InvariantCulture) : 0,
                    Saldo = sal != null ? decimal.Parse(sal, System.Globalization.CultureInfo.InvariantCulture) : 0,
                    FechaPago = fec != null ? DateTime.Parse(fec) : DateTime.Now
                });
            }
        }

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=RegistrarCredito | Exitoso={Exitoso} | CreditoCodigo={CreditoCodigo} | Mensaje={Mensaje} | Amortizaciones={Count}", ts, exitoso, creditoCodigo, mensaje, amortizaciones.Count);
        return (exitoso, creditoCodigo, mensaje, amortizaciones);
    }

    public async Task<(bool Exitoso, string Mensaje)> CrearClienteConCuentaAsync(string cedula, string nombre, string apellido, DateTime fechaNacimiento, string genero, decimal depositoInicial)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=CrearClienteConCuenta | cedula={Cedula}, depositoInicial={DepositoInicial}", ts, cedula, depositoInicial);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <CrearClienteConCuenta xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
      <nombre>{EscapeXml(nombre)}</nombre>
      <apellido>{EscapeXml(apellido)}</apellido>
      <fechaNacimiento>{fechaNacimiento.ToString("yyyy-MM-ddTHH:mm:ss")}</fechaNacimiento>
      <genero>{EscapeXml(genero)}</genero>
      <depositoInicial>{depositoInicial.ToString(System.Globalization.CultureInfo.InvariantCulture)}</depositoInicial>
    </CrearClienteConCuenta>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "CrearClienteConCuenta", ts);
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
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=CrearClienteConCuenta | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, faultString);
        }

        var exitosoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Exitoso");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (exitosoElement == null)
        {
            return (false, "No se pudo determinar el resultado de la operación");
        }

        var exitoso = bool.Parse(exitosoElement.Value);
        var mensaje = mensajeElement?.Value ?? "";

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=CrearClienteConCuenta | Exitoso={Exitoso} | Mensaje={Mensaje}", ts, exitoso, mensaje);
        return (exitoso, mensaje);
    }

    public async Task<(bool Exitoso, string Mensaje, List<ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto> Clientes)> ListarClientesAsync()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ListarClientes", ts);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ListarClientes xmlns=""http://tempuri.org/""/>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "ListarClientes", ts);
        if (!success)
        {
            return (false, responseBody, new List<ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto>());
        }

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null)
        {
            var faultString = fault.Element(s + "faultstring")?.Value ?? "Fault desconocido";
            _logger.LogWarning("[{Timestamp}] BANCO_CLIENT=ListarClientes | Result=Fault | FaultString={FaultString}", ts, faultString);
            return (false, faultString, new List<ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto>());
        }

        var exitosoElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Exitoso");
        var mensajeElement = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Mensaje");

        if (exitosoElement == null)
        {
            return (false, "No se pudo determinar el resultado de la operación", new List<ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto>());
        }

        var exitoso = bool.Parse(exitosoElement.Value);
        var mensaje = mensajeElement?.Value ?? "";

        var clientes = new List<ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto>();
        var clienteNodes = doc.Descendants().Where(e => e.Name.LocalName == "ClienteBancoDto");
        foreach (var node in clienteNodes)
        {
            var cedula = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Cedula")?.Value;
            var nombre = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Nombre")?.Value;
            var apellido = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Apellido")?.Value;
            var genero = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Genero")?.Value;
            var fecha = node.Elements().FirstOrDefault(e => e.Name.LocalName == "FechaNacimiento")?.Value;

            var estado = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Estado")?.Value;

            if (cedula != null)
            {
                clientes.Add(new ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto
                {
                    Cedula = cedula,
                    Nombre = nombre ?? "",
                    Apellido = apellido ?? "",
                    Genero = genero ?? "",
                    FechaNacimiento = fecha != null ? DateTime.Parse(fecha) : DateTime.Now,
                    Estado = estado ?? "ACTIVO"
                });
            }
        }

        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ListarClientes | Exitoso={Exitoso} | Mensaje={Mensaje} | Clientes={Count}", ts, exitoso, mensaje, clientes.Count);
        return (exitoso, mensaje, clientes);
    }

    public async Task<(bool Exitoso, string Mensaje)> ActualizarClienteAsync(ec.edu.monster.TicketPremium.Contracts.ClienteBancoDto cliente)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ActualizarCliente | cedula={Cedula}", ts, cliente.Cedula);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ActualizarCliente xmlns=""http://tempuri.org/"">
      <cliente xmlns:d4p1=""http://schemas.datacontract.org/2004/07/ec.edu.monster.CoreBancario.Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
        <d4p1:Apellido>{EscapeXml(cliente.Apellido)}</d4p1:Apellido>
        <d4p1:Cedula>{EscapeXml(cliente.Cedula)}</d4p1:Cedula>
        <d4p1:Estado>{EscapeXml(cliente.Estado)}</d4p1:Estado>
        <d4p1:FechaNacimiento>{cliente.FechaNacimiento.ToString("yyyy-MM-ddTHH:mm:ss")}</d4p1:FechaNacimiento>
        <d4p1:Genero>{EscapeXml(cliente.Genero)}</d4p1:Genero>
        <d4p1:Nombre>{EscapeXml(cliente.Nombre)}</d4p1:Nombre>
      </cliente>
    </ActualizarCliente>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "ActualizarCliente", ts);
        if (!success) return (false, responseBody);

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null) return (false, fault.Element(s + "faultstring")?.Value ?? "Fault desconocido");

        var exitoso = bool.Parse(doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Exitoso")?.Value ?? "false");
        var mensaje = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Mensaje")?.Value ?? "";
        return (exitoso, mensaje);
    }

    public async Task<(bool Exitoso, string Mensaje)> EliminarClienteAsync(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=EliminarCliente | cedula={Cedula}", ts, cedula);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <EliminarCliente xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
    </EliminarCliente>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "EliminarCliente", ts);
        if (!success) return (false, responseBody);

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null) return (false, fault.Element(s + "faultstring")?.Value ?? "Fault desconocido");

        var exitoso = bool.Parse(doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Exitoso")?.Value ?? "false");
        var mensaje = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Mensaje")?.Value ?? "";
        return (exitoso, mensaje);
    }

    public async Task<(bool Exitoso, string Mensaje, decimal SaldoAhorros, List<ec.edu.monster.TicketPremium.Contracts.CreditoDetalleDto> Creditos)> ObtenerClienteDetalleAsync(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] BANCO_CLIENT=ObtenerClienteDetalle | cedula={Cedula}", ts, cedula);

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ObtenerClienteDetalle xmlns=""http://tempuri.org/"">
      <cedula>{EscapeXml(cedula)}</cedula>
    </ObtenerClienteDetalle>
  </s:Body>
</s:Envelope>";

        var (success, responseBody) = await SendRawRequestAsync(soapEnvelope, "ObtenerClienteDetalle", ts);
        if (!success) return (false, responseBody, 0, new());

        var doc = XDocument.Parse(responseBody);
        XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        var fault = doc.Descendants(s + "Fault").FirstOrDefault();
        if (fault != null) return (false, fault.Element(s + "faultstring")?.Value ?? "Fault desconocido", 0, new());

        var exitoso = bool.Parse(doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Exitoso")?.Value ?? "false");
        var mensaje = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Mensaje")?.Value ?? "";
        var saldoStr = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "SaldoAhorros")?.Value;
        var saldo = saldoStr != null ? decimal.Parse(saldoStr, System.Globalization.CultureInfo.InvariantCulture) : 0m;

        var creditos = new List<ec.edu.monster.TicketPremium.Contracts.CreditoDetalleDto>();
        var creditoNodes = doc.Descendants().Where(e => e.Name.LocalName == "CreditoDetalleDto");
        foreach (var node in creditoNodes)
        {
            var montoStr = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Monto")?.Value;
            var plazoStr = node.Elements().FirstOrDefault(e => e.Name.LocalName == "PlazoMeses")?.Value;
            var fechaStr = node.Elements().FirstOrDefault(e => e.Name.LocalName == "FechaAprobacion")?.Value;
            var estadoStr = node.Elements().FirstOrDefault(e => e.Name.LocalName == "Estado")?.Value;

            if (montoStr != null && plazoStr != null)
            {
                creditos.Add(new ec.edu.monster.TicketPremium.Contracts.CreditoDetalleDto
                {
                    Monto = decimal.Parse(montoStr, System.Globalization.CultureInfo.InvariantCulture),
                    PlazoMeses = int.Parse(plazoStr),
                    FechaAprobacion = fechaStr != null ? DateTime.Parse(fechaStr) : DateTime.Now,
                    Estado = estadoStr ?? ""
                });
            }
        }

        return (exitoso, mensaje, saldo, creditos);
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
                if ((int)response.StatusCode == 500 && responseBody.Contains("Fault"))
                {
                    return (true, responseBody); // Let the caller parse the Fault XML
                }
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
