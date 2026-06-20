namespace AsientoService
{
    using System.Runtime.Serialization;
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="AsientoDto", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class AsientoDto : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Codigo { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Fila { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Numero { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Estado { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LocalidadCodigo { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PartidoCodigo { get; set; }
    }
    
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AsientoService.IAsientoService")]
    public interface IAsientoService
    {
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAsientoService/ListarAsientosPorPartido", ReplyAction="http://tempuri.org/IAsientoService/ListarAsientosPorPartidoResponse")]
        System.Threading.Tasks.Task<AsientoDto[]> ListarAsientosPorPartidoAsync(string sessionToken, string partidoCodigo);
    }
    
    public interface IAsientoServiceChannel : AsientoService.IAsientoService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    public partial class AsientoServiceClient : System.ServiceModel.ClientBase<AsientoService.IAsientoService>, AsientoService.IAsientoService
    {
        public AsientoServiceClient() : 
                base(AsientoServiceClient.GetDefaultBinding(), AsientoServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IAsientoService.ToString();
        }
        
        public System.Threading.Tasks.Task<AsientoDto[]> ListarAsientosPorPartidoAsync(string sessionToken, string partidoCodigo)
        {
            return base.Channel.ListarAsientosPorPartidoAsync(sessionToken, partidoCodigo);
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
            result.MaxBufferSize = int.MaxValue;
            result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            result.AllowCookies = true;
            return result;
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return new System.ServiceModel.EndpointAddress("http://192.168.0.137:9099/AsientoService.svc");
        }
        
        public enum EndpointConfiguration
        {
            BasicHttpBinding_IAsientoService,
        }
    }
}
