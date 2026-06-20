using System.Runtime.Serialization;
using CoreWCF;

namespace ec.edu.monster.TicketPremium.Contracts;

[DataContract]
public class LoginRequest
{
    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}

[DataContract]
public class LoginResponse
{
    [DataMember]
    public bool Exitoso { get; set; }

    [DataMember]
    public string Cedula { get; set; } = string.Empty;

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [DataMember]
    public string Rol { get; set; } = string.Empty;

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;

    [DataMember]
    public bool DebeCambiarPassword { get; set; }
}

[DataContract]
public class RegistroRequest
{
    [DataMember]
    public string Cedula { get; set; } = string.Empty;

    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [DataMember]
    public string Apellido { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Telefono { get; set; } = string.Empty;

    [DataMember]
    public string Genero { get; set; } = string.Empty;

    [DataMember]
    public DateTime FechaNacimiento { get; set; }

    [DataMember]
    public string Password { get; set; } = string.Empty;
}

[DataContract]
public class ValidarTokenResponse
{
    [DataMember]
    public bool Exitoso { get; set; }

    [DataMember]
    public string Cedula { get; set; } = string.Empty;

    [DataMember]
    public string Rol { get; set; } = string.Empty;

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
}

[DataContract]
public class CambiarPasswordRequest
{
    [DataMember]
    public string Cedula { get; set; } = string.Empty;

    [DataMember]
    public string NuevaPassword { get; set; } = string.Empty;
}

[ServiceContract]
public interface IAuthService
{
    [OperationContract]
    Task<LoginResponse> Login(LoginRequest request);

    [OperationContract]
    Task<LoginResponse> Registro(RegistroRequest request);

    [OperationContract]
    Task<ValidarTokenResponse> ValidarToken(string sessionToken);

    [OperationContract]
    Task<LoginResponse> CambiarPasswordPrimeraVez(CambiarPasswordRequest request);
}
