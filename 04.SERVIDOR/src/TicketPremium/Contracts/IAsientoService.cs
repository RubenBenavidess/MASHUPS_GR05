using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class AsientoDto
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string Fila { get; set; } = string.Empty;

    [DataMember]
    public int Numero { get; set; }

    [DataMember]
    public string Estado { get; set; } = string.Empty;

    [DataMember]
    public string LocalidadCodigo { get; set; } = string.Empty;

    [DataMember]
    public string PartidoCodigo { get; set; } = string.Empty;
}

[ServiceContract]
public interface IAsientoService
{
    [OperationContract]
    Task<List<AsientoDto>> ListarAsientosPorPartido(string sessionToken, string partidoCodigo);
}
