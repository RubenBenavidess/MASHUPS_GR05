using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.AppFIFA.Contracts;

[DataContract]
public class PartidoDisponible
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string EquipoLocal { get; set; } = string.Empty;

    [DataMember]
    public string EquipoVisitante { get; set; } = string.Empty;

    [DataMember]
    public DateTime FechaHora { get; set; }

    [DataMember]
    public string Lugar { get; set; } = string.Empty;
}

[ServiceContract]
public interface IFifaPartidoService
{
    [OperationContract]
    Task<List<PartidoDisponible>> ListarPartidosDisponibles();
}
