using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class ResumenLocalidadDto
{
    [DataMember]
    public string CodigoLocalidad { get; set; } = string.Empty;

    [DataMember]
    public string DescripcionLocalidad { get; set; } = string.Empty;

    [DataMember]
    public int BoletosVendidos { get; set; }

    [DataMember]
    public decimal TotalRecaudado { get; set; }
}

[DataContract]
public class ResumenClienteDto
{
    [DataMember]
    public string NumeroFactura { get; set; } = string.Empty;

    [DataMember]
    public DateTime Fecha { get; set; }

    [DataMember]
    public string Partido { get; set; } = string.Empty;

    [DataMember]
    public string Asiento { get; set; } = string.Empty;

    [DataMember]
    public string Localidad { get; set; } = string.Empty;

    [DataMember]
    public decimal PrecioUnitario { get; set; }

    [DataMember]
    public string MetodoPago { get; set; } = string.Empty;
}

[ServiceContract]
public interface IReporteService
{
    [OperationContract]
    Task<List<ResumenLocalidadDto>> ResumenVentasPorPartido(string codigoPartido);

    [OperationContract]
    Task<List<ResumenClienteDto>> ResumenVentasPorCliente(string cedulaCliente);
}
