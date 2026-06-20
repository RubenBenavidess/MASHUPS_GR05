namespace AuthService
{
    using System.Runtime.Serialization;
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="LoginRequest", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class LoginRequest : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password { get; set; }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="RegistroRequest", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class RegistroRequest : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Apellido { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Telefono { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Genero { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FechaNacimiento { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password { get; set; }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="LoginResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class LoginResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SessionToken { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Rol { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DebeCambiarPassword { get; set; }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="CambiarPasswordRequest", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class CambiarPasswordRequest : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NuevaPassword { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ValidarTokenResponse", Namespace="http://schemas.datacontract.org/2004/07/ec.edu.monster.TicketPremium.Contracts")]
    public partial class ValidarTokenResponse : object
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Exitoso { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cedula { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Rol { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje { get; set; }
    }
    
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthService.IAuthService")]
    public interface IAuthService
    {
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/Login", ReplyAction="http://tempuri.org/IAuthService/LoginResponse")]
        System.Threading.Tasks.Task<AuthService.LoginResponse> LoginAsync(AuthService.LoginRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/Registro", ReplyAction="http://tempuri.org/IAuthService/RegistroResponse")]
        System.Threading.Tasks.Task<AuthService.LoginResponse> RegistroAsync(AuthService.RegistroRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/ValidarToken", ReplyAction="http://tempuri.org/IAuthService/ValidarTokenResponse")]
        System.Threading.Tasks.Task<AuthService.ValidarTokenResponse> ValidarTokenAsync(string sessionToken);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/CambiarPasswordPrimeraVez", ReplyAction="http://tempuri.org/IAuthService/CambiarPasswordPrimeraVezResponse")]
        System.Threading.Tasks.Task<AuthService.LoginResponse> CambiarPasswordPrimeraVezAsync(AuthService.CambiarPasswordRequest request);
    }
    
    public interface IAuthServiceChannel : AuthService.IAuthService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    public partial class AuthServiceClient : System.ServiceModel.ClientBase<AuthService.IAuthService>, AuthService.IAuthService
    {
        public AuthServiceClient() : 
                base(AuthServiceClient.GetDefaultBinding(), AuthServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IAuthService.ToString();
        }
        
        public System.Threading.Tasks.Task<AuthService.LoginResponse> LoginAsync(AuthService.LoginRequest request)
        {
            return base.Channel.LoginAsync(request);
        }
        
        public System.Threading.Tasks.Task<AuthService.LoginResponse> RegistroAsync(AuthService.RegistroRequest request)
        {
            return base.Channel.RegistroAsync(request);
        }
        
        public System.Threading.Tasks.Task<AuthService.ValidarTokenResponse> ValidarTokenAsync(string sessionToken)
        {
            return base.Channel.ValidarTokenAsync(sessionToken);
        }

        public System.Threading.Tasks.Task<AuthService.LoginResponse> CambiarPasswordPrimeraVezAsync(AuthService.CambiarPasswordRequest request)
        {
            return base.Channel.CambiarPasswordPrimeraVezAsync(request);
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
            return new System.ServiceModel.EndpointAddress("http://192.168.0.137:9099/AuthService.svc");
        }
        
        public enum EndpointConfiguration
        {
            BasicHttpBinding_IAuthService,
        }
    }
}

