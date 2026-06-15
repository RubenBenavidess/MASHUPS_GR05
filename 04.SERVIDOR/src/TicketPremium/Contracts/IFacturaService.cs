using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class FacturaItemDto
{
    [DataMember]
    public string CodigoAsiento { get; set; } = string.Empty;

    [DataMember]
    public string Localidad { get; set; } = string.Empty;

    [DataMember]
    public string Partido { get; set; } = string.Empty;

    [DataMember]
    public decimal PrecioUnitario { get; set; }
}

[DataContract]
public class FacturaResponse
{
    [DataMember]
    public string Numero { get; set; } = string.Empty;

    [DataMember]
    public DateTime Fecha { get; set; }

    [DataMember]
    public decimal Subtotal { get; set; }

    [DataMember]
    public decimal Descuento { get; set; }

    [DataMember]
    public decimal Iva { get; set; }

    [DataMember]
    public decimal Total { get; set; }

    [DataMember]
    public string MetodoPago { get; set; } = string.Empty;

    [DataMember]
    public string ClienteCedula { get; set; } = string.Empty;

    [DataMember]
    public List<FacturaItemDto> Items { get; set; } = new List<FacturaItemDto>();
}

[ServiceContract]
public interface IFacturaService
{
    [OperationContract]
    Task<FacturaResponse> CalcularFactura(string sessionToken, List<string> codigosAsiento, bool esEfectivo, string clienteCedula);
}
