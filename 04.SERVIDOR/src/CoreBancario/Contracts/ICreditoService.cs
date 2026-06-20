using System.Runtime.Serialization;
using System.ServiceModel;

namespace ec.edu.monster.CoreBancario.Contracts
{
    [DataContract]
    public class VerificacionCreditoResponse
    {
        [DataMember]
        public bool Aprobado { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class AmortizacionDto
    {
        [DataMember]
        public int NumeroCuota { get; set; }
        [DataMember]
        public decimal ValorCuota { get; set; }
        [DataMember]
        public decimal InteresPagado { get; set; }
        [DataMember]
        public decimal CapitalPagado { get; set; }
        [DataMember]
        public decimal Saldo { get; set; }
        [DataMember]
        public DateTime FechaPago { get; set; }
    }

    [DataContract]
    public class MontoMaximoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public decimal MontoMaximo { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class RegistroCreditoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public int CreditoCodigo { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;

        [DataMember]
        public List<AmortizacionDto> Amortizaciones { get; set; } = new();
    }

    [DataContract]
    public class CrearClienteResponse
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
    public class ActualizarClienteResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }
        
        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class EliminarClienteResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }
        
        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class ListarClientesResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;

        [DataMember]
        public List<ClienteBancoDto> Clientes { get; set; } = new();
    }

    [ServiceContract]
    public interface ICreditoService
    {
        [OperationContract]
        Task<VerificacionCreditoResponse> VerificarSujetoCredito(string cedula);

        [OperationContract]
        Task<MontoMaximoResponse> ObtenerMontoMaximo(string cedula);

        [OperationContract]
        Task<RegistroCreditoResponse> RegistrarCredito(string cedula, decimal monto, int plazoMeses);

        [OperationContract]
        Task<CrearClienteResponse> CrearClienteConCuenta(string cedula, string nombre, string apellido, DateTime fechaNacimiento, string genero, decimal depositoInicial);

        [OperationContract]
        Task<ListarClientesResponse> ListarClientes();

        [OperationContract]
        Task<ActualizarClienteResponse> ActualizarCliente(ClienteBancoDto cliente);

        [OperationContract]
        Task<EliminarClienteResponse> EliminarCliente(string cedula);

        [OperationContract]
        Task<ClienteDetalleResponse> ObtenerClienteDetalle(string cedula);
    }
}
