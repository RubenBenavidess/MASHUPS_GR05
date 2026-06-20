namespace BancoAdminService
{
    using System.Runtime.Serialization;
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="CrearClienteBancoRequest", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class CrearClienteBancoRequest : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Apellido { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FechaNacimiento { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Genero { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal DepositoInicial { get; set; }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="CrearClienteBancoResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class CrearClienteBancoResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ClienteBancoDto", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class ClienteBancoDto : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Apellido { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FechaNacimiento { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Genero { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Estado { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ListarClientesBancoResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class ListarClientesBancoResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<BancoAdminService.ClienteBancoDto> Clientes { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="CreditoDetalleDto", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class CreditoDetalleDto : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Monto { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PlazoMeses { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FechaAprobacion { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Estado { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ClienteDetalleResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class ClienteDetalleResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal SaldoAhorros { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<BancoAdminService.CreditoDetalleDto> Creditos { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ActualizarClienteBancoResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class ActualizarClienteBancoResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="EliminarClienteBancoResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class EliminarClienteBancoResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
    }

    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BancoAdminService.IBancoAdminService")]
    public interface IBancoAdminService
    {
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBancoAdminService/CrearClienteBanco", ReplyAction="http://tempuri.org/IBancoAdminService/CrearClienteBancoResponse")]
        System.Threading.Tasks.Task<BancoAdminService.CrearClienteBancoResponse> CrearClienteBancoAsync(string sessionToken, BancoAdminService.CrearClienteBancoRequest request);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBancoAdminService/ListarClientesBanco", ReplyAction="http://tempuri.org/IBancoAdminService/ListarClientesBancoResponse")]
        System.Threading.Tasks.Task<BancoAdminService.ListarClientesBancoResponse> ListarClientesBancoAsync(string sessionToken);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBancoAdminService/ActualizarClienteBanco", ReplyAction="http://tempuri.org/IBancoAdminService/ActualizarClienteBancoResponse")]
        System.Threading.Tasks.Task<BancoAdminService.ActualizarClienteBancoResponse> ActualizarClienteBancoAsync(string sessionToken, BancoAdminService.ClienteBancoDto cliente);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBancoAdminService/EliminarClienteBanco", ReplyAction="http://tempuri.org/IBancoAdminService/EliminarClienteBancoResponse")]
        System.Threading.Tasks.Task<BancoAdminService.EliminarClienteBancoResponse> EliminarClienteBancoAsync(string sessionToken, string cedula);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBancoAdminService/ObtenerClienteDetalle", ReplyAction="http://tempuri.org/IBancoAdminService/ClienteDetalleResponse")]
        System.Threading.Tasks.Task<BancoAdminService.ClienteDetalleResponse> ObtenerClienteDetalleAsync(string sessionToken, string cedula);
    }
    
    public interface IBancoAdminServiceChannel : BancoAdminService.IBancoAdminService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    public partial class BancoAdminServiceClient : System.ServiceModel.ClientBase<BancoAdminService.IBancoAdminService>, BancoAdminService.IBancoAdminService
    {
        public BancoAdminServiceClient() : 
                base(BancoAdminServiceClient.GetDefaultBinding(), BancoAdminServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IBancoAdminService.ToString();
        }
        
        public System.Threading.Tasks.Task<BancoAdminService.CrearClienteBancoResponse> CrearClienteBancoAsync(string sessionToken, BancoAdminService.CrearClienteBancoRequest request)
        {
            return base.Channel.CrearClienteBancoAsync(sessionToken, request);
        }

        public System.Threading.Tasks.Task<BancoAdminService.ListarClientesBancoResponse> ListarClientesBancoAsync(string sessionToken)
        {
            return base.Channel.ListarClientesBancoAsync(sessionToken);
        }

        public System.Threading.Tasks.Task<BancoAdminService.ActualizarClienteBancoResponse> ActualizarClienteBancoAsync(string sessionToken, BancoAdminService.ClienteBancoDto cliente)
        {
            return base.Channel.ActualizarClienteBancoAsync(sessionToken, cliente);
        }

        public System.Threading.Tasks.Task<BancoAdminService.EliminarClienteBancoResponse> EliminarClienteBancoAsync(string sessionToken, string cedula)
        {
            return base.Channel.EliminarClienteBancoAsync(sessionToken, cedula);
        }

        public System.Threading.Tasks.Task<BancoAdminService.ClienteDetalleResponse> ObtenerClienteDetalleAsync(string sessionToken, string cedula)
        {
            return base.Channel.ObtenerClienteDetalleAsync(sessionToken, cedula);
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
            result.MaxBufferSize = int.MaxValue;
            result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            // result.AllowCookies = true;
            return result;
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return new System.ServiceModel.EndpointAddress("http://192.168.0.137:9099/BancoAdminService.svc");
        }
        
        public enum EndpointConfiguration
        {
            BasicHttpBinding_IBancoAdminService,
        }
    }
}
