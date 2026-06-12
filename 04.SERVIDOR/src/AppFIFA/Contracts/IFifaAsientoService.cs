using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.AppFIFA.Contracts;

[DataContract]
public class LocalidadDetalle
{
    [DataMember]
    public string Codigo { get; set; } = string.Empty;

    [DataMember]
    public string Descripcion { get; set; } = string.Empty;

    [DataMember]
    public decimal PrecioBase { get; set; }

    [DataMember]
    public int Capacidad { get; set; }

    [DataMember]
    public int AsientosDisponibles { get; set; }

    [DataMember]
    public List<AsientoDetalle> Asientos { get; set; } = new List<AsientoDetalle>();
}

[DataContract]
public class AsientoDetalle
{
    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public string Fila { get; set; } = string.Empty;

    [DataMember]
    public int Numero { get; set; }

    [DataMember]
    public string Estado { get; set; } = string.Empty;
}

[DataContract]
public class ReservaResponse
{
    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

[DataContract]
public class VentaResponse
{
    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

[DataContract]
public class LiberacionResponse
{
    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

[DataContract]
public class ExpirarResponse
{
    [DataMember]
    public int AsientosExpirados { get; set; }

    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

[ServiceContract]
public interface IFifaAsientoService
{
    [OperationContract]
    Task<List<LocalidadDetalle>> ListarLocalidadesPorPartido(string codigoPartido);

    [OperationContract]
    Task<ReservaResponse> ReservarAsiento(string codigoAsiento);

    [OperationContract]
    Task<VentaResponse> ConfirmarVenta(string codigoAsiento);

    [OperationContract]
    Task<LiberacionResponse> LiberarAsiento(string codigoAsiento);

    [OperationContract]
    Task<ExpirarResponse> ExpirarReservasVencidas();
}
