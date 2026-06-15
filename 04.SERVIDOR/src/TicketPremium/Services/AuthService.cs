using System.Collections.Concurrent;
using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using ec.edu.monster.TicketPremium.Utils;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class AuthService : IAuthService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<AuthService> _logger;

    private static readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();
    private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);

    private record SessionInfo(string Cedula, string Rol, DateTime ExpiresAt);

    public AuthService(TicketPremiumDbContext db, ILogger<AuthService> logger)
    {
        _db = db;
        _logger = logger;
        LimpiarSesionesExpiradas();
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=Login | Cedula={Cedula}", ts, request.Cedula);

        if (string.IsNullOrWhiteSpace(request.Cedula) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse { Exitoso = false, Mensaje = "Cedula y contrasena son obligatorias." };
        }

        if (!CedulaValidator.EsValida(request.Cedula, out var errorCedula))
        {
            return new LoginResponse { Exitoso = false, Mensaje = errorCedula };
        }

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Cedula == request.Cedula);

        if (cliente == null)
        {
            _logger.LogWarning("[{Timestamp}] Login=Failed | Reason=NotFound | Cedula={Cedula}", ts, request.Cedula);
            return new LoginResponse { Exitoso = false, Mensaje = "Cliente no registrado." };
        }

        if (string.IsNullOrWhiteSpace(cliente.PasswordHash))
        {
            _logger.LogWarning("[{Timestamp}] Login=Failed | Reason=NoPassword | Cedula={Cedula}", ts, request.Cedula);
            return new LoginResponse { Exitoso = false, Mensaje = "Cuenta no activada. Contacte al administrador." };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, cliente.PasswordHash))
        {
            _logger.LogWarning("[{Timestamp}] Login=Failed | Reason=InvalidPassword | Cedula={Cedula}", ts, request.Cedula);
            return new LoginResponse { Exitoso = false, Mensaje = "Contrasena incorrecta." };
        }

        var token = Guid.NewGuid().ToString("N");
        _sessions[token] = new SessionInfo(cliente.Cedula, cliente.Rol, DateTime.UtcNow.Add(SessionTimeout));

        _logger.LogInformation("[{Timestamp}] Login=Success | Cedula={Cedula} | Rol={Rol}", ts, cliente.Cedula, cliente.Rol);

        return new LoginResponse
        {
            Exitoso = true,
            SessionToken = token,
            Nombre = $"{cliente.Nombre} {cliente.Apellido}",
            Rol = cliente.Rol,
            Mensaje = "Inicio de sesion exitoso."
        };
    }

    public async Task<LoginResponse> Registro(RegistroRequest request)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=Registro | Cedula={Cedula}", ts, request.Cedula);

        if (string.IsNullOrWhiteSpace(request.Cedula) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse { Exitoso = false, Mensaje = "Cedula y contrasena son obligatorias." };
        }

        if (!CedulaValidator.EsValida(request.Cedula, out var errorCedula))
        {
            return new LoginResponse { Exitoso = false, Mensaje = errorCedula };
        }

        if (await _db.Clientes.AnyAsync(c => c.Cedula == request.Cedula))
        {
            return new LoginResponse { Exitoso = false, Mensaje = "Ya existe un cliente con esa cedula." };
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 11);

        var cliente = new Cliente
        {
            Cedula = request.Cedula,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,
            Telefono = request.Telefono,
            Genero = request.Genero,
            FechaNacimiento = request.FechaNacimiento,
            PasswordHash = passwordHash,
            Rol = "CLIENTE"
        };

        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync();

        var token = Guid.NewGuid().ToString("N");
        _sessions[token] = new SessionInfo(cliente.Cedula, cliente.Rol, DateTime.UtcNow.Add(SessionTimeout));

        _logger.LogInformation("[{Timestamp}] Registro=Success | Cedula={Cedula}", ts, cliente.Cedula);

        return new LoginResponse
        {
            Exitoso = true,
            SessionToken = token,
            Nombre = $"{cliente.Nombre} {cliente.Apellido}",
            Rol = cliente.Rol,
            Mensaje = "Registro exitoso. Sesion iniciada."
        };
    }

    public Task<ValidarTokenResponse> ValidarToken(string sessionToken)
    {
        LimpiarSesionesExpiradas();

        if (string.IsNullOrWhiteSpace(sessionToken) || !_sessions.TryGetValue(sessionToken, out var session))
        {
            return Task.FromResult(new ValidarTokenResponse
            {
                Exitoso = false,
                Mensaje = "Sesion invalida o expirada. Inicie sesion nuevamente."
            });
        }

        if (DateTime.UtcNow > session.ExpiresAt)
        {
            _sessions.TryRemove(sessionToken, out _);
            return Task.FromResult(new ValidarTokenResponse
            {
                Exitoso = false,
                Mensaje = "Sesion expirada. Inicie sesion nuevamente."
            });
        }

        // Extender sesion
        _sessions[sessionToken] = session with { ExpiresAt = DateTime.UtcNow.Add(SessionTimeout) };

        return Task.FromResult(new ValidarTokenResponse
        {
            Exitoso = true,
            Cedula = session.Cedula,
            Rol = session.Rol,
            Mensaje = "Token valido."
        });
    }

    public static bool EsAdmin(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken)) return false;
        if (!_sessions.TryGetValue(sessionToken, out var session)) return false;
        if (DateTime.UtcNow > session.ExpiresAt)
        {
            _sessions.TryRemove(sessionToken, out _);
            return false;
        }
        return session.Rol == "ADMIN";
    }

    public static bool EsClienteValido(string sessionToken, out string cedula)
    {
        cedula = string.Empty;
        if (string.IsNullOrWhiteSpace(sessionToken)) return false;
        if (!_sessions.TryGetValue(sessionToken, out var session)) return false;
        if (DateTime.UtcNow > session.ExpiresAt)
        {
            _sessions.TryRemove(sessionToken, out _);
            return false;
        }
        cedula = session.Cedula;
        return session.Rol == "ADMIN" || session.Rol == "CLIENTE";
    }

    private static void LimpiarSesionesExpiradas()
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _sessions)
        {
            if (now > kvp.Value.ExpiresAt)
                _sessions.TryRemove(kvp.Key, out _);
        }
    }
}
