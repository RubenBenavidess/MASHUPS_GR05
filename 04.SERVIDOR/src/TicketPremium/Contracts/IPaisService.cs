using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class PaisDto
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [DataMember]
    public string Continente { get; set; } = string.Empty;
}

[ServiceContract]
public interface IPaisService
{
    [OperationContract]
    Task<List<PaisDto>> ListarPaises(string sessionToken);

    [OperationContract]
    Task<PaisDto> ObtenerPais(string sessionToken, string codigo);

    [OperationContract]
    Task CrearPais(string sessionToken, PaisDto pais);

    [OperationContract]
    Task ActualizarPais(string sessionToken, PaisDto pais);

    [OperationContract]
    Task EliminarPais(string sessionToken, string codigo);
}
