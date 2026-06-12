using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class LocalidadDto
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string Descripcion { get; set; } = string.Empty;

    [DataMember]
    public int Capacidad { get; set; }

    [DataMember]
    public decimal PrecioBase { get; set; }

    [DataMember]
    public string EstadioCodigo { get; set; } = string.Empty;
}

[ServiceContract]
public interface ILocalidadService
{
    [OperationContract]
    Task<List<LocalidadDto>> ListarLocalidades();

    [OperationContract]
    Task<LocalidadDto> ObtenerLocalidad(string codigo);

    [OperationContract]
    Task CrearLocalidad(LocalidadDto localidad);

    [OperationContract]
    Task ActualizarLocalidad(LocalidadDto localidad);

    [OperationContract]
    Task EliminarLocalidad(string codigo);
}
