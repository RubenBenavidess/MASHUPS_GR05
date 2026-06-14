using ec.edu.monster.TicketClient.Modelos;
using ec.edu.monster.TicketClient.State;

namespace ec.edu.monster.TicketClient.Servicios
{
    public class TicketingService
    {
        private readonly CarritoState _carrito;

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

        public TicketingService(CarritoState carrito)
        {
            _carrito = carrito;
        }

        public void CargarAsientosPorPartido(string partidoId)
        {
            PartidoActualId = partidoId;
            LocalidadActivaCodigo = null;
            StadiumSeats.Clear();
            _carrito.VaciarCarrito();

            var partidoData = ObtenerDatosPartido(partidoId);
            PartidoLocal = partidoData.Local;
            PartidoVisitante = partidoData.Visitante;
            PartidoFecha = partidoData.Fecha;
            EstadioNombre = partidoData.Estadio;
            EstadioCodigo = partidoData.EstadioCodigo;

            var asientos = GenerarAsientos(partidoId, partidoData);
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

        private static (string Local, string Visitante, DateTime Fecha, string Estadio, string EstadioCodigo) ObtenerDatosPartido(string codigo)
        {
            return codigo switch
            {
                "PAR-001" => ("México", "Estados Unidos", new DateTime(2026, 6, 11, 20, 0, 0), "Estadio Azteca", "EST-001"),
                "PAR-002" => ("Argentina", "Brasil", new DateTime(2026, 6, 18, 20, 0, 0), "MetLife Stadium", "EST-002"),
                "PAR-003" => ("Francia", "Inglaterra", new DateTime(2026, 7, 5, 20, 0, 0), "SoFi Stadium", "EST-003"),
                _ => ("Equipo A", "Equipo B", DateTime.UtcNow, "Estadio", "EST-000"),
            };
        }

        private static List<Seat> GenerarAsientos(string partidoCodigo, (string, string, DateTime, string, string) data)
        {
            var seats = new List<Seat>();
            var (_, _, _, _, estadioCodigo) = data;

            var configs = new (string localidadCodigo, string descripcion, decimal precio, int cantidad, string estadio)[]
            {
                ("LOC-A1", "PALCO",    500m, 10, "EST-001"),
                ("LOC-A2", "TRIBUNA",  250m, 12, "EST-001"),
                ("LOC-A3", "GENERAL",  100m, 15, "EST-001"),
                ("LOC-B1", "PALCO",    500m, 10, "EST-002"),
                ("LOC-B2", "TRIBUNA",  250m, 12, "EST-002"),
                ("LOC-B3", "GENERAL",  100m, 15, "EST-002"),
                ("LOC-C1", "PALCO",    500m, 10, "EST-003"),
                ("LOC-C2", "TRIBUNA",  250m, 12, "EST-003"),
                ("LOC-C3", "GENERAL",  100m, 15, "EST-003"),
            };

            var relevantConfigs = configs.Where(c => c.estadio == estadioCodigo).ToList();
            int lIndex = 1;

            foreach (var cfg in relevantConfigs)
            {
                var lCode = $"L{lIndex:D3}";
                var filas = new[] { "A", "B", "C" };
                var filaIndex = 0;
                for (int i = 1; i <= cfg.cantidad; i++)
                {
                    if (filaIndex == 0 && i > 5) filaIndex = 1;
                    if (filaIndex == 1 && i > 10) filaIndex = 2;

                    var status = "Disponible";

                    if (partidoCodigo == "PAR-001" && cfg.localidadCodigo == "LOC-A1" && i <= 3)
                        status = "Ocupado";
                    if (partidoCodigo == "PAR-002" && cfg.localidadCodigo == "LOC-B2" && i <= 2)
                        status = "Ocupado";
                    if (partidoCodigo == "PAR-003" && cfg.localidadCodigo == "LOC-C1" && i <= 4)
                        status = "Ocupado";
                    if (new Random(i + partidoCodigo.GetHashCode()).Next(100) < 8)
                        status = "Reservado";

                    seats.Add(new Seat
                    {
                        Id = $"AS-{lCode}-{i:D2}",
                        Fila = filas[filaIndex],
                        Numero = i,
                        Zone = cfg.descripcion,
                        LocalidadCodigo = cfg.localidadCodigo,
                        LocalidadDescripcion = cfg.descripcion,
                        PartidoCodigo = partidoCodigo,
                        EstadioCodigo = cfg.estadio,
                        Price = cfg.precio,
                        Status = status,
                    });
                }
                lIndex++;
            }

            return seats;
        }
    }
}
