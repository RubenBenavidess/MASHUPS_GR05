using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
using ec.edu.monster.TicketPremium.Utils;
using Microsoft.EntityFrameworkCore;

namespace ec.edu.monster.TicketPremium.Services;

public class ClienteService : IClienteService
{
    private readonly TicketPremiumDbContext _db;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(TicketPremiumDbContext db, ILogger<ClienteService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<ClienteDto>> ListarClientes(string sessionToken)
    {
        RequerirAdmin(sessionToken);
        var ts = Now();
        _logger.LogInformation("[{T}] Operation=ListarClientes", ts);

        var result = await _db.Clientes
            .OrderBy(c => c.Apellido)
            .Select(c => new ClienteDto
            {
                Cedula = c.Cedula, Nombre = c.Nombre, Apellido = c.Apellido,
                FechaNacimiento = c.FechaNacimiento, Genero = c.Genero ?? "",
                Telefono = c.Telefono ?? "", Email = c.Email ?? "", Rol = c.Rol
            })
            .ToListAsync();
        return result;
    }

    public async Task<ClienteDto> ObtenerCliente(string sessionToken, string cedula)
    {
        RequerirAdminOPropio(sessionToken, cedula);
        var cliente = await _db.Clientes.FindAsync(cedula)
            ?? throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));

        return new ClienteDto
        {
            Cedula = cliente.Cedula, Nombre = cliente.Nombre, Apellido = cliente.Apellido,
            FechaNacimiento = cliente.FechaNacimiento, Genero = cliente.Genero ?? "",
            Telefono = cliente.Telefono ?? "", Email = cliente.Email ?? "", Rol = cliente.Rol
        };
    }

    public async Task CrearCliente(string sessionToken, ClienteDto cliente)
    {
        RequerirAdmin(sessionToken);
        ValidarCedula(cliente.Cedula);
        if (await _db.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula))
            throw new FaultException(new FaultReason("Ya existe un cliente con esa cedula"), new FaultCode("Duplicate"));

        _db.Clientes.Add(new Cliente
        {
            Cedula = cliente.Cedula, Nombre = cliente.Nombre, Apellido = cliente.Apellido,
            FechaNacimiento = cliente.FechaNacimiento, Genero = cliente.Genero,
            Telefono = cliente.Telefono, Email = cliente.Email, Rol = "CLIENTE", PasswordHash = ""
        });
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarCliente(string sessionToken, ClienteDto cliente)
    {
        RequerirAdmin(sessionToken);
        ValidarCedula(cliente.Cedula);
        var existing = await _db.Clientes.FindAsync(cliente.Cedula)
            ?? throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));

        existing.Nombre = cliente.Nombre; existing.Apellido = cliente.Apellido;
        existing.FechaNacimiento = cliente.FechaNacimiento; existing.Genero = cliente.Genero;
        existing.Telefono = cliente.Telefono; existing.Email = cliente.Email;
        await _db.SaveChangesAsync();
    }

    public async Task EliminarCliente(string sessionToken, string cedula)
    {
        RequerirAdmin(sessionToken);
        var cliente = await _db.Clientes.FindAsync(cedula)
            ?? throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));
        _db.Clientes.Remove(cliente);
        await _db.SaveChangesAsync();
    }

    private static void RequerirAdmin(string sessionToken)
    {
        if (!AuthService.EsAdmin(sessionToken))
            throw new FaultException(new FaultReason("Acceso denegado. Se requiere rol ADMIN."), new FaultCode("AccesoDenegado"));
    }

    private static void RequerirAdminOPropio(string sessionToken, string cedula)
    {
        if (AuthService.EsAdmin(sessionToken)) return;
        if (AuthService.EsClienteValido(sessionToken, out var c) && c == cedula) return;
        throw new FaultException(new FaultReason("Acceso denegado."), new FaultCode("AccesoDenegado"));
    }

    private static void ValidarCedula(string cedula)
    {
        if (!CedulaValidator.EsValida(cedula, out var error))
            throw new FaultException(new FaultReason(error), new FaultCode("CedulaInvalida"));
    }

    private static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
}
