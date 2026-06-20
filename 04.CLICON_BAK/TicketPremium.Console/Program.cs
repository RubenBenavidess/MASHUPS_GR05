using TicketPremium.Console.Modelos;
using TicketPremium.Console.Servicios;
using PartidoService;
using CompraService;

namespace TicketPremium.Console
{
    class Program
    {
        private static readonly TicketConsoleService _ticketService = new();
        private static readonly CrudMenuService _crudService = new();
        private static readonly AuthClient _authClient = new();
        private static PartidoServiceClient? _partidoClient;

        private static CompraServiceClient? _compraClient;
        private static List<PartidoDto> _partidosCache = new();
        public static string? SessionToken;
        private static string? _sessionRol;
        private static string? _sessionNombre;

        static async Task Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Console.Title = "TicketPremium — World Cup 2026";

            try
            {
                _partidoClient = new PartidoServiceClient();
                _compraClient = new CompraServiceClient();
            }
            catch { }
            while (true)
            {
                var exit = await MostrarMenuPrincipal();
                if (exit) break;
            }

            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  Gracias por usar TicketPremium. Hasta pronto!", ConsoleColor.Cyan);
            System.Console.WriteLine();
        }

        static async Task<bool> MostrarMenuPrincipal()
        {
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  MENU PRINCIPAL", ConsoleColor.Yellow);
            if (SessionToken != null)
            {
                ConsoleUI.Escribir($"  Sesion: {_sessionNombre}  |  Rol: ", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea(_sessionRol ?? "", _sessionRol == "ADMIN" ? ConsoleColor.Cyan : ConsoleColor.Green);
            }
            else
            {
                ConsoleUI.EscribirLinea("  No has iniciado sesion.", ConsoleColor.DarkGray);
            }
            System.Console.WriteLine();
            ConsoleUI.EscribirLinea("  [1] Ver Partidos y Comprar Boletos", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [2] Administracion (CRUD) - Requiere Admin", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [3] Reportes - Requiere Admin", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [4] Ver Carrito de Compras", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [5] Iniciar Sesion / Registrarse", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [6] Cerrar Sesion", SessionToken != null ? ConsoleColor.White : ConsoleColor.DarkGray);
            ConsoleUI.EscribirLinea("  [7] Salir", ConsoleColor.White);
            System.Console.WriteLine();

            if (!_crudService.BackendDisponible)
                ConsoleUI.EscribirLinea("  [!] Backend no detectado. CRUD y Reportes requieren los servicios SOAP activos.", ConsoleColor.DarkYellow);

            var opcion = ConsoleUI.LeerOpcion(1, 7);

            switch (opcion)
            {
                case 1:
                    await MostrarPartidos();
                    return false;
                case 2:
                    await MenuAdministracion();
                    return false;
                case 3:
                    if (!RequerirAdmin()) return false;
                    await _crudService.MenuReportes();
                    return false;
                case 4:
                    MostrarCarrito();
                    return false;
                case 5:
                    await IniciarSesion();
                    return false;
                case 6:
                    CerrarSesion();
                    return false;
                case 7:
                    return true;
                default:
                    return false;
            }
        }

        static async Task MenuAdministracion()
        {
            if (!RequerirAdmin()) return;

            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea("  ADMINISTRACION (CRUD)", ConsoleColor.Yellow);
                System.Console.WriteLine();
                ConsoleUI.EscribirLinea("  [1] Paises", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [2] Partidos", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [3] Localidades", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [4] Clientes", ConsoleColor.White);
                ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var op = ConsoleUI.LeerOpcion(0, 4);
                if (op == 0) return;

                switch (op)
                {
                    case 1: await _crudService.CrudPaises(); break;
                    case 2: await _crudService.CrudPartidos(); break;
                    case 3: await _crudService.CrudLocalidades(); break;
                    case 4: await _crudService.CrudClientes(); break;
                }
            }
        }

        static async Task MostrarPartidos()
        {
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  PARTIDOS DISPONIBLES — MUNDIAL FIFA 2026", ConsoleColor.Yellow);
            System.Console.WriteLine();

            var partidos = await ObtenerPartidos();

            if (partidos.Count == 0)
            {
                ConsoleUI.EscribirLinea("  No hay partidos disponibles en este momento.", ConsoleColor.DarkGray);
                ConsoleUI.Pausa();
                return;
            }

            for (int i = 0; i < partidos.Count; i++)
            {
                var p = partidos[i];
                var estadio = ObtenerNombreEstadio(p.EstadioCodigo);
                var fecha = p.FechaHora.ToString("dd/MMM/yyyy HH:mm", new System.Globalization.CultureInfo("es-MX"));
                ConsoleUI.Escribir($"  [{i + 1}] ", ConsoleColor.Cyan);
                ConsoleUI.Escribir($"{p.EquipoLocal} vs {p.EquipoVisitante}", ConsoleColor.White);
                ConsoleUI.EscribirLinea($"  |  {estadio}  |  {fecha}", ConsoleColor.DarkGray);
            }

            System.Console.WriteLine();
            ConsoleUI.EscribirLinea("  [0] Volver al menu principal", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            var opcion = ConsoleUI.LeerOpcion(0, partidos.Count);

            if (opcion == 0) return;

            var partidoElegido = partidos[opcion - 1];
            await MostrarLocalidades(partidoElegido.Codigo);
        }

        static async Task MostrarLocalidades(string codigoPartido)
        {
            _ticketService.CargarAsientosPorPartido(codigoPartido);

            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea($"  SELECCION DE ASIENTOS", ConsoleColor.Yellow);
                ConsoleUI.Escribir($"  {_ticketService.PartidoLocal} vs {_ticketService.PartidoVisitante}", ConsoleColor.White);
                ConsoleUI.EscribirLinea($"  —  {_ticketService.EstadioNombre}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"  {_ticketService.PartidoFecha:dd \\de MMMM \\de yyyy · HH:mm}", ConsoleColor.DarkGray);
                System.Console.WriteLine();
                ConsoleUI.EscribirLinea("  LOCALIDADES DISPONIBLES:", ConsoleColor.Cyan);
                System.Console.WriteLine();

                var localidades = _ticketService.GetLocalidades();

                for (int i = 0; i < localidades.Count; i++)
                {
                    var loc = localidades[i];
                    var stats = _ticketService.GetLocalidadStats(loc);
                    var seats = _ticketService.GetAsientosDeLocalidad(loc);
                    var precio = seats.FirstOrDefault()?.Price ?? 0;
                    var ratio = stats.total > 0 ? (double)stats.disponibles / stats.total : 0;
                    var colorDisponibilidad = ratio > 0.5 ? ConsoleColor.Green :
                                              ratio > 0.1 ? ConsoleColor.Yellow : ConsoleColor.Red;

                    ConsoleUI.Escribir($"  [{i + 1}] {seats.FirstOrDefault()?.LocalidadDescripcion ?? loc,-10}", ConsoleColor.White);
                    System.Console.Write("  |  ");
                    ConsoleUI.Escribir($"{stats.disponibles}/{stats.total} disponibles", colorDisponibilidad);
                    ConsoleUI.EscribirLinea($"  |  ${precio} c/u", ConsoleColor.DarkGray);
                }

                System.Console.WriteLine();
                ConsoleUI.EscribirLinea("  [0] Volver a partidos", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var opcion = ConsoleUI.LeerOpcion(0, localidades.Count);

                if (opcion == 0) return;

                var localidadElegida = localidades[opcion - 1];
                _ticketService.SeleccionarLocalidad(localidadElegida);

                var result = await MostrarAsientos();

                if (result == "volver_partidos") return;
                if (result == "volver_localidades") continue;
            }
        }

        static async Task<string> MostrarAsientos()
        {
            while (true)
            {
                ConsoleUI.MostrarEncabezado();
                var localidadNombre = _ticketService.LocalidadActivaNombre;
                var seats = _ticketService.GetAsientosDeLocalidadActiva();

                ConsoleUI.EscribirLinea($"  ASIENTOS — {localidadNombre}", ConsoleColor.Yellow);
                ConsoleUI.EscribirLinea($"  {_ticketService.PartidoLocal} vs {_ticketService.PartidoVisitante}  |  {_ticketService.EstadioNombre}", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                var filas = seats.Select(s => s.Fila).Distinct().OrderBy(f => f).ToList();

                foreach (var fila in filas)
                {
                    ConsoleUI.Escribir($"  Fila {fila}:  ", ConsoleColor.DarkGray);
                    var asientosFila = seats.Where(s => s.Fila == fila).OrderBy(s => s.Numero).ToList();

                    foreach (var a in asientosFila)
                    {
                        DibujarAsiento(a);
                        System.Console.Write(" ");
                    }
                    System.Console.WriteLine();
                }

                System.Console.WriteLine();
                System.Console.Write("  ");
                DibujarAsiento(new Seat { Status = "Disponible" });
                System.Console.Write(" Disponible  ");
                DibujarAsiento(new Seat { Status = "Ocupado" });
                System.Console.Write(" Ocupado  ");
                DibujarAsiento(new Seat { Status = "Seleccionado" });
                System.Console.Write(" Seleccionado");
                System.Console.WriteLine();

                var cartCount = _ticketService.Cart.Count;
                var cartTotal = _ticketService.GetTotal();

                System.Console.WriteLine();
                if (cartCount > 0)
                {
                    ConsoleUI.EscribirLinea($"  CARRITO: {cartCount} asiento(s) — Total: ${cartTotal:F0}", ConsoleColor.Yellow);
                }
                ConsoleUI.EscribirLinea("  Ingrese numero de asiento para seleccionar/deseleccionar", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea("  [P] Pagar  |  [V] Ver carrito  |  [B] Volver a localidades", ConsoleColor.DarkGray);
                System.Console.WriteLine();

                System.Console.Write("  > ");
                var input = (System.Console.ReadLine() ?? "").Trim().ToUpper();
                System.Console.WriteLine();

                if (input == "B")
                {
                    _ticketService.VolverASecciones();
                    return "volver_localidades";
                }

                if (input == "P")
                {
                    if (_ticketService.Cart.Count == 0)
                    {
                        ConsoleUI.EscribirLinea("  No tiene asientos seleccionados. Seleccione al menos uno.", ConsoleColor.Red);
                        ConsoleUI.Pausa();
                        continue;
                    }
                    var pagoResult = await ProcesarPago();
                    if (pagoResult == "exit") return "volver_partidos";
                    continue;
                }

                if (input == "V")
                {
                    MostrarCarritoInline();
                    continue;
                }

                if (int.TryParse(input, out var numero))
                {
                    var result = _ticketService.ToggleSeatByNumero(numero);
                    if (!result)
                    {
                        ConsoleUI.EscribirLinea($"  El asiento {numero} no esta disponible o no existe.", ConsoleColor.Red);
                        ConsoleUI.Pausa();
                    }
                }
                else
                {
                    ConsoleUI.EscribirLinea("  Entrada no valida.", ConsoleColor.Red);
                    ConsoleUI.Pausa();
                }
            }
        }

        static void DibujarAsiento(Seat seat)
        {
            switch (seat.Status)
            {
                case "Disponible":
                    ConsoleUI.Escribir($"[{seat.Numero,2}]", ConsoleColor.Green);
                    break;
                case "Ocupado":
                case "Reservado":
                    ConsoleUI.Escribir($"[XX]", ConsoleColor.Red);
                    break;
                case "Seleccionado":
                    ConsoleUI.Escribir($"[{seat.Numero,2}]", ConsoleColor.Yellow);
                    break;
            }
        }

        static void MostrarCarritoInline()
        {
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  CARRITO DE COMPRAS", ConsoleColor.Yellow);
            System.Console.WriteLine();

            if (_ticketService.Cart.Count == 0)
            {
                ConsoleUI.EscribirLinea("  El carrito esta vacio.", ConsoleColor.DarkGray);
                System.Console.WriteLine();
                ConsoleUI.Pausa();
                return;
            }

            foreach (var item in _ticketService.Cart)
            {
                ConsoleUI.Escribir($"  {item.Id,-14}", ConsoleColor.White);
                System.Console.Write($"  Fila {item.Fila,-3}");
                ConsoleUI.Escribir($"  {item.LocalidadDescripcion,-10}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"  ${item.Price,6:F0}", ConsoleColor.Yellow);
            }

            System.Console.WriteLine();
            ConsoleUI.Escribir("  ", ConsoleColor.White);
            ConsoleUI.EscribirLinea(new string('─', 40), ConsoleColor.DarkGray);
            ConsoleUI.Escribir("  TOTAL: ", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"${_ticketService.GetTotal(),6:F0}", ConsoleColor.Yellow);
            System.Console.WriteLine();
            ConsoleUI.Pausa();
        }

        static void MostrarCarrito()
        {
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  CARRITO DE COMPRAS", ConsoleColor.Yellow);
            System.Console.WriteLine();

            if (_ticketService.Cart.Count == 0)
            {
                ConsoleUI.EscribirLinea("  El carrito esta vacio.", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea("  Vaya a 'Ver Partidos' para seleccionar asientos.", ConsoleColor.DarkGray);
                System.Console.WriteLine();
                ConsoleUI.Pausa();
                return;
            }

            foreach (var item in _ticketService.Cart)
            {
                ConsoleUI.Escribir($"  {item.Id,-14}", ConsoleColor.White);
                System.Console.Write($"  {item.PartidoCodigo,-10}");
                ConsoleUI.Escribir($"  Fila {item.Fila,-3}", ConsoleColor.DarkGray);
                ConsoleUI.Escribir($"  {item.LocalidadDescripcion,-10}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"  ${item.Price,6:F0}", ConsoleColor.Yellow);
            }

            System.Console.WriteLine();
            ConsoleUI.Escribir("  ", ConsoleColor.White);
            ConsoleUI.EscribirLinea(new string('─', 52), ConsoleColor.DarkGray);
            ConsoleUI.Escribir("  TOTAL: ", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"${_ticketService.GetTotal(),6:F0}", ConsoleColor.Yellow);

            System.Console.WriteLine();
            ConsoleUI.EscribirLinea("  [1] Ir a Pagar", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [2] Vaciar Carrito", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            var opcion = ConsoleUI.LeerOpcion(0, 2);

            switch (opcion)
            {
                case 1:
                    _ = ProcesarPago();
                    break;
                case 2:
                    _ticketService.Cart.Clear();
                    foreach (var s in _ticketService.StadiumSeats.Where(s => s.IsSelected))
                        s.Status = "Disponible";
                    ConsoleUI.EscribirLinea("  Carrito vaciado.", ConsoleColor.Green);
                    ConsoleUI.Pausa();
                    break;
            }
        }

        static async Task<string> ProcesarPago()
        {
            if (!RequerirLogin()) return "cancel";

            var cartTotal = _ticketService.GetTotal();

            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  PROCESAR PAGO", ConsoleColor.Yellow);
            System.Console.WriteLine();

            foreach (var item in _ticketService.Cart)
            {
                ConsoleUI.Escribir($"  {item.Id,-14}", ConsoleColor.White);
                ConsoleUI.Escribir($"  {item.LocalidadDescripcion,-10}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"  ${item.Price,6:F0}", ConsoleColor.Yellow);
            }

            System.Console.WriteLine();
            ConsoleUI.Escribir("  ", ConsoleColor.White);
            ConsoleUI.EscribirLinea(new string('─', 36), ConsoleColor.DarkGray);
            ConsoleUI.Escribir("  TOTAL A PAGAR: ", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"${cartTotal,6:F0}", ConsoleColor.Yellow);
            System.Console.WriteLine();

            var cedula = ConsoleUI.LeerCedula();

            var clienteExiste = await _crudService.ValidarClienteExiste(cedula);
            if (!clienteExiste && _crudService.BackendDisponible)
            {
                ConsoleUI.EscribirLinea("  Cliente no registrado en el sistema.", ConsoleColor.Red);
                ConsoleUI.EscribirLinea("  Registrelo en Administracion > Clientes > Crear Cliente.", ConsoleColor.DarkGray);
                ConsoleUI.Pausa();
                return "error";
            }

            System.Console.WriteLine();

            ConsoleUI.EscribirLinea("  METODO DE PAGO:", ConsoleColor.Cyan);
            ConsoleUI.EscribirLinea("  [1] Efectivo — 12% de descuento", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [2] Credito Directo — 3 a 18 meses (16.5% anual)", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [0] Cancelar pago", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            var metodo = ConsoleUI.LeerOpcion(0, 2);

            if (metodo == 0) return "cancel";

            var esEfectivo = metodo == 1;
            int plazoMeses = 6;

            if (!esEfectivo)
            {
                System.Console.Write("  Ingrese plazo en meses (3-18): ");
                var plazoInput = System.Console.ReadLine();
                System.Console.WriteLine();
                if (!int.TryParse(plazoInput, out plazoMeses) || plazoMeses < 3 || plazoMeses > 18)
                {
                    ConsoleUI.EscribirLinea("  Plazo invalido. Se usara 6 meses.", ConsoleColor.Yellow);
                    plazoMeses = 6;
                    ConsoleUI.Pausa();
                }
            }

            var descuento = esEfectivo ? cartTotal * 0.12m : 0;
            var subtotalConDesc = cartTotal - descuento;
            var iva = subtotalConDesc * 0.15m;
            var totalFinal = Math.Round(subtotalConDesc + iva, 2);

            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  CONFIRMAR PAGO", ConsoleColor.Yellow);
            System.Console.WriteLine();
            ConsoleUI.EscribirLinea($"  Cedula: {cedula}", ConsoleColor.White);
            ConsoleUI.EscribirLinea($"  Metodo: {(esEfectivo ? "EFECTIVO (12% desc.)" : $"CREDITO ({plazoMeses} meses)")}", ConsoleColor.White);
            System.Console.WriteLine();
            ConsoleUI.Escribir($"  Subtotal: ".PadRight(22), ConsoleColor.DarkGray);
            ConsoleUI.EscribirLinea($"${cartTotal,8:F0}", ConsoleColor.White);
            if (esEfectivo)
            {
                ConsoleUI.Escribir($"  Descuento (12%): ".PadRight(22), ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"-${descuento,7:F0}", ConsoleColor.Green);
            }
            ConsoleUI.Escribir($"  IVA (15%): ".PadRight(22), ConsoleColor.DarkGray);
            ConsoleUI.EscribirLinea($"+${iva,7:F0}", ConsoleColor.DarkGray);
            ConsoleUI.Escribir("  ", ConsoleColor.White);
            ConsoleUI.Escribir(new string('─', 28), ConsoleColor.DarkGray);
            System.Console.WriteLine();
            ConsoleUI.Escribir($"  TOTAL: ".PadRight(22), ConsoleColor.White);
            ConsoleUI.EscribirLinea($"${totalFinal,8:F0}", ConsoleColor.Yellow);
            System.Console.WriteLine();

            ConsoleUI.EscribirLinea("  [1] Confirmar Pago", ConsoleColor.Green);
            ConsoleUI.EscribirLinea("  [0] Cancelar", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            var confirmar = ConsoleUI.LeerOpcion(0, 1);

            if (confirmar == 0) return "cancel";

            var codigos = _ticketService.GetCodigosAsientos();

            if (_compraClient == null)
            {
                ConsoleUI.EscribirLinea("  Servidor backend no disponible. No se puede procesar el pago.", ConsoleColor.Red);
                ConsoleUI.EscribirLinea("  Inicie los servicios en 04.SERVIDOR e intente de nuevo.", ConsoleColor.DarkGray);
                ConsoleUI.Pausa();
                return "error";
            }

            try
            {
                CompraResponse response;

                if (esEfectivo)
                    response = await _compraClient.ComprarEnEfectivoAsync(SessionToken, codigos, cedula);
                else
                    response = await _compraClient.ComprarACreditoAsync(SessionToken, codigos, cedula, plazoMeses);

                if (response.Exitoso)
                {
                    ConsoleUI.MostrarEncabezado();
                    ConsoleUI.EscribirLinea("  COMPRA EXITOSA!", ConsoleColor.Green);
                    System.Console.WriteLine();
                    ConsoleUI.EscribirLinea($"  {response.Mensaje}", ConsoleColor.White);
                    ConsoleUI.EscribirLinea($"  Factura: {response.NumeroFactura}", ConsoleColor.Cyan);
                    System.Console.WriteLine();

                    _ticketService.Cart.Clear();
                    foreach (var s in _ticketService.StadiumSeats.Where(s => s.IsSelected))
                        s.Status = "Disponible";

                    ConsoleUI.Pausa();
                    return "success";
                }
                else
                {
                    ConsoleUI.MostrarEncabezado();
                    ConsoleUI.EscribirLinea("  ERROR EN LA COMPRA", ConsoleColor.Red);
                    System.Console.WriteLine();
                    ConsoleUI.EscribirLinea($"  {response.Mensaje}", ConsoleColor.White);
                    System.Console.WriteLine();
                    ConsoleUI.Pausa();
                    return "error";
                }
            }
            catch (System.ServiceModel.FaultException fex)
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea("  ERROR DEL SERVIDOR", ConsoleColor.Red);
                System.Console.WriteLine();
                ConsoleUI.EscribirLinea($"  Codigo: {fex.Code.Name}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea($"  {fex.Reason}", ConsoleColor.White);
                System.Console.WriteLine();
                ConsoleUI.Pausa();
                return "error";
            }
            catch (Exception ex)
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea("  ERROR DE CONEXION", ConsoleColor.Red);
                System.Console.WriteLine();
                ConsoleUI.EscribirLinea($"  {ex.Message}", ConsoleColor.DarkGray);
                ConsoleUI.EscribirLinea("  Verifique que los backends esten corriendo en los puertos correctos.", ConsoleColor.DarkGray);
                System.Console.WriteLine();
                ConsoleUI.Pausa();
                return "error";
            }
        }

        static async Task<List<PartidoDto>> ObtenerPartidos()
        {
            if (_partidosCache.Count > 0) return _partidosCache;

            try
            {
                if (_partidoClient != null)
                {
                    var partidos = await _partidoClient.ListarPartidosAsync(SessionToken);
                    _partidosCache = partidos.ToList();
                    return _partidosCache;
                }
            }
            catch
            {
            }

            _partidosCache = new List<PartidoDto>
            {
                new() { Codigo = "PAR-001", EquipoLocal = "Mexico", EquipoVisitante = "Estados Unidos", FechaHora = new DateTime(2026, 6, 11, 20, 0, 0), EstadioCodigo = "EST-001" },
                new() { Codigo = "PAR-002", EquipoLocal = "Argentina", EquipoVisitante = "Brasil", FechaHora = new DateTime(2026, 6, 18, 20, 0, 0), EstadioCodigo = "EST-002" },
                new() { Codigo = "PAR-003", EquipoLocal = "Francia", EquipoVisitante = "Inglaterra", FechaHora = new DateTime(2026, 7, 5, 20, 0, 0), EstadioCodigo = "EST-003" },
            };
            return _partidosCache;
        }

        static string ObtenerNombreEstadio(string codigo) => codigo switch
        {
            "EST-001" => "Estadio Azteca",
            "EST-002" => "MetLife Stadium",
            "EST-003" => "SoFi Stadium",
            _ => codigo,
        };

        static bool RequerirAdmin()
        {
            if (SessionToken != null && _sessionRol == "ADMIN") return true;
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  ACCESO RESTRINGIDO", ConsoleColor.Red);
            ConsoleUI.EscribirLinea("  Esta seccion requiere iniciar sesion como ADMINISTRADOR.", ConsoleColor.DarkGray);
            ConsoleUI.EscribirLinea("  Use la opcion [5] del menu principal para iniciar sesion.", ConsoleColor.DarkGray);
            ConsoleUI.Pausa();
            return false;
        }

        static bool RequerirLogin()
        {
            if (SessionToken != null) return true;
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  ACCESO RESTRINGIDO", ConsoleColor.Red);
            ConsoleUI.EscribirLinea("  Para realizar una compra debe iniciar sesion.", ConsoleColor.DarkGray);
            ConsoleUI.EscribirLinea("  Use la opcion [5] del menu principal para iniciar sesion o registrarse.", ConsoleColor.DarkGray);
            ConsoleUI.Pausa();
            return false;
        }

        static async Task IniciarSesion()
        {
            ConsoleUI.MostrarEncabezado();
            ConsoleUI.EscribirLinea("  INICIAR SESION / REGISTRO", ConsoleColor.Yellow);
            System.Console.WriteLine();
            ConsoleUI.EscribirLinea("  [1] Iniciar Sesion", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [2] Registrarse como Cliente", ConsoleColor.White);
            ConsoleUI.EscribirLinea("  [0] Volver", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            var op = ConsoleUI.LeerOpcion(0, 2);
            if (op == 0) return;

            if (op == 1)
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea("  INICIAR SESION", ConsoleColor.Yellow);
                var cedula = ConsoleUI.LeerTexto("  Cedula: ");
                System.Console.Write("  Contrasena: ");
                var password = ReadPassword();
                System.Console.WriteLine();

                try
                {
                    var (exitoso, token, nombre, rol, mensaje) = await _authClient.LoginAsync(cedula, password);
                    if (exitoso)
                    {
                        SessionToken = token;
                        _sessionRol = rol;
                        _sessionNombre = nombre;
                        ConsoleUI.EscribirLinea($"  Bienvenido/a, {nombre}!", ConsoleColor.Green);
                        ConsoleUI.EscribirLinea($"  Rol: {rol}", ConsoleColor.Cyan);
                    }
                    else
                    {
                        ConsoleUI.EscribirLinea($"  Error: {mensaje}", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  Error de conexion: {ex.Message}", ConsoleColor.Red);
                    ConsoleUI.EscribirLinea("  Verifique que los backends esten corriendo.", ConsoleColor.DarkGray);
                }
                ConsoleUI.Pausa();
            }
            else
            {
                ConsoleUI.MostrarEncabezado();
                ConsoleUI.EscribirLinea("  REGISTRO DE CLIENTE", ConsoleColor.Yellow);
                var cedula = ConsoleUI.LeerTexto("  Cedula: ");
                var nombre = ConsoleUI.LeerTexto("  Nombre: ");
                var apellido = ConsoleUI.LeerTexto("  Apellido: ");
                var email = ConsoleUI.LeerTexto("  Email: ");
                var telefono = ConsoleUI.LeerTexto("  Telefono: ");
                var genero = ConsoleUI.LeerTexto("  Genero (MASCULINO/FEMENINO): ");
                var fechaStr = ConsoleUI.LeerTexto("  Fecha Nacimiento (yyyy-MM-dd): ");
                System.Console.Write("  Contrasena: ");
                var password = ReadPassword();
                System.Console.WriteLine();

                try
                {
                    var fecha = DateTime.Parse(fechaStr);
                    var (exitoso, token, nom, rol, mensaje) = await _authClient.RegistroAsync(cedula, nombre, apellido, email, telefono, genero, fecha, password);
                    if (exitoso)
                    {
                        SessionToken = token;
                        _sessionRol = rol;
                        _sessionNombre = nom;
                        ConsoleUI.EscribirLinea($"  Registro exitoso. Bienvenido/a, {nom}!", ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleUI.EscribirLinea($"  Error: {mensaje}", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.EscribirLinea($"  Error de conexion: {ex.Message}", ConsoleColor.Red);
                }
                ConsoleUI.Pausa();
            }
        }

        static void CerrarSesion()
        {
            if (SessionToken == null)
            {
                ConsoleUI.EscribirLinea("  No hay sesion activa.", ConsoleColor.DarkGray);
                ConsoleUI.Pausa();
                return;
            }
            SessionToken = null;
            _sessionRol = null;
            _sessionNombre = null;
            ConsoleUI.EscribirLinea("  Sesion cerrada.", ConsoleColor.Green);
            ConsoleUI.Pausa();
        }

        static string ReadPassword()
        {
            var password = new System.Text.StringBuilder();
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == System.ConsoleKey.Enter) break;
                if (key.Key == System.ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Length--;
                    System.Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    System.Console.Write("*");
                }
            }
            return password.ToString();
        }
    }
}
