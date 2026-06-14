using System;
using System.Collections.Generic;
using System.Text;

namespace ec.edu.monster.TicketClient.Modelos
{
    public class Seat
    {
        public string Id { get; set; }
        public string Zone { get; set; }
        public decimal Price { get; set; }

        // Estados: "Disponible", "Ocupado", "Seleccionado"
        public string Status { get; set; } = "Disponible";
    }
}
