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
    Task<List<PaisDto>> ListarPaises();

    [OperationContract]
    Task<PaisDto> ObtenerPais(string codigo);

    [OperationContract]
    Task CrearPais(PaisDto pais);

    [OperationContract]
    Task ActualizarPais(PaisDto pais);

    [OperationContract]
    Task EliminarPais(string codigo);
}
