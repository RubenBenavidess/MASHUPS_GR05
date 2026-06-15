using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class ClienteDto
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
    public string Telefono { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Rol { get; set; } = string.Empty;
}

[ServiceContract]
public interface IClienteService
{
    [OperationContract]
    Task<List<ClienteDto>> ListarClientes(string sessionToken);

    [OperationContract]
    Task<ClienteDto> ObtenerCliente(string sessionToken, string cedula);

    [OperationContract]
    Task CrearCliente(string sessionToken, ClienteDto cliente);

    [OperationContract]
    Task ActualizarCliente(string sessionToken, ClienteDto cliente);

    [OperationContract]
    Task EliminarCliente(string sessionToken, string cedula);
}
