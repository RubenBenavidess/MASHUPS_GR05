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

[DataContract]
    public class DetalleVentaAsientoDto
{
    [DataMember]
    public string NumeroFactura { get; set; } = string.Empty;

    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public string Fila { get; set; } = string.Empty;

    [DataMember]
    public int Numero { get; set; }

    [DataMember]
    public string CedulaCliente { get; set; } = string.Empty;

    [DataMember]
    public string NombreCliente { get; set; } = string.Empty;

    [DataMember]
    public string ApellidoCliente { get; set; } = string.Empty;

    [DataMember]
    public DateTime FechaCompra { get; set; }
}

[ServiceContract]
public interface IReporteService
{
    [OperationContract]
    Task<List<ResumenLocalidadDto>> ResumenVentasPorPartido(string sessionToken, string codigoPartido);

    [OperationContract]
    Task<List<ResumenClienteDto>> ResumenVentasPorCliente(string sessionToken, string cedulaCliente);

    [OperationContract]
    Task<List<DetalleVentaAsientoDto>> DetallesVentasPorPartido(string sessionToken, string codigoPartido);
}

