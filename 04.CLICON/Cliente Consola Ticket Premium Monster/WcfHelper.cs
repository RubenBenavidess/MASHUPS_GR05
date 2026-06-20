namespace TicketPremium.Console
{
    public static class WcfHelper
    {
        // CAMBIA ESTA IP POR LA DE TU NUEVA COMPUTADORA:
        public static string IpAddress = "192.168.0.137";
        public static string UrlBase => $"http://{IpAddress}:9099";

        public static CompraService.CompraServiceClient CreateCompraServiceClient() { var c = new CompraService.CompraServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/CompraService.svc"); return c; }
        public static PartidoService.PartidoServiceClient CreatePartidoServiceClient() { var c = new PartidoService.PartidoServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/PartidoService.svc"); return c; }
        public static LocalidadService.LocalidadServiceClient CreateLocalidadServiceClient() { var c = new LocalidadService.LocalidadServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/LocalidadService.svc"); return c; }
        public static ReporteService.ReporteServiceClient CreateReporteServiceClient() { var c = new ReporteService.ReporteServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/ReporteService.svc"); return c; }
        public static PaisService.PaisServiceClient CreatePaisServiceClient() { var c = new PaisService.PaisServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/PaisService.svc"); return c; }
        public static EstadioService.EstadioServiceClient CreateEstadioServiceClient() { var c = new EstadioService.EstadioServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/EstadioService.svc"); return c; }
        public static ClienteService.ClienteServiceClient CreateClienteServiceClient() { var c = new ClienteService.ClienteServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/ClienteService.svc"); return c; }
        // public static AuthService.AuthServiceClient CreateAuthServiceClient() { var c = new AuthService.AuthServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/AuthService.svc"); return c; }
        public static BancoAdminService.BancoAdminServiceClient CreateBancoAdminServiceClient() { var c = new BancoAdminService.BancoAdminServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/BancoAdminService.svc"); return c; }
        public static AsientoService.AsientoServiceClient CreateAsientoServiceClient() { var c = new AsientoService.AsientoServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/AsientoService.svc"); return c; }
        public static FacturaService.FacturaServiceClient CreateFacturaServiceClient() { var c = new FacturaService.FacturaServiceClient(); c.Endpoint.Address = new System.ServiceModel.EndpointAddress($"{UrlBase}/FacturaService.svc"); return c; }
    }
}

