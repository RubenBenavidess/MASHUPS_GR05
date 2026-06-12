using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Data;
using ec.edu.monster.TicketPremium.Models;
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

    public async Task<List<ClienteDto>> ListarClientes()
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ListarClientes | Params=None", ts);

        var result = await _db.Clientes
            .OrderBy(c => c.Apellido)
            .Select(c => new ClienteDto
            {
                Cedula = c.Cedula,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FechaNacimiento = c.FechaNacimiento,
                Genero = c.Genero,
                Telefono = c.Telefono,
                Email = c.Email
            })
            .ToListAsync();

        _logger.LogInformation("[{Timestamp}] Operation=ListarClientes | ResultCount={Count}", ts, result.Count);
        return result;
    }

    public async Task<ClienteDto> ObtenerCliente(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ObtenerCliente | Params=cedula={Cedula}", ts, cedula);

        var cliente = await _db.Clientes.FindAsync(cedula);
        if (cliente == null)
        {
            throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));
        }

        return new ClienteDto
        {
            Cedula = cliente.Cedula,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            FechaNacimiento = cliente.FechaNacimiento,
            Genero = cliente.Genero,
            Telefono = cliente.Telefono,
            Email = cliente.Email
        };
    }

    public async Task CrearCliente(ClienteDto cliente)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=CrearCliente | Params=cedula={Cedula}", ts, cliente.Cedula);

        if (await _db.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula))
        {
            throw new FaultException(new FaultReason("Ya existe un cliente con esa cédula"), new FaultCode("Duplicate"));
        }

        _db.Clientes.Add(new Cliente
        {
            Cedula = cliente.Cedula,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            FechaNacimiento = cliente.FechaNacimiento,
            Genero = cliente.Genero,
            Telefono = cliente.Telefono,
            Email = cliente.Email
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=CrearCliente | Result=Success", ts);
    }

    public async Task ActualizarCliente(ClienteDto cliente)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarCliente | Params=cedula={Cedula}", ts, cliente.Cedula);

        var existing = await _db.Clientes.FindAsync(cliente.Cedula);
        if (existing == null)
        {
            throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));
        }

        existing.Nombre = cliente.Nombre;
        existing.Apellido = cliente.Apellido;
        existing.FechaNacimiento = cliente.FechaNacimiento;
        existing.Genero = cliente.Genero;
        existing.Telefono = cliente.Telefono;
        existing.Email = cliente.Email;
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=ActualizarCliente | Result=Success", ts);
    }

    public async Task EliminarCliente(string cedula)
    {
        var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation("[{Timestamp}] Operation=EliminarCliente | Params=cedula={Cedula}", ts, cedula);

        var cliente = await _db.Clientes.FindAsync(cedula);
        if (cliente == null)
        {
            throw new FaultException(new FaultReason("Cliente no encontrado"), new FaultCode("NotFound"));
        }

        _db.Clientes.Remove(cliente);
        await _db.SaveChangesAsync();
        _logger.LogInformation("[{Timestamp}] Operation=EliminarCliente | Result=Success", ts);
    }
}
