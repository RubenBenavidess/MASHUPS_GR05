using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace TicketPremium.Console.Servicios
{
    public class AuthClient
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };
        private const string AuthEndpoint = "http://localhost:8080/AuthService.svc";
        private const string SoapNS = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string TemNS = "http://tempuri.org/";
        private const string DataNS = "http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts";

        public async Task<(bool Exitoso, string Token, string Nombre, string Rol, string Mensaje)> LoginAsync(string cedula, string password)
        {
            var body = $@"
                <Login xmlns=""{TemNS}"">
                    <request xmlns:d4p1=""{DataNS}"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                        <d4p1:Cedula>{Escape(cedula)}</d4p1:Cedula>
                        <d4p1:Password>{Escape(password)}</d4p1:Password>
                    </request>
                </Login>";

            var responseXml = await SendAsync("Login", body);

            var loginResult = responseXml.Descendants(XName.Get("LoginResult", TemNS)).FirstOrDefault()
                           ?? responseXml.Descendants(XName.Get("LoginResponse", TemNS)).FirstOrDefault();

            if (loginResult == null)
            {
                var fault = responseXml.Descendants(XName.Get("Fault", SoapNS)).FirstOrDefault();
                if (fault != null)
                {
                    var reason = fault.Descendants(XName.Get("Text", SoapNS)).FirstOrDefault()?.Value ?? "Error desconocido";
                    return (false, "", "", "", reason);
                }
                return (false, "", "", "", "Respuesta invalida del servidor.");
            }

            return (
                bool.Parse(loginResult.Element(XName.Get("Exitoso", DataNS))?.Value ?? "false"),
                loginResult.Element(XName.Get("SessionToken", DataNS))?.Value ?? "",
                loginResult.Element(XName.Get("Nombre", DataNS))?.Value ?? "",
                loginResult.Element(XName.Get("Rol", DataNS))?.Value ?? "",
                loginResult.Element(XName.Get("Mensaje", DataNS))?.Value ?? ""
            );
        }

        public async Task<(bool Exitoso, string Token, string Nombre, string Rol, string Mensaje)> RegistroAsync(
            string cedula, string nombre, string apellido, string email, string telefono, string genero, DateTime fechaNac, string password)
        {
            var fecha = fechaNac.ToString("o");
            var body = $@"
                <Registro xmlns=""{TemNS}"">
                    <request xmlns:d4p1=""{DataNS}"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                        <d4p1:Cedula>{Escape(cedula)}</d4p1:Cedula>
                        <d4p1:Nombre>{Escape(nombre)}</d4p1:Nombre>
                        <d4p1:Apellido>{Escape(apellido)}</d4p1:Apellido>
                        <d4p1:Email>{Escape(email)}</d4p1:Email>
                        <d4p1:Telefono>{Escape(telefono)}</d4p1:Telefono>
                        <d4p1:Genero>{Escape(genero)}</d4p1:Genero>
                        <d4p1:FechaNacimiento>{fecha}</d4p1:FechaNacimiento>
                        <d4p1:Password>{Escape(password)}</d4p1:Password>
                    </request>
                </Registro>";

            var responseXml = await SendAsync("Registro", body);

            var result = responseXml.Descendants(XName.Get("RegistroResult", TemNS)).FirstOrDefault()
                      ?? responseXml.Descendants(XName.Get("RegistroResponse", TemNS)).FirstOrDefault();

            if (result == null)
            {
                var fault = responseXml.Descendants(XName.Get("Fault", SoapNS)).FirstOrDefault();
                if (fault != null)
                {
                    var reason = fault.Descendants(XName.Get("Text", SoapNS)).FirstOrDefault()?.Value ?? "Error desconocido";
                    return (false, "", "", "", reason);
                }
                return (false, "", "", "", "Respuesta invalida del servidor.");
            }

            return (
                bool.Parse(result.Element(XName.Get("Exitoso", DataNS))?.Value ?? "false"),
                result.Element(XName.Get("SessionToken", DataNS))?.Value ?? "",
                result.Element(XName.Get("Nombre", DataNS))?.Value ?? "",
                result.Element(XName.Get("Rol", DataNS))?.Value ?? "",
                result.Element(XName.Get("Mensaje", DataNS))?.Value ?? ""
            );
        }

        public async Task<(bool Exitoso, string Cedula, string Rol, string Mensaje)> ValidarTokenAsync(string token)
        {
            var body = $@"
                <ValidarToken xmlns=""{TemNS}"">
                    <sessionToken>{Escape(token)}</sessionToken>
                </ValidarToken>";

            var responseXml = await SendAsync("ValidarToken", body);

            var result = responseXml.Descendants(XName.Get("ValidarTokenResult", TemNS)).FirstOrDefault()
                      ?? responseXml.Descendants(XName.Get("ValidarTokenResponse", TemNS)).FirstOrDefault();

            if (result == null) return (false, "", "", "Respuesta invalida del servidor.");

            return (
                bool.Parse(result.Element(XName.Get("Exitoso", DataNS))?.Value ?? "false"),
                result.Element(XName.Get("Cedula", DataNS))?.Value ?? "",
                result.Element(XName.Get("Rol", DataNS))?.Value ?? "",
                result.Element(XName.Get("Mensaje", DataNS))?.Value ?? ""
            );
        }

        private static async Task<XDocument> SendAsync(string operation, string bodyXml)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <s:Envelope xmlns:s=""{SoapNS}"">
                    <s:Body>{bodyXml}</s:Body>
                </s:Envelope>";

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var request = new HttpRequestMessage(HttpMethod.Post, AuthEndpoint)
            {
                Content = content
            };
            request.Headers.Add("SOAPAction", $"http://tempuri.org/IAuthService/{operation}");

            var response = await _http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(responseBody);
        }

        private static string Escape(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}

