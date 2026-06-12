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
}

[ServiceContract]
public interface ICompraService
{
    [OperationContract]
    Task<CompraResponse> ComprarEnEfectivo(List<string> codigosAsiento, string clienteCedula);

    [OperationContract]
    Task<CompraResponse> ComprarACredito(List<string> codigosAsiento, string clienteCedula, int plazoMeses);
}
