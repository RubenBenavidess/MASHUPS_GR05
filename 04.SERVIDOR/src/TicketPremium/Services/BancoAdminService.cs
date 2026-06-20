using CoreWCF;
using ec.edu.monster.TicketPremium.Contracts;
using ec.edu.monster.TicketPremium.Clients;

namespace ec.edu.monster.TicketPremium.Services;

public class BancoAdminService : IBancoAdminService
{
    private readonly BancoSoapClient _bancoClient;
    private readonly ILogger<BancoAdminService> _logger;

    public BancoAdminService(BancoSoapClient bancoClient, ILogger<BancoAdminService> logger)
    {
        _bancoClient = bancoClient;
        _logger = logger;
    }

    public async Task<CrearClienteBancoResponse> CrearClienteBanco(string sessionToken, CrearClienteBancoRequest request)
    {
        try
        {
            if (!AuthService.EsAdmin(sessionToken))
            {
                return new CrearClienteBancoResponse { Exitoso = false, Mensaje = "No autorizado. Solo los administradores pueden realizar esta acción." };
            }

            if (string.IsNullOrWhiteSpace(request.Cedula) || string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Apellido))
            {
                return new CrearClienteBancoResponse { Exitoso = false, Mensaje = "Cédula, nombre y apellido son obligatorios." };
            }

            if (request.DepositoInicial < 0)
            {
                return new CrearClienteBancoResponse { Exitoso = false, Mensaje = "El depósito inicial no puede ser negativo." };
            }

            var (exitoso, mensaje) = await _bancoClient.CrearClienteConCuentaAsync(
                request.Cedula,
                request.Nombre,
                request.Apellido,
                request.FechaNacimiento,
                request.Genero,
                request.DepositoInicial
            );

            return new CrearClienteBancoResponse
            {
                Exitoso = exitoso,
                Mensaje = mensaje
            };
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CrearClienteBanco");
                return new CrearClienteBancoResponse { Exitoso = false, Mensaje = "Error interno del servidor." };
            }
        }

        public async Task<ListarClientesBancoResponse> ListarClientesBanco(string sessionToken)
        {
            try
            {
                if (!AuthService.EsAdmin(sessionToken))
                {
                    return new ListarClientesBancoResponse { Exitoso = false, Mensaje = "No autorizado. Solo los administradores pueden realizar esta acción." };
                }

                var (exitoso, mensaje, clientes) = await _bancoClient.ListarClientesAsync();

                return new ListarClientesBancoResponse
                {
                    Exitoso = exitoso,
                    Mensaje = mensaje,
                    Clientes = clientes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ListarClientesBanco");
                return new ListarClientesBancoResponse { Exitoso = false, Mensaje = "Error interno del servidor." };
            }
        }
        public async Task<ActualizarClienteBancoResponse> ActualizarClienteBanco(string sessionToken, ClienteBancoDto cliente)
        {
            try
            {
                if (!AuthService.EsAdmin(sessionToken))
                {
                    return new ActualizarClienteBancoResponse { Exitoso = false, Mensaje = "No autorizado. Solo los administradores pueden realizar esta acción." };
                }

                var (exitoso, mensaje) = await _bancoClient.ActualizarClienteAsync(cliente);

                return new ActualizarClienteBancoResponse
                {
                    Exitoso = exitoso,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ActualizarClienteBanco");
                return new ActualizarClienteBancoResponse { Exitoso = false, Mensaje = "Error interno del servidor." };
            }
        }

        public async Task<EliminarClienteBancoResponse> EliminarClienteBanco(string sessionToken, string cedula)
        {
            try
            {
                if (!AuthService.EsAdmin(sessionToken))
                {
                    return new EliminarClienteBancoResponse { Exitoso = false, Mensaje = "No autorizado. Solo los administradores pueden realizar esta acción." };
                }

                var (exitoso, mensaje) = await _bancoClient.EliminarClienteAsync(cedula);

                return new EliminarClienteBancoResponse
                {
                    Exitoso = exitoso,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en EliminarClienteBanco");
                return new EliminarClienteBancoResponse { Exitoso = false, Mensaje = "Error interno del servidor." };
            }
        }

        public async Task<ClienteDetalleResponse> ObtenerClienteDetalle(string sessionToken, string cedula)
        {
            try
            {
                if (!AuthService.EsAdmin(sessionToken))
                {
                    return new ClienteDetalleResponse { Exitoso = false, Mensaje = "No autorizado. Solo los administradores pueden realizar esta acción." };
                }

                var (exitoso, mensaje, saldo, creditos) = await _bancoClient.ObtenerClienteDetalleAsync(cedula);

                return new ClienteDetalleResponse
                {
                    Exitoso = exitoso,
                    Mensaje = mensaje,
                    SaldoAhorros = saldo,
                    Creditos = creditos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerClienteDetalle");
                return new ClienteDetalleResponse { Exitoso = false, Mensaje = "Error interno del servidor." };
            }
        }
    }
