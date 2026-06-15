using PaisService;
using EstadioService;
using LocalidadService;
using PartidoService;
using ClienteService;
using FacturaService;
using ReporteService;

namespace TicketPremium.Console.Servicios
{
    public class CrudMenuService
    {
        private readonly PaisServiceClient? _paisClient;
        private readonly LocalidadServiceClient? _localidadClient;
        private readonly PartidoServiceClient? _partidoClient;
        private readonly ClienteServiceClient? _clienteClient;
        private readonly FacturaServiceClient? _facturaClient;
        private readonly ReporteServiceClient? _reporteClient;

        public bool BackendDisponible { get; }

        public CrudMenuService()
        {
            try
            {
                _paisClient = new PaisServiceClient();
                _localidadClient = new LocalidadServiceClient();
                _partidoClient = new PartidoServiceClient();
                _clienteClient = new ClienteServiceClient();
                _facturaClient = new FacturaServiceClient();
                _reporteClient = new ReporteServiceClient();
                BackendDisponible = true;
            }
            catch
            {
                BackendDisponible = false;
            }
        }

        public static void MostrarTitulo(string titulo)
        {
            ConsoleUI.EscribirLinea($"  {titulo}", ConsoleColor.Yellow);
            System.Console.WriteLine();
        }

        // ========================
        // PAISES
        // ========================
        public async Task CrudPaises()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                MostrarTitulo("ADMINISTRACION DE PAISES");
                ConsoleUI.EscribirLinea("  [1] Listar Paises", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Buscar Pais", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Crear Pais", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [4] Actualizar Pais", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [5] Eliminar Pais", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 5);
                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: await ListarPaises(); break;
                        case 2: await BuscarPais(); break;
                        case 3: await CrearPais(); break;
                        case 4: await ActualizarPais(); break;
                        case 5: await EliminarPais(); break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  ERROR: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        async Task ListarPaises()
        {
            if (_paisClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("LISTA DE PAISES");
            var paises = await _paisClient.ListarPaisesAsync();
            foreach (var p in paises)
                ConsoleUI.EscribirLinea($"  {p.Codigo,-6} {p.Nombre,-30} {p.Continente}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task BuscarPais()
        {
            if (_paisClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("BUSCAR PAIS");
            var codigo = ConsoleUI.LeerTexto("  Codigo del pais: ");
            var pais = await _paisClient.ObtenerPaisAsync(codigo);
            ConsoleUI.EscribirLinea($"  Codigo: {pais.Codigo}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Nombre: {pais.Nombre}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Continente: {pais.Continente}", ConsoleColor.White);
            ConsoleUI.Pausa();
        }

        async Task CrearPais()
        {
            if (_paisClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("CREAR PAIS");
            var codigo = ConsoleUI.LeerTexto("  Codigo (ej: ECU): ");
            var nombre = ConsoleUI.LeerTexto("  Nombre: ");
            var continente = ConsoleUI.LeerTexto("  Continente: ");
            await _paisClient.CrearPaisAsync(new PaisService.PaisDto { Codigo = codigo, Nombre = nombre, Continente = continente });
            ConsoleUI.EscribirLinea("  Pais creado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task ActualizarPais()
        {
            if (_paisClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ACTUALIZAR PAIS");
            var codigo = ConsoleUI.LeerTexto("  Codigo del pais a editar: ");
            var nombre = ConsoleUI.LeerTexto("  Nuevo nombre: ");
            var continente = ConsoleUI.LeerTexto("  Nuevo continente: ");
            await _paisClient.ActualizarPaisAsync(new PaisService.PaisDto { Codigo = codigo, Nombre = nombre, Continente = continente });
            ConsoleUI.EscribirLinea("  Pais actualizado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task EliminarPais()
        {
            if (_paisClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ELIMINAR PAIS");
            var codigo = ConsoleUI.LeerTexto("  Codigo del pais a eliminar: ");
            await _paisClient.EliminarPaisAsync(codigo);
            ConsoleUI.EscribirLinea("  Pais eliminado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        // ========================
        // PARTIDOS
        // ========================
        public async Task CrudPartidos()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                MostrarTitulo("ADMINISTRACION DE PARTIDOS");
                ConsoleUI.EscribirLinea("  [1] Listar Partidos", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Buscar Partido", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Crear Partido", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [4] Actualizar Partido", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [5] Eliminar Partido", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 5);
                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: await ListarPartidosCrud(); break;
                        case 2: await BuscarPartido(); break;
                        case 3: await CrearPartido(); break;
                        case 4: await ActualizarPartido(); break;
                        case 5: await EliminarPartido(); break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  ERROR: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        async Task ListarPartidosCrud()
        {
            if (_partidoClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("LISTA DE PARTIDOS");
            var partidos = await _partidoClient.ListarPartidosAsync();
            foreach (var p in partidos)
                ConsoleUI.EscribirLinea($"  {p.Codigo,-10} {p.EquipoLocal,-20} vs {p.EquipoVisitante,-20} {p.FechaHora:dd/MMM/yyyy HH:mm}  {p.EstadioCodigo}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task BuscarPartido()
        {
            if (_partidoClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("BUSCAR PARTIDO");
            var codigo = ConsoleUI.LeerTexto("  Codigo del partido: ");
            var p = await _partidoClient.ObtenerPartidoAsync(codigo);
            ConsoleUI.EscribirLinea($"  Codigo: {p.Codigo}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Equipos: {p.EquipoLocal} vs {p.EquipoVisitante}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Fecha: {p.FechaHora:dd/MMM/yyyy HH:mm}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Estadio: {p.EstadioCodigo}", ConsoleColor.White);
            ConsoleUI.Pausa();
        }

        async Task CrearPartido()
        {
            if (_partidoClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("CREAR PARTIDO");
            var codigo = ConsoleUI.LeerTexto("  Codigo (ej: PAR-010): ");
            var local = ConsoleUI.LeerTexto("  Equipo Local: ");
            var visitante = ConsoleUI.LeerTexto("  Equipo Visitante: ");
            var fecha = ConsoleUI.LeerTexto("  Fecha (yyyy-MM-dd HH:mm): ");
            var estadio = ConsoleUI.LeerTexto("  Codigo Estadio: ");
            await _partidoClient.CrearPartidoAsync(new PartidoService.PartidoDto
            {
                Codigo = codigo, EquipoLocal = local, EquipoVisitante = visitante,
                FechaHora = DateTime.Parse(fecha), EstadioCodigo = estadio
            });
            ConsoleUI.EscribirLinea("  Partido creado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task ActualizarPartido()
        {
            if (_partidoClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ACTUALIZAR PARTIDO");
            var codigo = ConsoleUI.LeerTexto("  Codigo del partido a editar: ");
            var local = ConsoleUI.LeerTexto("  Nuevo Equipo Local: ");
            var visitante = ConsoleUI.LeerTexto("  Nuevo Equipo Visitante: ");
            var fecha = ConsoleUI.LeerTexto("  Nueva Fecha (yyyy-MM-dd HH:mm): ");
            var estadio = ConsoleUI.LeerTexto("  Nuevo Codigo Estadio: ");
            await _partidoClient.ActualizarPartidoAsync(new PartidoService.PartidoDto
            {
                Codigo = codigo, EquipoLocal = local, EquipoVisitante = visitante,
                FechaHora = DateTime.Parse(fecha), EstadioCodigo = estadio
            });
            ConsoleUI.EscribirLinea("  Partido actualizado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task EliminarPartido()
        {
            if (_partidoClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ELIMINAR PARTIDO");
            var codigo = ConsoleUI.LeerTexto("  Codigo del partido a eliminar: ");
            await _partidoClient.EliminarPartidoAsync(codigo);
            ConsoleUI.EscribirLinea("  Partido eliminado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        // ========================
        // LOCALIDADES
        // ========================
        public async Task CrudLocalidades()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                MostrarTitulo("ADMINISTRACION DE LOCALIDADES");
                ConsoleUI.EscribirLinea("  [1] Listar Localidades", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Buscar Localidad", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Crear Localidad", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [4] Actualizar Localidad", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [5] Eliminar Localidad", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 5);
                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: await ListarLocalidades(); break;
                        case 2: await BuscarLocalidad(); break;
                        case 3: await CrearLocalidad(); break;
                        case 4: await ActualizarLocalidad(); break;
                        case 5: await EliminarLocalidad(); break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  ERROR: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        async Task ListarLocalidades()
        {
            if (_localidadClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("LISTA DE LOCALIDADES");
            var locs = await _localidadClient.ListarLocalidadesAsync();
            foreach (var l in locs)
                ConsoleUI.EscribirLinea($"  {l.Codigo,-10} {l.Descripcion,-15} Cap: {l.Capacidad,-5} ${l.PrecioBase,-8} Estadio: {l.EstadioCodigo}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task BuscarLocalidad()
        {
            if (_localidadClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("BUSCAR LOCALIDAD");
            var codigo = ConsoleUI.LeerTexto("  Codigo de la localidad: ");
            var l = await _localidadClient.ObtenerLocalidadAsync(codigo);
            ConsoleUI.EscribirLinea($"  Codigo: {l.Codigo}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Descripcion: {l.Descripcion}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Capacidad: {l.Capacidad}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Precio Base: ${l.PrecioBase}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Estadio: {l.EstadioCodigo}", ConsoleColor.White);
            ConsoleUI.Pausa();
        }

        async Task CrearLocalidad()
        {
            if (_localidadClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("CREAR LOCALIDAD");
            var codigo = ConsoleUI.LeerTexto("  Codigo: ");
            var desc = ConsoleUI.LeerTexto("  Descripcion: ");
            var cap = int.Parse(ConsoleUI.LeerTexto("  Capacidad: "));
            var precio = decimal.Parse(ConsoleUI.LeerTexto("  Precio Base: "));
            var estadio = ConsoleUI.LeerTexto("  Codigo Estadio: ");
            await _localidadClient.CrearLocalidadAsync(new LocalidadService.LocalidadDto
            {
                Codigo = codigo, Descripcion = desc, Capacidad = cap, PrecioBase = precio, EstadioCodigo = estadio
            });
            ConsoleUI.EscribirLinea("  Localidad creada exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task ActualizarLocalidad()
        {
            if (_localidadClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ACTUALIZAR LOCALIDAD");
            var codigo = ConsoleUI.LeerTexto("  Codigo de la localidad a editar: ");
            var desc = ConsoleUI.LeerTexto("  Nueva Descripcion: ");
            var cap = int.Parse(ConsoleUI.LeerTexto("  Nueva Capacidad: "));
            var precio = decimal.Parse(ConsoleUI.LeerTexto("  Nuevo Precio Base: "));
            var estadio = ConsoleUI.LeerTexto("  Nuevo Codigo Estadio: ");
            await _localidadClient.ActualizarLocalidadAsync(new LocalidadService.LocalidadDto
            {
                Codigo = codigo, Descripcion = desc, Capacidad = cap, PrecioBase = precio, EstadioCodigo = estadio
            });
            ConsoleUI.EscribirLinea("  Localidad actualizada exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task EliminarLocalidad()
        {
            if (_localidadClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ELIMINAR LOCALIDAD");
            var codigo = ConsoleUI.LeerTexto("  Codigo de la localidad a eliminar: ");
            await _localidadClient.EliminarLocalidadAsync(codigo);
            ConsoleUI.EscribirLinea("  Localidad eliminada exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        // ========================
        // CLIENTES
        // ========================
        public async Task CrudClientes()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                MostrarTitulo("ADMINISTRACION DE CLIENTES");
                ConsoleUI.EscribirLinea("  [1] Listar Clientes", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Buscar Cliente", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Crear Cliente", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [4] Actualizar Cliente", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [5] Eliminar Cliente", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 5);
                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: await ListarClientes(); break;
                        case 2: await BuscarCliente(); break;
                        case 3: await CrearCliente(); break;
                        case 4: await ActualizarCliente(); break;
                        case 5: await EliminarCliente(); break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  ERROR: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        async Task ListarClientes()
        {
            if (_clienteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("LISTA DE CLIENTES");
            var clientes = await _clienteClient.ListarClientesAsync();
            foreach (var c in clientes)
                ConsoleUI.EscribirLinea($"  {c.Cedula,-15} {c.Nombre,-15} {c.Apellido,-15} {c.Email,-25} {c.Genero}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task BuscarCliente()
        {
            if (_clienteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("BUSCAR CLIENTE");
            var cedula = ConsoleUI.LeerTexto("  Cedula del cliente: ");
            var c = await _clienteClient.ObtenerClienteAsync(cedula);
            ConsoleUI.EscribirLinea($"  Cedula: {c.Cedula}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Nombre: {c.Nombre} {c.Apellido}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Email: {c.Email}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Telefono: {c.Telefono}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Genero: {c.Genero}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Fecha Nac: {c.FechaNacimiento:dd/MMM/yyyy}", ConsoleColor.White);
            ConsoleUI.Pausa();
        }

        async Task CrearCliente()
        {
            if (_clienteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("CREAR CLIENTE");
            var cedula = ConsoleUI.LeerTexto("  Cedula: ");
            var nombre = ConsoleUI.LeerTexto("  Nombre: ");
            var apellido = ConsoleUI.LeerTexto("  Apellido: ");
            var email = ConsoleUI.LeerTexto("  Email: ");
            var telefono = ConsoleUI.LeerTexto("  Telefono: ");
            var genero = ConsoleUI.LeerTexto("  Genero (MASCULINO/FEMENINO): ");
            var fecha = ConsoleUI.LeerTexto("  Fecha Nacimiento (yyyy-MM-dd): ");
            await _clienteClient.CrearClienteAsync(new ClienteService.ClienteDto
            {
                Cedula = cedula, Nombre = nombre, Apellido = apellido, Email = email,
                Telefono = telefono, Genero = genero, FechaNacimiento = DateTime.Parse(fecha)
            });
            ConsoleUI.EscribirLinea("  Cliente creado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task ActualizarCliente()
        {
            if (_clienteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ACTUALIZAR CLIENTE");
            var cedula = ConsoleUI.LeerTexto("  Cedula del cliente a editar: ");
            var nombre = ConsoleUI.LeerTexto("  Nuevo Nombre: ");
            var apellido = ConsoleUI.LeerTexto("  Nuevo Apellido: ");
            var email = ConsoleUI.LeerTexto("  Nuevo Email: ");
            var telefono = ConsoleUI.LeerTexto("  Nuevo Telefono: ");
            var genero = ConsoleUI.LeerTexto("  Nuevo Genero: ");
            var fecha = ConsoleUI.LeerTexto("  Nueva Fecha Nacimiento (yyyy-MM-dd): ");
            await _clienteClient.ActualizarClienteAsync(new ClienteService.ClienteDto
            {
                Cedula = cedula, Nombre = nombre, Apellido = apellido, Email = email,
                Telefono = telefono, Genero = genero, FechaNacimiento = DateTime.Parse(fecha)
            });
            ConsoleUI.EscribirLinea("  Cliente actualizado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        async Task EliminarCliente()
        {
            if (_clienteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("ELIMINAR CLIENTE");
            var cedula = ConsoleUI.LeerTexto("  Cedula del cliente a eliminar: ");
            await _clienteClient.EliminarClienteAsync(cedula);
            ConsoleUI.EscribirLinea("  Cliente eliminado exitosamente.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        // ========================
        // REPORTES
        // ========================
        public async Task MenuReportes()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                MostrarTitulo("REPORTES");
                ConsoleUI.EscribirLinea("  [1] Ventas por Partido", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Ventas por Cliente", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Calcular Factura", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 3);
                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: await VentasPorPartido(); break;
                        case 2: await VentasPorCliente(); break;
                        case 3: await CalcularFactura(); break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  ERROR: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        async Task VentasPorPartido()
        {
            if (_reporteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("VENTAS POR PARTIDO");
            var codigo = ConsoleUI.LeerTexto("  Codigo del partido: ");
            var resumen = await _reporteClient.ResumenVentasPorPartidoAsync(codigo);
            foreach (var r in resumen)
                ConsoleUI.EscribirLinea($"  {r.DescripcionLocalidad,-15} Vendidos: {r.BoletosVendidos,-5} Recaudado: ${r.TotalRecaudado,8:F0}", ConsoleColor.White);
            if (resumen.Length == 0)
                ConsoleUI.EscribirLinea("  Sin ventas registradas para este partido.", ConsoleColor.DarkGray);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task VentasPorCliente()
        {
            if (_reporteClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("VENTAS POR CLIENTE");
            var cedula = ConsoleUI.LeerTexto("  Cedula del cliente: ");
            var resumen = await _reporteClient.ResumenVentasPorClienteAsync(cedula);
            foreach (var r in resumen)
                ConsoleUI.EscribirLinea($"  {r.NumeroFactura,-20} {r.Fecha:dd/MMM/yyyy}  {r.Partido,-25} {r.Asiento,-14} {r.Localidad,-10} ${r.PrecioUnitario,6:F0}  {r.MetodoPago}", ConsoleColor.White);
            if (resumen.Length == 0)
                ConsoleUI.EscribirLinea("  Sin compras registradas para este cliente.", ConsoleColor.DarkGray);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        async Task CalcularFactura()
        {
            if (_facturaClient == null) { SinBackend(); return; }
            ConsoleUI.MostrarEncabezado();
            MostrarTitulo("CALCULAR FACTURA");
            var codigosStr = ConsoleUI.LeerTexto("  Codigos de asiento (separados por coma): ");
            var codigos = codigosStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var efectivoStr = ConsoleUI.LeerTexto("  Efectivo? (s/n): ");
            var esEfectivo = efectivoStr.StartsWith("s", StringComparison.OrdinalIgnoreCase);
            var cedula = ConsoleUI.LeerTexto("  Cedula del cliente: ");
            var factura = await _facturaClient.CalcularFacturaAsync(codigos, esEfectivo, cedula);
            ConsoleUI.EscribirLinea($"  Factura: {factura.Numero}", ConsoleColor.Cyan);
            ConsoleUI.EscribirLinea($"  Fecha: {factura.Fecha:dd/MMM/yyyy HH:mm}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Subtotal: ${factura.Subtotal:F0}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Descuento: ${factura.Descuento:F0}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  IVA: ${factura.Iva:F0}", ConsoleColor.White);
            ConsoleUI.Escribir($"  TOTAL: ${factura.Total:F0}", ConsoleColor.Yellow);
            System.Console.WriteLine($"  ({factura.MetodoPago})");
            System.Console.WriteLine();
            ConsoleUI.EscribirLinea("  Items:", ConsoleColor.DarkGray);
            foreach (var item in factura.Items)
                ConsoleUI.EscribirLinea($"    {item.CodigoAsiento,-14} {item.Localidad,-12} {item.Partido,-30} ${item.PrecioUnitario,6:F0}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        // ========================
        // VALIDACION DE CLIENTE
        // ========================
        public async Task<bool> ValidarClienteExiste(string cedula)
        {
            if (_clienteClient == null) return true; // sin backend, no se puede validar
            try
            {
                await _clienteClient.ObtenerClienteAsync(cedula);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void SinBackend()
        {
            ConsoleUI.EscribirLinea("  Servidor backend no disponible.", ConsoleColor.Red);
            ConsoleUI.EscribirLinea("  Inicie los servicios en 04.SERVIDOR e intente de nuevo.", ConsoleColor.DarkGray);
            ConsoleUI.Pausa();
        }
    }
}
