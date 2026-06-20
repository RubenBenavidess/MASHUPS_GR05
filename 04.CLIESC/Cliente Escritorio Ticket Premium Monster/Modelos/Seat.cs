namespace ec.edu.monster.TicketClient.Modelos
{
    public class Seat
    {
        public string Id { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string LocalidadCodigo { get; set; } = string.Empty;
        public string LocalidadDescripcion { get; set; } = string.Empty;
        public string PartidoCodigo { get; set; } = string.Empty;
        public string EstadioCodigo { get; set; } = string.Empty;
        public string Fila { get; set; } = string.Empty;
        public int Numero { get; set; }
        public decimal Price { get; set; }

        public string Status { get; set; } = "Disponible";

        public bool IsAvailable => Status == "Disponible";
        public bool IsOccupied => Status == "Ocupado";
        public bool IsSelected => Status == "Seleccionado";
        public bool IsReserved => Status == "Reservado";
    }
}
