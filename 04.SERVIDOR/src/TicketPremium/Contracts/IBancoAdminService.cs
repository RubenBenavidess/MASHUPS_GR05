using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class CrearClienteBancoRequest
{
    [DataMember]
    public string Cedula { get; set; } = string.Empty;

    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [DataMember]
    public string Apellido { get; set; } = string.Empty;

    [DataMember]
    public DateTime FechaNacimiento { get; set; }

    [DataMember]
    public string Genero { get; set; } = string.Empty;

    [DataMember]
    public decimal DepositoInicial { get; set; }
}

[DataContract]
public class CrearClienteBancoResponse
{
    [DataMember]
    public bool Exitoso { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

    [DataContract]
    public class ClienteBancoDto
    {
        [DataMember]
        public string Cedula { get; set; } = string.Empty;

        [DataMember]
        public string Nombre { get; set; } = string.Empty;

        [DataMember]
        public string Apellido { get; set; } = string.Empty;

        [DataMember]
        public DateTime FechaNacimiento { get; set; }

        [DataMember]
        public string Genero { get; set; } = string.Empty;

        [DataMember]
        public string Estado { get; set; } = string.Empty;
    }

    [DataContract]
    public class CreditoDetalleDto
    {
        [DataMember]
        public decimal Monto { get; set; }
        
        [DataMember]
        public int PlazoMeses { get; set; }
        
        [DataMember]
        public DateTime FechaAprobacion { get; set; }
        
        [DataMember]
        public string Estado { get; set; } = string.Empty;
    }

    [DataContract]
    public class ClienteDetalleResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }
        
        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
        
        [DataMember]
        public decimal SaldoAhorros { get; set; }
        
        [DataMember]
        public List<CreditoDetalleDto> Creditos { get; set; } = new();
    }

    [DataContract]
    public class ActualizarClienteBancoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }
        
        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class EliminarClienteBancoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }
        
        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class ListarClientesBancoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;

        [DataMember]
        public List<ClienteBancoDto> Clientes { get; set; } = new();
    }

    [ServiceContract]
    public interface IBancoAdminService
    {
        [OperationContract]
        Task<CrearClienteBancoResponse> CrearClienteBanco(string sessionToken, CrearClienteBancoRequest request);

        [OperationContract]
        Task<ListarClientesBancoResponse> ListarClientesBanco(string sessionToken);

        [OperationContract]
        Task<ActualizarClienteBancoResponse> ActualizarClienteBanco(string sessionToken, ClienteBancoDto cliente);

        [OperationContract]
        Task<EliminarClienteBancoResponse> EliminarClienteBanco(string sessionToken, string cedula);

        [OperationContract]
        Task<ClienteDetalleResponse> ObtenerClienteDetalle(string sessionToken, string cedula);
    }
