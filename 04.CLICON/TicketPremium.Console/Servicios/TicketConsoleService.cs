using TicketPremium.Console.Modelos;
using AsientoService;
using LocalidadService;

namespace TicketPremium.Console.Servicios
{
    public class TicketConsoleService
    {
        public List<Seat> StadiumSeats { get; private set; } = new();
        public List<Seat> Cart { get; private set; } = new();
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

        public async Task CargarAsientosPorPartidoAsync(string partidoId, string equipoLocal, string equipoVisitante, DateTime fecha, string estadioNombre, string estadioCodigo)
        {
            PartidoActualId = partidoId;
            LocalidadActivaCodigo = null;
            StadiumSeats.Clear();

            PartidoLocal = equipoLocal;
            PartidoVisitante = equipoVisitante;
            PartidoFecha = fecha;
            EstadioNombre = estadioNombre;
            EstadioCodigo = estadioCodigo;

            try 
            {
                var asientoClient = WcfHelper.CreateAsientoServiceClient();
                asientoClient.Endpoint.EndpointBehaviors.Add(new SessionTokenInspector(() => Program.SessionToken));
                
                var localidadClient = WcfHelper.CreateLocalidadServiceClient();
                localidadClient.Endpoint.EndpointBehaviors.Add(new SessionTokenInspector(() => Program.SessionToken));

                var realSeats = await asientoClient.ListarAsientosPorPartidoAsync(Program.SessionToken, partidoId);
                var localidades = await localidadClient.ListarLocalidadesAsync();

                foreach(var a in realSeats) 
                {
                    var loc = localidades.FirstOrDefault(l => l.Codigo == a.LocalidadCodigo);
                    var status = a.Estado;
                    if (status?.ToUpper() == "DISPONIBLE" || status?.ToUpper() == "LIBRE") status = "Disponible";
                    else if (status?.ToUpper() == "OCUPADO" || status?.ToUpper() == "VENDIDO") status = "Ocupado";
                    else if (status?.ToUpper() == "RESERVADO") status = "Reservado";

                    StadiumSeats.Add(new Seat {
                        Id = a.Codigo,
                        Fila = a.Fila,
                        Numero = a.Numero,
                        Status = status ?? "Disponible",
                        LocalidadCodigo = a.LocalidadCodigo,
                        LocalidadDescripcion = loc?.Descripcion ?? a.LocalidadCodigo,
                        PartidoCodigo = a.PartidoCodigo,
                        EstadioCodigo = EstadioCodigo,
                        Price = loc?.PrecioBase ?? 0m,
                    });
                }
            } 
            catch 
            {
                var asientos = GenerarAsientos(partidoId, (PartidoLocal, PartidoVisitante, PartidoFecha, EstadioNombre, EstadioCodigo));
                StadiumSeats.AddRange(asientos);
            }

            SincronizarCarrito();
        }

        private void SincronizarCarrito()
        {
            foreach (var cartSeat in Cart)
            {
                var match = StadiumSeats.FirstOrDefault(s => s.Id == cartSeat.Id);
                if (match != null)
                    match.Status = "Seleccionado";
            }
        }

        public void SeleccionarLocalidad(string localidadCodigo)
        {
            LocalidadActivaCodigo = localidadCodigo;
        }

        public void VolverASecciones()
        {
            LocalidadActivaCodigo = null;
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

        public (int total, int disponibles, int ocupados) GetLocalidadStats(string localidadCodigo)
        {
            var seats = GetAsientosDeLocalidad(localidadCodigo);
            return (
                seats.Count,
                seats.Count(s => s.IsAvailable),
                seats.Count(s => s.IsOccupied || s.IsReserved)
            );
        }

        public bool ToggleSeat(string seatId)
        {
            var seat = StadiumSeats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null || seat.IsOccupied || seat.IsReserved) return false;

            if (seat.IsAvailable)
            {
                seat.Status = "Seleccionado";
                Cart.Add(seat);
                return true;
            }
            else if (seat.IsSelected)
            {
                seat.Status = "Disponible";
                Cart.RemoveAll(s => s.Id == seat.Id);
                return true;
            }
            return false;
        }

        public bool ToggleSeatByFilaYNumero(string fila, int numero)
        {
            var seats = GetAsientosDeLocalidadActiva();
            var seat = seats.FirstOrDefault(s => s.Fila.Equals(fila, StringComparison.OrdinalIgnoreCase) && s.Numero == numero);
            if (seat == null) return false;
            return ToggleSeat(seat.Id);
        }

        public bool ToggleSeatByNumero(int numero)
        {
            var seats = GetAsientosDeLocalidadActiva();
            var seat = seats.FirstOrDefault(s => s.Numero == numero);
            if (seat == null) return false;
            return ToggleSeat(seat.Id);
        }

        public Seat? GetAsientoByNumero(int numero)
        {
            return GetAsientosDeLocalidadActiva().FirstOrDefault(s => s.Numero == numero);
        }

        public decimal GetTotal() => Cart.Sum(s => s.Price);
        public string[] GetCodigosAsientos() => Cart.Select(s => s.Id).ToArray();



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

