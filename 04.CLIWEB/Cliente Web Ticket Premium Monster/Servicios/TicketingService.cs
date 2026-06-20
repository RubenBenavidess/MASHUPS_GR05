using ec.edu.monster.TicketClient.Modelos;
using ec.edu.monster.TicketClient.State;

namespace ec.edu.monster.TicketClient.Servicios
{
    public class TicketingService
    {
        private readonly CarritoState _carrito;
        private readonly AuthState _authState;
        private readonly PartidoService.PartidoServiceClient _partidoClient;

        public List<Seat> StadiumSeats { get; private set; } = new();
        public List<Seat> Cart => _carrito.AsientosSeleccionados;
        public string PartidoActualId { get; private set; } = string.Empty;
        public string PartidoLocal { get; private set; } = string.Empty;
        public string PartidoVisitante { get; private set; } = string.Empty;
        public DateTime PartidoFecha { get; private set; }
        public string EstadioNombre { get; private set; } = string.Empty;
        public string EstadioCodigo { get; private set; } = string.Empty;

        public string? LocalidadActivaCodigo { get; private set; }
        public string LocalidadActivaNombre => LocalidadActivaCodigo != null
            ? StadiumSeats.FirstOrDefault(s => s.LocalidadCodigo == LocalidadActivaCodigo)?.LocalidadDescripcion ?? ""
            : "";

        public event Action? OnStateChanged;

        public TicketingService(CarritoState carrito, AuthState authState, PartidoService.PartidoServiceClient partidoClient)
        {
            _carrito = carrito;
            _authState = authState;
            _partidoClient = partidoClient;
        }

        public async Task CargarAsientosPorPartidoAsync(string partidoId)
        {
            PartidoActualId = partidoId;
            LocalidadActivaCodigo = null;
            StadiumSeats.Clear();
            // Ya no vaciamos el carrito aquí para permitir comprar asientos de múltiples partidos

            var partidoData = await ObtenerDatosPartidoReal(partidoId);
            PartidoLocal = partidoData.Local;
            PartidoVisitante = partidoData.Visitante;
            PartidoFecha = partidoData.Fecha;
            EstadioNombre = partidoData.Estadio;
            EstadioCodigo = partidoData.EstadioCodigo;

            var asientos = await ObtenerAsientosReales(partidoId, partidoData);

            // Marcar como seleccionados los asientos que ya están en el carrito
            foreach (var asiento in asientos)
            {
                if (_carrito.AsientosSeleccionados.Any(s => s.Id == asiento.Id))
                {
                    asiento.Status = "Seleccionado";
                }
            }

            StadiumSeats.AddRange(asientos);

            NotifyStateChanged();
        }

        public void SeleccionarLocalidad(string localidadCodigo)
        {
            LocalidadActivaCodigo = localidadCodigo;
            NotifyStateChanged();
        }

        public void VolverASecciones()
        {
            LocalidadActivaCodigo = null;
            NotifyStateChanged();
        }

        public List<Seat> GetAsientosDeLocalidad(string localidadCodigo)
        {
            return StadiumSeats
                .Where(s => s.LocalidadCodigo == localidadCodigo)
                .OrderBy(s => s.Fila)
                .ThenBy(s => s.Numero)
                .ToList();
        }

        public List<Seat> GetAsientosDeLocalidadActiva()
        {
            if (LocalidadActivaCodigo == null) return new();
            return GetAsientosDeLocalidad(LocalidadActivaCodigo);
        }

        public List<string> GetLocalidades()
        {
            return StadiumSeats
                .Select(s => s.LocalidadCodigo)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public Dictionary<string, List<Seat>> GetSeatsGroupedByLocalidad()
        {
            return StadiumSeats
                .GroupBy(s => s.LocalidadCodigo)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public (int total, int disponibles, int ocupados) GetLocalidadStats(string localidadCodigo)
        {
            var seats = GetAsientosDeLocalidad(localidadCodigo);
            return (
                seats.Count,
                seats.Count(s => s.IsAvailable),
                seats.Count(s => s.IsOccupied || s.IsReserved)
            );
        }

        public void ToggleSeat(string seatId)
        {
            var seat = StadiumSeats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null || seat.IsOccupied || seat.IsReserved) return;

            if (seat.IsAvailable)
            {
                seat.Status = "Seleccionado";
                _carrito.AgregarAsiento(seat);
            }
            else if (seat.IsSelected)
            {
                seat.Status = "Disponible";
                _carrito.RemoverAsiento(seat.Id);
            }

            NotifyStateChanged();
        }

        public decimal GetTotal() => _carrito.GetTotal();

        private void NotifyStateChanged() => OnStateChanged?.Invoke();

        private async Task<(string Local, string Visitante, DateTime Fecha, string Estadio, string EstadioCodigo)> ObtenerDatosPartidoReal(string codigo)
        {
            try
            {
                var partidos = await _partidoClient.ListarPartidosAsync(_authState.Token);
                var partido = partidos.FirstOrDefault(p => p.Codigo == codigo);
                if (partido != null)
                {
                    string estadioNombre = GetStadiumNameFallback(partido.EstadioCodigo);
                    return (partido.EquipoLocal, partido.EquipoVisitante, partido.FechaHora, estadioNombre, partido.EstadioCodigo);
                }
            }
            catch { }
            return ("Equipo Local", "Equipo Visitante", DateTime.UtcNow, "Estadio", "EST-000");
        }

        private static string GetStadiumNameFallback(string codigo) => codigo switch
        {
            "EST-001" => "MetLife Stadium",
            "EST-002" => "AT&T Stadium",
            "EST-003" => "Arrowhead Stadium",
            "EST-004" => "NRG Stadium",
            "EST-005" => "Mercedes-Benz Stadium",
            "EST-006" => "SoFi Stadium",
            "EST-007" => "Lincoln Financial Field",
            "EST-008" => "Lumen Field",
            "EST-009" => "Levi's Stadium",
            "EST-010" => "Gillette Stadium",
            "EST-011" => "Hard Rock Stadium",
            "EST-012" => "Estadio Azteca",
            "EST-013" => "Estadio BBVA",
            "EST-014" => "Estadio Akron",
            "EST-015" => "BC Place",
            "EST-016" => "BMO Field",
            _ => codigo
        };

        private async Task<List<Seat>> ObtenerAsientosReales(string partidoCodigo, (string, string, DateTime, string, string) data)
        {
            var seats = new List<Seat>();
            var (_, _, _, _, estadioCodigo) = data;

            try
            {
                var client = new AsientoService.AsientoServiceClient();
                var asientosReales = await client.ListarAsientosPorPartidoAsync(_authState.Token, partidoCodigo);

                var locClient = new LocalidadService.LocalidadServiceClient();
                var localidades = await locClient.ListarLocalidadesAsync(_authState.Token);

                foreach (var a in asientosReales)
                {
                    var cfg = localidades.FirstOrDefault(c => c.Codigo == a.LocalidadCodigo);
                    string descripcion = cfg != null ? cfg.Descripcion : "Desconocido";
                    decimal precio = cfg != null ? cfg.PrecioBase : 100m;
                    
                    var status = a.Estado == "LIBRE" ? "Disponible" : 
                                 (a.Estado == "VENDIDO" ? "Ocupado" : "Reservado");

                    seats.Add(new Seat
                    {
                        Id = a.Codigo,
                        Fila = a.Fila,
                        Numero = a.Numero,
                        Zone = descripcion,
                        LocalidadCodigo = a.LocalidadCodigo,
                        LocalidadDescripcion = descripcion,
                        PartidoCodigo = a.PartidoCodigo,
                        EstadioCodigo = estadioCodigo,
                        Price = precio,
                        Status = status,
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener asientos reales: {ex.Message}");
            }

            return seats;
        }
    }
}
