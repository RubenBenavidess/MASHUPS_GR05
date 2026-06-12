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
}

[ServiceContract]
public interface IClienteService
{
    [OperationContract]
    Task<List<ClienteDto>> ListarClientes();

    [OperationContract]
    Task<ClienteDto> ObtenerCliente(string cedula);

    [OperationContract]
    Task CrearCliente(ClienteDto cliente);

    [OperationContract]
    Task ActualizarCliente(ClienteDto cliente);

    [OperationContract]
    Task EliminarCliente(string cedula);
}
