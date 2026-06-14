using ec.edu.monster.TicketClient.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ec.edu.monster.TicketClient.Servicios
{
    public class TicketingService
    {
        public List<Seat> StadiumSeats { get; private set; } = new();
        public List<Seat> Cart { get; private set; } = new();
        public string PartidoActualId { get; private set; }

        // Evento para avisarle a la vista que algo cambió (ej. se actualizó el carrito)
        public event Action? OnStateChanged;

        public TicketingService()
        {
            // Simulamos cargar los asientos de una base de datos
            LoadMockData();
        }


        // Cambia el parámetro de int a string
        public void CargarAsientosPorPartido(string partidoId)
        {
            PartidoActualId = partidoId;
            Cart.Clear();
            StadiumSeats.Clear();

            // Simulación de datos
            StadiumSeats.Add(new Seat { Id = "Norte-1", Zone = "Norte", Price = 15.00m, Status = "Disponible" });
            StadiumSeats.Add(new Seat { Id = "Norte-2", Zone = "Norte", Price = 15.00m, Status = "Ocupado" });
            StadiumSeats.Add(new Seat { Id = "Sur-1", Zone = "Sur", Price = 10.00m, Status = "Disponible" });

            NotifyStateChanged();
        }

        public void ToggleSeat(string seatId)
        {
            var seat = StadiumSeats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null || seat.Status == "Ocupado") return;

            if (seat.Status == "Disponible")
            {
                seat.Status = "Seleccionado";
                Cart.Add(seat);
            }
            else if (seat.Status == "Seleccionado")
            {
                seat.Status = "Disponible";
                Cart.Remove(seat);
            }

            // Notificamos a la UI que debe redibujarse
            NotifyStateChanged();
        }

        public decimal GetTotal() => Cart.Sum(s => s.Price);

        private void NotifyStateChanged() => OnStateChanged?.Invoke();

        private void LoadMockData()
        {
            // Generamos algunos asientos de prueba
            StadiumSeats.Add(new Seat { Id = "Norte-1", Zone = "Norte", Price = 15.00m, Status = "Disponible" });
            StadiumSeats.Add(new Seat { Id = "Norte-2", Zone = "Norte", Price = 15.00m, Status = "Ocupado" });
            StadiumSeats.Add(new Seat { Id = "Sur-1", Zone = "Sur", Price = 10.00m, Status = "Disponible" });
        }
    }
}
