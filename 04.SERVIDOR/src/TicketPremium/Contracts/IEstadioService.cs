using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class EstadioDto
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [DataMember]
    public string Ciudad { get; set; } = string.Empty;

    [DataMember]
    public int CapacidadTotal { get; set; }

    [DataMember]
    public string PaisCodigo { get; set; } = string.Empty;
}

[ServiceContract]
public interface IEstadioService
{
    [OperationContract]
    Task<List<EstadioDto>> ListarEstadios(string sessionToken);

    [OperationContract]
    Task<EstadioDto> ObtenerEstadio(string sessionToken, string codigo);

    [OperationContract]
    Task CrearEstadio(string sessionToken, EstadioDto estadio);

    [OperationContract]
    Task ActualizarEstadio(string sessionToken, EstadioDto estadio);

    [OperationContract]
    Task EliminarEstadio(string sessionToken, string codigo);
}
