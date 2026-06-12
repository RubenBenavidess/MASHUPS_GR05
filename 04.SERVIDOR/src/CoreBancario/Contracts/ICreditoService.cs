using System.Runtime.Serialization;
using System.ServiceModel;

namespace ec.edu.monster.CoreBancario.Contracts
{
    [DataContract]
    public class VerificacionCreditoResponse
    {
        [DataMember]
        public bool Aprobado { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class MontoMaximoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public decimal MontoMaximo { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract]
    public class RegistroCreditoResponse
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public int CreditoCodigo { get; set; }

        [DataMember]
        public string Mensaje { get; set; } = string.Empty;
    }

    [ServiceContract]
    public interface ICreditoService
    {
        [OperationContract]
        Task<VerificacionCreditoResponse> VerificarSujetoCredito(string cedula);

        [OperationContract]
        Task<MontoMaximoResponse> ObtenerMontoMaximo(string cedula);

        [OperationContract]
        Task<RegistroCreditoResponse> RegistrarCredito(string cedula, decimal monto, int plazoMeses);
    }
}
