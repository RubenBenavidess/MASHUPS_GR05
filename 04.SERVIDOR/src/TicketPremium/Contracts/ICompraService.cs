using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class CompraResponse
{
    [DataMember]
    public bool Exitoso { get; set; }

    [DataMember]
    public string NumeroFactura { get; set; } = string.Empty;

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;

    [DataMember]
    public List<AmortizacionDto> Amortizaciones { get; set; } = new();
}

[DataContract]
public class AmortizacionDto
{
    [DataMember]
    public int NumeroCuota { get; set; }
    [DataMember]
    public decimal ValorCuota { get; set; }
    [DataMember]
    public decimal InteresPagado { get; set; }
    [DataMember]
    public decimal CapitalPagado { get; set; }
    [DataMember]
    public decimal Saldo { get; set; }
    [DataMember]
    public DateTime FechaPago { get; set; }
}

[ServiceContract]
public interface ICompraService
{
    [OperationContract]
    Task<CompraResponse> ComprarEnEfectivo(string sessionToken, List<string> codigosAsiento, string clienteCedula);

    [OperationContract]
    Task<CompraResponse> ComprarACredito(string sessionToken, List<string> codigosAsiento, string clienteCedula, int plazoMeses);
}
