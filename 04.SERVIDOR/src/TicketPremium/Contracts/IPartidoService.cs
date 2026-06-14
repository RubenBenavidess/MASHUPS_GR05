using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class PartidoDto
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
    public string EstadioCodigo { get; set; } = string.Empty;
}

[ServiceContract]
public interface IPartidoService
{
    [OperationContract]
    Task<List<PartidoDto>> ListarPartidos();

    [OperationContract]
    Task<PartidoDto> ObtenerPartido(string codigo);

    [OperationContract]
    Task CrearPartido(PartidoDto partido);

    [OperationContract]
    Task ActualizarPartido(PartidoDto partido);

    [OperationContract]
    Task EliminarPartido(string codigo);

    // En tu interface IPartidoService
    //[OperationContract]
    //Task<List<AsientoDto>> ListarAsientosPorPartido(string codigoPartido);
}
