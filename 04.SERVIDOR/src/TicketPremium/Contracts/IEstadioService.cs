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
    Task<List<EstadioDto>> ListarEstadios();

    [OperationContract]
    Task<EstadioDto> ObtenerEstadio(string codigo);

    [OperationContract]
    Task CrearEstadio(EstadioDto estadio);

    [OperationContract]
    Task ActualizarEstadio(EstadioDto estadio);

    [OperationContract]
    Task EliminarEstadio(string codigo);
}
